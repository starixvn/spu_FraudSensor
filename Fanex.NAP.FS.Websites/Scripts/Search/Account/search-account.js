function search_account() {
    var ddlSubscriber = $('#ddlSubscriber');
    var btnSearch = $('#btnSearch');
    var inputAccount = $('#account');
    var captionError = $('#caption-error');
    var table = $('#table-account');
    var content = $('.content-account');
    var regex = new RegExp("^[a-zA-Z0-9$ ]*$");

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

        $(captionError).hide();        

        GetSubscribers();

        GenerateGrid();

        var inputValue = $(inputAccount).val();

        if (inputValue != '') {
            SearchSubscriberAccount(inputValue);
        }
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

        if (!ValidateInput(key)) {
            return;
        }

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
                $(content).show();

                BindDataToGrid(data);
            },
            error: function (errorMessage) {
                common.closeLoading();
                console.log(errorMessage);
                common.showError('Search Account fail!');
            }
        });
    }

    function GenerateGrid() {
        table.DataTable({            
            searching: false,
            paging: false,            
            lengthMenu: [1000],
            pageLength: 1000,
            dom: "t<'pagination'<'page-size'lp>>",
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
                { data: (row, type, set, meta) => meta.row + 1 },
                { data: obj => obj.SubscriberName },
                { data: obj => RenderUserName(obj, obj.UserName) },
                { data: obj => RenderUserName(obj, obj.UserName2) }
            ]
        });

        $(content).hide();
    }

    function BindDataToGrid(data) {
        if (data != null) {
            if (data.length == 1) {
                window.location.href = common.modifyRelativeUrl('/Account/DetailAccount?id=' + data[0].AccountID);               
            } else {
                table.DataTable().clear().rows.add(data).draw();
                table.DataTable().columns.adjust();

                common.closeLoading();
            }
        }        
    }

    function RenderUserName(obj, userName) {
        var url = common.modifyRelativeUrl('/Account/DetailAccount?id=' + obj.AccountID);

        return `<a class='info-bng' 
                    href=${url}
                    target='_blank'>${userName}
                </a>`;
    }

    function ValidateInput(input) {

        if (input == null || input.length < 3 || !regex.test(input)) {

            $('span', captionError).text('The Account is not valid!');
            $(captionError).show();
            $(content).hide();

            return false;
        }

        $(captionError).hide();

        return true;
    }

    inputAccount.keyup((e) => {
        $(captionError).hide();

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