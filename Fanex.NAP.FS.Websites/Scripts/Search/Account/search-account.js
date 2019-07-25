function search_account() {
    var ddlSubscriber = $('#ddlSubscriber');
    var btnSearch = $('#btnSearch');
    var inputAccount = $('#txtAccount');
    var content_table = $("#account-content-table");

    function Init() {
        ddlSubscriber.multiselect({
            menuWidth: 225,
            menuHeight: 350,
            selectedText: "# of # selected",
            noneSelectedText: "Select options"
        }).multiselectfilter({
            width: 150,
            placeholder: ''
        });

        GetSubscribers();
    }

    function GetSubscribers() {
        var url = common.modifyRelativeUrl('/SearchAccount/GetSubscribers');

        $.get(url, function (data) {
            if (data != '') {
                ddlSubscriber.multiselect().empty();

                for (var i = 0; i < data.length; i++) {
                    ddlSubscriber.append('<option value="' + data[i].SubscriberID + '">' + data[i].SubscriberName + '</option>');
                }

                ddlSubscriber.multiselect('refresh').multiselect("checkAll");
            }
        });
    }

    function SearchSubscriberAccount(key) {
        var subscribers = [];

        $('option:selected', ddlSubscriber).each(function (idx, ele) {            
            subscribers.push($(ele).val());
        });

        common.showLoading();

        var url = common.modifyRelativeUrl('/SearchAccount/SearchSubscriberAccount');

        $.ajax(url, {
            type: 'POST',
            data: {
                key: key,
                subscribers: subscribers.join(',')
            },
            success: function (data) {               
                GenerateContent(data);
            },
            error: function (errorMessage) {
                common.closeLoading();
                console.log(errorMessage);
                common.showError('Search Account fail!');
            }
        });
    }

    function GenerateContent(data) {
        GenerateGrid();

        BindAccountToGrid(data);
    }

    function GenerateGrid() {
        content_table.DataTable({
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
            searching: false,
            ordering: false,
            lengthMenu: [10, 20, 50, 100, 500],
            pageLength: 10,
            scrollY: "61vh",
            scrollX: true,
            scrollCollapse: true,
            columnDefs: [
                { targets: "_all", defaultContent: "-" },
                { targets: [-2, -1], className: "text-center" },
                { targets: [0, 1, 2, 3], className: "text-center" },
                { targets: [0], width: "10%" },
                { targets: [1], width: "30%" },
                { targets: [2], width: "30%" }              
            ],
            columns: [
                { data: obj => "No1" },
                { data: obj => obj.SubscriberName },
                { data: obj => obj.UserName1 },
                { data: obj => obj.UserName2 }
            ]
        });
    }

    function BindAccountToGrid(data) {

        content_table.DataTable().clear().rows.add(data).draw();

        content_table.DataTable().columns.adjust();

        common.closeLoading();
    }

    inputAccount.keyup((e) => {
        if (e.which == 13) {
            SearchSubscriberAccount($(e.target).val());
        }
    });

    $(btnSearch).click(() => {
        SearchSubscriberAccount($(inputAccount).val());
    });

    Init();
}

$(search_account);