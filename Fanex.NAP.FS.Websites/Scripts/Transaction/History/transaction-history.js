function transaction_history() {
    var btnSearch = $('#btn-search-transaction');
    var status = $('#cbb-fraud-status');
    var transactionTable = $('#transaction-history-table');
    var transactionContent = $('#transaction-history-content');    

    function GenerateGrid() {
        transactionTable.DataTable({
            dom: 't<"pagination"<"page-size"><"bottom"lp<"clear">>"i>',
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
            pageLength: 20,           
            scrollX: false,
            scrollCollapse: false,
            columnDefs: [
                { targets: "_all", defaultContent: "-" },
                { targets: [-2, -1], className: "text-center" },
                { targets: [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12], className: "text-center" }
            ],
            columns: [
                { data: obj => moment(obj.TransDate).format("MMM DD, YYYY") },
                { data: obj => RenderDevice(obj) },
                { data: obj => obj.RobotTracking },
                { data: obj => obj.Action },
                { data: obj => obj.ActionResult },
                { data: obj => obj.DeviceCode },                                
                { data: obj => obj.Browser },
                { data: obj => obj.Domain },
                { data: obj => obj.IP },
                { data: obj => obj.Country },
                { data: obj => obj.Region },
                { data: obj => obj.City },
                { data: obj => obj.ServiceProvider }
            ]
        });
    }

    function RenderDevice(obj) {
        var url = common.modifyRelativeUrl('/Device/DetailDevice/' + obj.DeviceID);

        var html = '<span class= "' + obj.Flagged + '">';
        html += '<a class="info-bng" href="' + url + '" target="_blank">' + obj.DeviceCode + '</a>';
        html += '</span>';

        return html;
    }

    function RenderRobotTracking(obj) {
        return '<span class=robot-tracking>' + obj.RobotTracking + '</span>';
    }

    function GenerateReport() {
        var url = common.modifyRelativeUrl('/Transaction/GetTransactionHistoryFilter');
        var reportLength = $(status).val();

        $.ajax(url, {
            type: 'POST',
            data: {
                AccountId: ACCOUNTID,
                ReportLength: reportLength
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