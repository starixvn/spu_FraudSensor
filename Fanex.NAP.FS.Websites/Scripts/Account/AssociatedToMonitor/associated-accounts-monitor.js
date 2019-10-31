function associated_account_monitor() {
    var filter_sites = $('#filter-sites');
    var filter_cust_levels = $('#filter-cust-levels');
    var filter_evidences = $('#filter-evidences');
    var filter_date_from = $('#filter-associated-date-from');
    var filter_date_to = $('#filter-associated-date-to');
    var btn_generate = $('#btn-generate');
    var grid_associated_account = $('#grid-associated-account');

    function init() {
        filter_sites.multiselect({
            menuWidth: 225,
            menuHeight: 350
        }).multiselect('checkAll')
            .multiselectfilter({
                width: 145,
                placeholder: '',
                label: 'Filter'
            });

        filter_cust_levels.multiselect({
            menuWidth: 225,
            menuHeight: 350
        }).multiselect('checkAll')
            .multiselectfilter({
                width: 145,
                placeholder: '',
                label: 'Filter'
            });

        filter_evidences.multiselect({
            menuWidth: 330,
            menuHeight: 350
        }).multiselect('checkAll')
            .multiselectfilter({
                width: 250,
                placeholder: '',
                label: 'Filter'
            });

        var dateFromMaxLimit = new Date();
        var dateFromMinLimit = new Date();
        dateFromMinLimit.setDate(dateFromMinLimit.getDate() - 29);

        filter_date_from.datepicker({
            dateFormat: "M dd, yy",
            changeMonth: true,
            changeYear: true,
            firstDay: 1,
            maxDate: "y"
        }).datepicker("option", "minDate", dateFromMinLimit)
            .datepicker("option", "maxDate", dateFromMaxLimit)
            .datepicker("setDate", new Date());

        filter_date_to.datepicker({
            dateFormat: "M dd, yy",
            changeMonth: true,
            changeYear: true,
            firstDay: 1,
            maxDate: "y"
        }).datepicker("setDate", new Date())
            .change(restrictFilterDateFrom);

        renderAssociatedAccountGridOptions();
    }

    function restrictFilterDateFrom() {
        var dateMinLimit = filter_date_to.datepicker("getDate");
        var dateMaxLimit = filter_date_to.datepicker("getDate");

        if (dateMinLimit != null) {
            dateMinLimit.setDate(dateMinLimit.getDate() - 29);
            filter_date_from.datepicker("option", "minDate", dateMinLimit)
                .datepicker("option", "maxDate", dateMaxLimit);
        }
    }

    function renderAssociatedAccountGridOptions() {
        grid_associated_account.DataTable({
            searching: false,
            lengthMenu: [10, 20, 50, 100, 500],
            pageLength: 50,
            scrollY: "61vh",
            scrollX: true,
            scrollCollapse: true,
            columnDefs: [
                { targets: "_all", defaultContent: "-" },                
                { targets: [0, 2, 3, 4, 5, 6], orderable: false }
            ],
            columns: [
                { data: (row, type, set, meta) => meta.row + 1, className: "text-center", width: "5%" },
                { data: row => row.MainAccountUsername, className: "text-center", width: "10%" },
                { data: row => row.MainAccountSite, className: "text-center", width: "10%" },
                { data: row => row.MainAccountLevelName, className: "text-center", width: "10%" },
                { data: row => renderEvidence(row.MainAccountEvidences), className: "text-left", width: "20%" },
                { data: row => renderNoOfAssociations(row), className: "text-center", width: "10%" },
                { data: row => row.NoOfVerified, className: "text-center", width: "10%" }
            ],
            dom: "t<'pagination'<'page-size'lp>i>",
            language: {
                lengthMenu: "Page size _MENU_",
                paginate: {
                    previous: "Back"
                },
                info: "Displaying _START_ to _END_ of _TOTAL_ item(s)",
                infoEmpty: "",
                zeroRecords: "There's no data"
            },
            order: [[1, "asc"]]
        });
    }

    function renderEvidence(evidences) {
        if (evidences != null && evidences != '') {
            var evidencesForDisplay = common.makeWrapString(evidences, 72);
            return "<span title='" + evidences + "'>" + evidencesForDisplay + "</span>";
        }
        return evidences;
    }

    function renderNoOfAssociations(row) {
        var filter = {};
        filter.AccountCustId = row.MainAccountCustId;
        filter.Username = row.MainAccountUsername;
        filter.NoOfAssociations = row.NoOfAssociations;
        filter.DateFrom = filter_date_from.val();
        filter.DateTo = filter_date_to.val();

        var noOfAssociationsDisplay = '';
        var noOfAssociations = row.NoOfAssociations;
        if (noOfAssociations < 1000) {
            noOfAssociationsDisplay = noOfAssociations;
        } else if (parseInt(noOfAssociations / 1e6) != 0) {
            noOfAssociationsDisplay = parseInt(noOfAssociations / 1e6) + "M";
        } else {
            noOfAssociationsDisplay = parseInt(noOfAssociations / 1e3) + "K";
        }

        return `<a href='AssociatedAccountsDetailMonitor?AccountCustId=${filter.AccountCustId}&Username=${filter.Username}&NoOfAssociations=${filter.NoOfAssociations}&DateFrom=${filter.DateFrom}&DateTo=${filter.DateTo}' target='_blank'>
            <span class='btn-no-of-associations'>${noOfAssociationsDisplay}</span></a>`;
    }

    function generateReport() {
        var url = common.modifyRelativeUrl("/Account/GetAssociatedAccountsToMonitor");
        var paramFilter = getParamsFilter();

        if (validateFilter(paramFilter)) {
            common.showLoading();

            $.ajax({
                type: 'POST',
                url: url,
                data: paramFilter,
                success: function (response) {
                    renderAssociatedAccountGrid(response);
                },
                error: function (xhr, status, error) {
                    console.log('data of ' + xhr.responseText);
                    common.closeLoading();
                }
            });
        }
    }

    function getParamsFilter() {
        var filter = {};

        filter.AssociatedDateFrom = filter_date_from.val();
        filter.AssociatedDateTo = filter_date_to.val();     
        filter.Sites = getSeletedMultiple("#filter-sites");
        filter.CustLevels = getSeletedMultiple("#filter-cust-levels");
        filter.EvidenceCodes = getSeletedMultiple("#filter-evidences");
        filter.EvidencesCache = EVIDENCECACHE;

        return filter;
    }  

    function getSeletedMultiple(multipleName) {
        var selects = [];
        $('option:selected', multipleName).each(function (idx, ele) {
            selects.push($(ele).val());
        });

        return selects.join(",");
    }

    function validateFilter(filter) {
        var isOK = true;
        $(filter_date_from).removeClass("fail-by-validate");
        $(filter_date_to).removeClass("fail-by-validate");

        if (filter.AssociatedDateFrom == "") {
            $(filter_date_from).addClass("fail-by-validate");
            isOK = false;
        }

        if (filter.AssociatedDateTo == "") {
            $(filter_date_to).addClass("fail-by-validate");
            isOK = false;
        }
        return isOK;
    }

    function renderAssociatedAccountGrid(json) {
        grid_associated_account.DataTable().clear().rows.add(json).draw();
        grid_associated_account.DataTable().columns.adjust();
        common.closeLoading();
    }
       
    init();

    btn_generate.click(generateReport);
}

$(associated_account_monitor);