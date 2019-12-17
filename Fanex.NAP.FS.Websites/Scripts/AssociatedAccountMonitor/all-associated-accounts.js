function all_associated_accounts() {
    var form = $('#form-all-association-accounts');
    var grid = $('#grid-all-association-accounts');

    this.OpenModal = function (custId, userName) {
        if (!form.hasClass("ui-dialog-content")) {
            renderAllAssociatedAccountsGridOptions();
        }       
        
        getAllAssociatedAccounts(custId, userName);        
    }

    function renderAllAssociatedAccountsGridOptions() {       
        grid.DataTable({
            searching: false,
            lengthMenu: [20, 50, 100, 500],
            pageLength: 50,
            scrollY: "65vh",
            scrollCollapse: true,
            dom: "t<'pagination'<'page-size'lp>i>",
            language: {
                lengthMenu: "Page size _MENU_",
                paginate: {
                    previous: "Back"
                },
                info: "Displaying _START_ to _END_ of _TOTAL_ item(s)",
                infoEmpty: "",
                zeroRecords: "There's no data",
                processing: 'Loading...'
            },
            columnDefs: [
                { targets: "_all", defaultContent: "-" },
                { targets: [0, 2, 3], orderable: false }
            ],
            columns: [
                { data: (row, type, set, meta) => meta.row + 1, className: "text-center", width: "15%" },
                { data: row => row.ToOWUsername, className: "text-left", width: "30%" },
                { data: row => row.SubscriberName, className: "text-center", width: "30%" },
                { data: row => row.RoleLevel, className: "text-center", width: "25%" }            
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

    function getAllAssociatedAccounts(custId, userName) {
        common.showLoading();
        var url = common.modifyRelativeUrl('/AssociatedAccountsMonitor/GetAllAssociatedAccounts');
        var filter = {};
        filter.CustIdList = custId;       
        filter.RoleIdList = '1,2,3,4';

        $.ajax({
            type: 'POST',
            url: url,
            data: filter,
            success: function (response) {
                renderAllAssociatedAccounts(response, userName);
            },
            error: function (xhr, status, error) {
                console.log('data of ' + xhr.responseText);
                common.closeLoading();
                common.showError();
            }
        });       
    }

    function renderAllAssociatedAccounts(data, userName) {
        common.closeLoading();
        openForm(userName);

        grid.DataTable().clear().rows.add(data).draw();
        grid.DataTable().columns.adjust();
    }

    function openForm(userName) {
        var titleFormat = 'Associated Accounts of customer {0}';
        form.dialog({
            modal: true,
            width: '980px',
            title: titleFormat.format(userName),
            resizable: false,
            draggable: false,
            close: function () {                
                $(this).dialog('close');
            }
        })
    }
}