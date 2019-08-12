function device_by_account() {
    var btnSearch = $('#btn-search-device');
    var usernameSpan = $('.username-span');
    var associatedTable = $('#device-by-account-table');
    var associatedContent = $('#device-by-account-content');

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
                { targets: [-2, -1], className: "text-center" },
                { targets: [1, 2, 3, 4], className: "text-center" },                
                { targets: [1], width: "15%" },
                { targets: [2], width: "15%" },
                { targets: [3], width: "15%" },
                { targets: [4], width: "15%" },
                { targets: [5], width: "20%" }
            ],
            columns: [
                { data: obj => RenderUserName(obj) },            
                { data: obj => moment(obj.FirstLogin).format("MMM DD, YYYY") },
                { data: obj => moment(obj.LastLogin).format("MMM DD, YYYY") },              
                { data: obj => moment(obj.AssociatedDate).format("MMM DD, YYYY") },
                { data: obj => obj.TotalUses },
                { data: obj => RenderGraphImage() }
            ],
            headerCallback: function (thread) {
                $("th", thread).eq(3).text('Associated with ' + USERNAME + ' on'),
                $("th", thread).eq(5).text('Path to ' + USERNAME)
            }
        });       
    }

    function RenderUserName(obj) {
        var url = common.modifyRelativeUrl('/Device/DetailDevice/' + obj.DeviceNo);
        var className = 'icon-device-fraud';

        if (obj.FraudStatus == 'Clean') {
            className = 'icon-device-clean';
        }        

        var html = '<span class="' + className + '"></span>';
        html += '<a class="info-bng" href="' + url + '" target="_blank">' + obj.DeviceNo + '</a>';
        //html += '</span>';

        return html;
    }

    function RenderGraphImage() {
        return '<span class="icon-path graph" title="Grap View"></span>';
    }

    function GenerateReport() {
        var usedByAccountCurrent = $('#chk-used-by-account-current').is(':checked');
        var usedByAccountAssociated = $('#chk-used-by-account-associated').is(':checked');       
        var fraudStatus = $('#cbb-fraud-status').val();
        var url = common.modifyRelativeUrl('/Account/GetAssociatedDeviceByAccountFilter');

        $.ajax(url, {
            type: 'POST',
            data: {
                AccountId: ACCOUNTID,
                UsedByAccountCurrent: usedByAccountCurrent,
                UsedByAccountAssociated: usedByAccountAssociated,
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

    usernameSpan.text(USERNAME);

    btnSearch.click(GenerateReport);

    GenerateGrid();
}

$(device_by_account);