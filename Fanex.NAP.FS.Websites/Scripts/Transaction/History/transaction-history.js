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
                { targets: [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10], className: "text-center" },
                { targets: [0], width: "12%" },
                { targets: [1], width: "15%" },
                { targets: [2], width: "10%" },               
                { targets: [4], width: "10%" },
                { targets: [5], width: "6%" },
                { targets: [6], width: "8%" },
                { targets: [7], width: "7%" },
                { targets: [8], width: "6%" },
                { targets: [9], width: "6%" },
                { targets: [10], width: "20%" }
            ],
            columns: [
                { data: obj => moment(obj.TransDate).format("MMM DD, YYYY hh:mm:ss A") },
                { data: obj => RenderDevice(obj) },
                { data: obj => obj.ActionResult },
                { data: obj => obj.DeviceType },
                { data: obj => obj.Browser },
                { data: obj => RenderHostName(obj.Domain) },
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

        var html = '<span class= "device-' + obj.DeviceStatus + '">';
        html += '<a class="info-bng" href="' + url + '" target="_blank">' + obj.DeviceCode + '</a>';
        html += '</span>';

        return html;
    }  

    function RenderHostName(url) {
        var urlNew = new URL(url);
        return urlNew.hostname;
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