function associated_account() {
    var filter_evidences = $('#filter-evidences');
    var filter_verify_status = $('#filter-verify-status');
    var filter_date_from = $('#filter-associated-date-from');
    var filter_date_to = $('#filter-associated-date-to');
    var btn_generate = $('#btn-generate');
    var grid_associated_account = $('#grid-associated-account');

    function init() {
        $(filter_evidences).multiselect({
            menuWidth: 330,
            menuHeight: 350
        }).multiselectfilter({
            width: 250,
            placeholder: '',
            label: 'Filter'
        });

        $(filter_date_from).datepicker({
            dateFormat: "M dd, yy",
            changeMonth: true,
            changeYear: true,
            firstDay: 1,
            minDate: "-1y +1d"
        });

        $(filter_date_to).datepicker({
            dateFormat: "M dd, yy",
            changeMonth: true,
            changeYear: true,
            firstDay: 1
        });

        renderAssociatedAccountGridOptions();
    }

    function generateReport() {
        var url = common.modifyRelativeUrl("/Account/GetAssociatedAccounts");
        var paramFilter = getParamFilter();
        common.showLoading();

        $.ajax({
            type: 'POST',
            url: url,
            data: paramFilter,
            global: false,
            crossDomain: true,
            success: function (response) {
                renderAssociatedAccountGrid(response);
            },
            error: function (xhr, status, error) {
                console.log('data of ' + xhr.responseText);
                common.showLoading();
            }
        });
    }

    function getParamFilter() {
        var filter = {};

        filter.AssociatedDateFrom = $('').val();
        filter.AssociatedDateTo = $('').val();
        filter.VerifyStatus = $('').val();
        filter.EvidenceCodes = getSeletedEvidences();

        return filter;
    }

    function getSeletedEvidences() {
        var evidences = [];
        $('option:selected', "#ddlSites").each(function (idx, ele) {
            evidences.push($(ele).val());
        });

        return evidences.join(",");
    }

    function renderAssociatedAccountGrid(json) {
        grid_associated_account.DataTable().clear().rows.add(json).draw();
        common.closeLoading();
    }

    function renderAssociatedAccountGridOptions() {
        grid_associated_account.DataTable({
            searching: false,
            lengthMenu: [10, 20, 50, 100, 500],
            pageLength: 50,
            ordering: false,
            scrollY: "61vh",
            scrollX: true,
            scrollCollapse: true,
            columnDefs: [
                { targets: "_all", defaultContent: "-" },
                { targets: 10, type: "check-box" }
            ],
            columns: [
                { data: (row, type, set, meta) => meta.row + 1, className: "text-center", width: "40px" },
                { data: row => row.AssociatedAccountUsername, className: "text-center", width: "40px" },
                { data: row => row.AssociatedAccountSite, className: "text-center", width: "100px" },
                { data: row => row.AssociatedAccountLevel, className: "text-center", width: "100px" },
                { data: row => row.AssociatedAccountEvidences, className: "text-center", width: "60px" },
                { data: row => row.MainAccountUsername, className: "text-center", width: "40px" },
                { data: row => row.MainAccountSite, className: "text-center", width: "100px" },
                { data: row => row.MainAccountLevel, className: "text-center", width: "100px" },
                { data: row => row.MainAccountEvidences, className: "text-center", width: "60px" },
                { data: row => row.FirstAssociated, className: "text-center", width: "100px" },
                { data: renderVerifyCheckBox, className: "text-center", width: "100px" },
                { data: row => row.VerifiedBy, className: "text-center", width: "100px" },
                { data: row => row.Note, className: "text-center", width: "100px" },                
                //{
                //    data: renderNote, className: "col-note", width: "300px", createdCell: (cell, _, row) => {
                //        //$(".icon-note", cell).click(() => renderNoteEdit(cell, row));
                //        //$(".note-ellipsis", cell).click(() => expandNote(cell));
                //    }
                //}
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
            //headerCallback: th => $("th", th).removeClass("has-icon")
        });
    }

    function renderVerifyCheckBox(obj) {
        return "<label class='checkbox'>"
            + "<input type= 'checkbox' class='verified-checkbox'" + (obj.Verified ? "checked" : "")+ "/>"
            + "<span></span></label>";
    }

    init();

    btn_generate.click(generateReport);
}

$(associated_account);