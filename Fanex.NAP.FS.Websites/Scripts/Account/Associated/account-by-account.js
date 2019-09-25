function account_by_account() {
    var loading = $('#associated-account-loading-div');
    var btnSearch = $('#btn-search-account-ass-account');
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
                { data: obj => obj.FirstLoginToString},
                { data: obj => obj.LastLoginToString},
                { data: obj => obj.TotalLoginWithRootAccount },
                { data: obj => obj.AssociatedWithRootAccountOnToString},
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
        var url = common.modifyRelativeUrl('/Account/DetailAccount?id=' + obj.AccountID
            + '&custId=' + obj.CustID);
        var className = 'icon-clean';

        if (obj.EvidenceLevel == "0") {
            className = 'icon-fraud-flag';
        } else if (obj.EvidenceLevel == "2") {
            className = 'icon-fraud';
        }

        var html = '<span class="' + className + '"></span>';
        html += '<a class="info-bng" href="' + url + '" target="_blank">' + obj.Username + "(" + obj.Username2 + ")" + '</a>';
      
        return html;
    }

    function GenerateReport() {
        var deviceAwayOne = $('#chk-device-away-1').is(':checked');
        var deviceAwayMore = $('#chk-device-away-2').is(':checked');
        var status = $('#cbb-status-ass-account').val();
        var evidenceStatus = $('#cbb-evidence-status-ass-account').val();
        var url = common.modifyRelativeUrl('/Account/GetAssociatedAccountByAccountFilter');

        if (!ValidateInputSearch(deviceAwayOne, deviceAwayMore)) {
            associatedTable.DataTable().clear().draw();
            return;
        }

        associatedContent.hide();
        loading.show();

        $.ajax(url, {
            type: 'POST',
            data: {
                AccountId: ACCOUNTID,
                CustId: CUSTID,
                DeviceAwayOne: deviceAwayOne,
                DeviceAwayMore: deviceAwayMore,
                Status: status.join(','),
                EvidenceStatus: evidenceStatus
            },
            success: function (response) {
                BindDataToGrid(response);
                AnimateContent();
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

    function AnimateContent() {
        $('html, body').animate({
            scrollTop: $(associatedContent).offset().top
        }, 1500);
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

    function ValidateInputSearch(deviceAwayOne, deviceAwayMore) {
        if (!deviceAwayOne && !deviceAwayMore) {
            return false;
        }
        return true;
    }

    function OpenGraphRender() {
        $('#form-graph-render').dialog({
            modal: true,
            width: '1500px',
            title: 'Graph visualization',
            dialogClass: "modal-graph-render",
            resizable: true,
            draggable: true,
            close: function () { $(this).dialog('close'); }
        })
    }

    GenerateGrid();

    InitSearchOnGrid();

    $("#cbb-status-ass-account").multiselect({
        menuWidth: 225,
        menuHeight: 225,
        selectedText: "# of # selected",
        noneSelectedText: "Select options"
    });

    usernameSpan.text(USERNAME);

    loading.hide(); 

    btnSearch.click(GenerateReport);    

    $('#btn-ass-acc').click(OpenGraphRender);    

    function RenderGraphImage() {
        return '<span class="icon-path graph" title="Graph View" onclick="OpenShortestPath(22971808,4677919)"></span>';
    }
}

$(account_by_account);

function OpenShortestPath(startCustId, endCustId) {
    $('#shortest-path-render').dialog({
        modal: true,
        width: '1500px',
        title: 'Shortest Path',
        dialogClass: "modal-graph-render",
        resizable: true,
        draggable: true,
        open: function () {
            $.ajax({
                type: "POST",
                data: { startCustId: startCustId, endCustId: endCustId },
                url: "GetShortestPath",
                beforeSend: function() {
                    alert('before send');
                },
                success: function () {

                }
            });
        },
        close: function () { $(this).dialog('close'); }
    })
}