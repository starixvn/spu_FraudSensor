function transaction_history() {
    var btnSearch = $('#btn-search-transaction');
    var transactionTable = $('#transaction-history-table');
    var transactionContent = $('#transaction-history-content');

    function GenerateGrid() {
        transactionTable.DataTable({
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
            ordering: false,
            lengthMenu: [10, 20, 50, 100, 200],
            pageLength: 10,
            scrollY: "61vh",
            scrollX: true,
            scrollCollapse: true,
            columnDefs: [
                { targets: "_all", defaultContent: "-" },
                { targets: [-2, -1], className: "text-center" }
            ],
            columns: [
                { data: obj => moment(obj.TransDate).format("MMM DD, YYYY") },
                { data: obj => obj.Device.DeviceNo },
                { data: obj => obj.RobotTracking },
                { data: obj => obj.Action },
                { data: obj => obj.ActionResult },
                { data: obj => obj.Device.OSName },                                
                { data: obj => obj.Browser },
                { data: obj => obj.Domain },
                { data: obj => obj.IPAddress },
                { data: obj => obj.Country },
                { data: obj => obj.Region },
                { data: obj => obj.City },
                { data: obj => obj.ServiceProvider }
            ]
        });
    }

    function GenerateReport() {
        var url = common.modifyRelativeUrl('/Transaction/GetTransactionHistoryFilter');

        $.ajax(url, {
            type: 'POST',
            data: {
                AccountId: ACCOUNTID,
                ReportLength: '24hours'
            },
            success: function (response) {
                BindDataToGrid(response);
            },
            error: function (errMsg) {
                console.log(errMsg);
                common.showError(errMsg);
            }
        });
    }

    function BindDataToGrid(data) {
        if (transactionContent.is(':hidden')) {
            transactionContent.removeAttr('hidden');
        }

        transactionTable.DataTable().clear().rows.add(data).draw();
        transactionTable.DataTable().columns.adjust();
    }

    btnSearch.click(GenerateReport);

    GenerateGrid();
}

$(transaction_history);