function account_by_account() {
    var btnSearch = $('#btn-search-account');
    var accountSearch = $('#account-search');
    var usernameSpan = $('.username-span');
    var associatedTable = $('#account-by-account-table'); 
    var associatedContent = $('#account-by-account-content'); 

    function GenerateGrid() {
        associatedTable.DataTable({
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
                { targets: [-2, -1], className: "text-center" },
                { targets: [0, 1, 2, 3, 4, 5, 6], className: "text-center" },
                { targets: [0], width: "15%" },
                { targets: [1], width: "15%" },
                { targets: [2], width: "15%" },
                { targets: [3], width: "15%" },
                { targets: [4], width: "15%" },
                { targets: [5], width: "15%" }
            ],
            columns: [
                { data: obj => RenderUserName(obj)},
                { data: obj => obj.Status },
                { data: obj => moment(obj.AccountInfoAssociated.FirstLogin).format("MMM DD, YYYY")},
                { data: obj => moment(obj.AccountInfoAssociated.LastLogin).format("MMM DD, YYYY")},
                { data: obj => obj.TotalSharedDeviceLogin },
                { data: obj => moment(obj.AccountInfoAssociated.AssociatedDate).format("MMM DD, YYYY")},
                { data: obj => RenderGraphImage()}
            ],
            headerCallback: function (thread) {
                $("th", thread).eq(4).text('Total Logins on shared device with ' + USERNAME),
                $("th", thread).eq(5).text('Associated with ' + USERNAME + ' on'),
                $("th", thread).eq(6).text('Path to ' + USERNAME)
            }
        });        
    }

    function RenderUserName(obj) {
        var html = '<div class="account-status">';
        html += '<a href="#">' + obj.AccountAssociated.UserName + "(" + obj.AccountAssociated.UserName2 + ")" + '</a>';
        html += '</div>';

        return html;
    }

    function RenderGraphImage() {
        return '<span class="graph" title="Grap View"></span>';
    }    

    function GenerateReport() {
        var deviceAwayOne = $('#chk-device-away-1').is(':checked');
        var deviceAwayMore = $('#chk-device-away-2').is(':checked');
        var status = $('#cbb-status').val();
        var fraudStatus = $('#cbb-fraud-status').val();
        var url = common.modifyRelativeUrl('/Account/GetAssociatedAccountByAccountFilter');

        $.ajax(url, {
            type: 'POST',
            data: {
                AccountId: ACCOUNTID,
                DeviceAwayOne: deviceAwayOne,
                DeviceAwayMore: deviceAwayMore,
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
        if (associatedContent.is(':hidden')) {
            associatedContent.removeAttr('hidden');
        }

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

    btnSearch.click(GenerateReport);

    GenerateGrid();

    InitSearchOnGrid();
}

$(account_by_account);