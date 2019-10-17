function transaction_history() {
    var loading = $('#transaction-history-loading');
    var btnSearch = $('#transaction-history-btn-search');
    var reportLength = $('#transaction-history-cbb-report-length');
    var transactionTable = $('#transaction-history-table');
    var transactionContent = $('#transaction-history-content');
    var formIPInformation = new form_ip_information();

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
                { targets: [0, 1, 2, 3, 4, 5, 6], className: "text-center" },
                { targets: [0], width: "15%" },
                { targets: [1], width: "15%" },
                { targets: [2], width: "15%" },
                { targets: [3], width: "10%" },
                { targets: [4], width: "15%" },
                {
                    targets: [5],
                    width: "15%",
                    visible: IsPermissionIPLocation.toLowerCase() == 'true' ? true : false
                },
                {
                    targets: [6],
                    width: "15%",
                    visible: IsPermissionIPLocation.toLowerCase() == 'true' ? true : false
                }
            ],
            columns: [
                { data: obj => obj.TransDateToString },
                { data: obj => RenderDevice(obj) },
                { data: obj => obj.ActionResult },
                { data: obj => obj.DeviceType },
                { data: obj => obj.Browser },
                { data: obj => RenderHostName(obj.Domain) },
                { data: (obj, type, set, meta) => RenderIPInformation(obj, meta.row + 1) }
            ]
        });

        transactionTable.on('click', '.ip-link', ShowFormIPInformation);
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

    function RenderIPInformation(obj, rowIndex) {
        return "<span class='link ip-link' id=ip-address-" + rowIndex + ">" + obj.IP + "</span>";
    }

    function GenerateReport() {
        transactionContent.hide();
        loading.show();

        var url = common.modifyRelativeUrl('/Transaction/GetTransactionHistoryFilter');
        var lengthReport = $(reportLength).val();

        $.ajax(url, {
            type: 'POST',
            data: {
                AccountId: ACCOUNTID,
                ReportLength: lengthReport
            },
            success: function (response) {
                BindDataToGrid(response);
                AnimateContent();
            },
            error: function (errMsg) {
                console.log(errMsg);
                common.showError(errMsg);
                loading.hide();
                transactionContent.show();
            }
        });
    }

    function BindDataToGrid(data) {
        loading.hide();
        transactionContent.show();

        transactionTable.DataTable().clear().rows.add(data).draw();
        transactionTable.DataTable().columns.adjust();        
    }

    function ShowFormIPInformation() {
        var valueIP = $(this).text();
       
        var targetIPAddress = "#" + $(this).attr("id");

        formIPInformation.OpenModal(valueIP, targetIPAddress);
    }

    function AnimateContent() {
        $('html, body').animate({
            scrollTop: $(transactionContent).offset().top
        }, 1500);
    }

    loading.hide();

    $('#form-ip-information').hide();

    transactionContent.hide();

    btnSearch.click(GenerateReport);

    GenerateGrid();    
}

$(transaction_history);