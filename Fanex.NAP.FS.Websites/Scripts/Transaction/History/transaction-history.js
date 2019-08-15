function transaction_history() {
    var loading = $('#loading-div');
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
                { targets: [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12], className: "text-center" },
                { targets: [0], width: "11%" },
                { targets: [1], width: "10%" },
                { targets: [2], width: "10%" },
                { targets: [3], width: "8%" },
                { targets: [4], width: "8%" },
                { targets: [5], width: "5%" },
                { targets: [6], width: "5%" },
                { targets: [7], width: "10%" },
                { targets: [8], width: "10%" },
                { targets: [9], width: "5%" },
                { targets: [10], width: "5%" },
                { targets: [11], width: "5%" },
                { targets: [12], width: "10%" }
            ],
            columns: [
                { data: obj => moment(obj.TransDate).format("MMM DD, YYYY hh:mm:ss A") },
                { data: obj => RenderDevice(obj) },
                { data: obj => RenderTracking(obj) },
                { data: obj => obj.Action },
                { data: obj => obj.ActionResult },
                { data: obj => obj.DeviceType },                                
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
        var url = common.modifyRelativeUrl('/Device/DetailDevice/' + obj.DeviceCode);

        var html = '<span class= "device-' + obj.Flagged + '">';
        html += '<a class="info-bng" href="' + url + '" target="_blank">' + obj.DeviceCode + '</a>';
        html += '</span>';

        return html;
    }

    function RenderTracking(obj) {
        if (obj.RobotTracking == 'Human') {
            return '<span class=tracking-human>' + obj.RobotTracking + '</span>';
        }

        return '<span class=tracking-robot>' + obj.RobotTracking + '</span>';
    }

    function GenerateReport() {
        transactionContent.hide();
        loading.show();

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
        loading.hide();
        transactionContent.show();

        transactionTable.DataTable().clear().rows.add(data).draw();
        transactionTable.DataTable().columns.adjust();
    }

    loading.hide();

    transactionContent.hide();

    btnSearch.click(GenerateReport);

    GenerateGrid();
}

$(transaction_history);