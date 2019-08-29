function account_by_account() {
    var loading = $('#loading-div');
    var btnSearch = $('#btn-search-account');
    var accountSearch = $('#account-search');
    var usernameSpan = $('.username-span');
    var associatedTable = $('#account-by-account-table'); 
    var associatedContent = $('#account-by-account-content'); 

    function GenerateGrid() {
        associatedTable.DataTable({
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
                { targets: [-2,-1], className: "text-center" },
                { targets: [1, 2, 3, 4, 5, 6], className: "text-center" },
                { targets: [0], width: "15%" },
                { targets: [1], width: "15%" },
                { targets: [2], width: "15%" },
                { targets: [3], width: "15%" },
                { targets: [4], width: "15%" },
                { targets: [5], width: "15%" }
            ],
            columns: [
                { data: obj => RenderUsername(obj) },
                { data: obj => obj.Status },
                { data: obj => moment(obj.FirstLogin).format("MMM DD, YYYY")},
                { data: obj => moment(obj.LastLogin).format("MMM DD, YYYY")},
                { data: obj => obj.TotalLoginWithRootAccount },
                { data: obj => moment(obj.AssociatedWithRootAccountOn).format("MMM DD, YYYY")},
                { data: obj => RenderGraphImage()}
            ],
            headerCallback: function (thread) {
                $("th", thread).eq(4).text('Total Logins on shared device with ' + USERNAME),
                $("th", thread).eq(5).text('Associated with ' + USERNAME + ' on'),
                $("th", thread).eq(6).text('Path to ' + USERNAME)
            }
        });
    }

    function RenderUsername(obj) {
        var url = common.modifyRelativeUrl('/Account/DetailAccount?id=' + obj.AccountID);
        var className = 'icon-fraud';

        if (obj.EvidenceLevel == 0) {
            className = 'icon-clean';
        }

        var html = '<span class="' + className + '"></span>';
        html += '<a class="info-bng" href="' + url + '" target="_blank">' + obj.Username + "(" + obj.Username2 + ")" + '</a>';
      
        return html;
    }

    function RenderGraphImage() {
        return '<span class="icon-path graph" title="Graph View"></span>';
    }    

    function GenerateReport() {
        associatedContent.hide();
        loading.show();
        var deviceAwayOne = $('#chk-device-away-1').is(':checked');
        var status = $('#cbb-status').val();
        var fraudStatus = $('#cbb-fraud-status').val();
        var url = common.modifyRelativeUrl('/Account/GetAssociatedAccountByAccountFilter');

        $.ajax(url, {
            type: 'POST',
            data: {
                AccountId: ACCOUNTID,
                CustId: CUSTID,
                DeviceAwayOne: deviceAwayOne,              
                Status: status,
                FraudStatus: fraudStatus
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
        associatedContent.show();

        associatedTable.DataTable().clear().rows.add(data).draw();
        associatedTable.DataTable().columns.adjust();
    }

    function InitSearchOnGrid() {
        associatedTable.DataTable().columns([0]).every(function () {
            var column = this;

            accountSearch.keyup(function () {
                if (column.search() !== this.value) {
                    column.search(this.value).draw();
                }
            });
        });
    }

    usernameSpan.text(USERNAME);

    loading.hide();

    $('#chk-device-away-1').on('click', function () {
        if ($(this).is(':checked')) {
            $('#chk-device-away-2').prop("checked", false);
        } else {
            $('#chk-device-away-2').prop("checked", true);
        }
    })

    $('#chk-device-away-2').on('click', function () {
        if ($(this).is(':checked')) {
            $('#chk-device-away-1').prop("checked", false);
        } else {
            $('#chk-device-away-1').prop("checked", true);
        }
    })

    btnSearch.click(GenerateReport);

    GenerateGrid();

    InitSearchOnGrid();
}

$(account_by_account);