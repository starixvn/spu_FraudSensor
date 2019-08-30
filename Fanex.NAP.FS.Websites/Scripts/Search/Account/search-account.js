function search_account() {
    var ddlSubscriber = $('#ddlSubscriber');
    var btnSearch = $('#btnSearch');
    var inputAccount = $('#account');
    var captionErrorAccount = $('#caption-error-account');
    var captionErrorSubscribers = $('#caption-error-subscribers');
    var table = $('#table-account');
    var content = $('.content-account');
    var regex = new RegExp("^[a-zA-Z0-9 $@#&_*.{}()+-]*$");

    function Init() {
        ddlSubscriber.multiselect({
            menuWidth: 225,
            menuHeight: 350,
            click: RemoveMessageError,
            checkAll: RemoveMessageError,
            uncheckAll: RemoveMessageError,
            selectedText: "# of # selected",
            noneSelectedText: "Select options"
        }).multiselectfilter({
            width: 150,
            placeholder: ''
        });
       
        GenerateGrid();

        GetSubscribers();
    }

    function GetSubscribers() {
        var url = common.modifyRelativeUrl('/SearchAccount/GetSubscribers');

        $.get(url, function (data) {
            if (data != '') {
                ddlSubscriber.multiselect().empty();
                var subscribers = [];

                for (var i = 0; i < data.length; i++) {
                    ddlSubscriber.append('<option value="' + data[i].SubscriberID + '">' + data[i].SubscriberName + '</option>');
                    subscribers.push(data[i].SubscriberID);
                }

                ddlSubscriber.multiselect('refresh').multiselect("checkAll");

                var keySearch = $(inputAccount).val();
                if (keySearch != '') {
                    SearchSubscriberAccount(keySearch, subscribers);
                }
            }
        });
    }

    function SearchSubscriberAccount(keySearch, subscribers) {
        if (subscribers == null) {
            var subscribers = [];

            $('option:selected', ddlSubscriber).each(function (idx, ele) {
                subscribers.push($(ele).val());
            });
        }

        if (!ValidateInput(keySearch, subscribers)) {
            return;
        }

        common.showLoading();

        var url = common.modifyRelativeUrl('/SearchAccount/SearchSubscriberAccount');

        $.ajax(url, {
            type: 'POST',
            data: {
                key: keySearch.trim(),
                subscribers: ConvertSubscribersToString(subscribers)
            },
            success: function (data) {
                $(content).show();

                BindDataToGrid(data, keySearch.trim());
            },
            error: function (errorMessage) {
                common.closeLoading();
                console.log(errorMessage);
                common.showError('Search Account fail!');
            }
        });
    }

    function ConvertSubscribersToString(subscribers) {
        var length = $('option', ddlSubscriber).length;

        if (subscribers.length == length) {
            return '-1';
        }
        else {
            return subscribers.join(',');
        }
    }

    function GenerateGrid() {
        table.DataTable({            
            searching: false,
            lengthMenu: [50, 100, 200, 500],
            pageLength: 100,
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

    function BindDataToGrid(data, keySearch) {
        if (data != null) {
            var redirect = false;
            if (data.length == 1) {
                if (keySearch == data[0].UserName || keySearch == data[0].UserName2) {
                    redirect = true;
                }
            } 

            if (redirect) {
                window.location.href = common.modifyRelativeUrl('/Account/DetailAccount?id=' + data[0].AccountID
                    + '&custId=' + data[0].CustID
                    + '&subscriber=' + data[0].SubscriberName
                );               
            } else {
                table.DataTable().clear().rows.add(data).draw();
                table.DataTable().columns.adjust();

                common.closeLoading();
            }
        }
    }
    
    function RenderUserName(obj, userName) {
        var url = common.modifyRelativeUrl('/Account/DetailAccount?id=' + obj.AccountID + '&custId=' + obj.CustID + '&subscriber=' + obj.SubscriberName);

        return `<a class='info-bng' 
                    href=${url}
                    target='_blank'>${userName}
                </a>`;
    }

    function ValidateInput(keySearch, subscribers) {
        var error = false;

        if (keySearch == null || keySearch.trim().length < 3 || !regex.test(keySearch)) {
            $('span', captionErrorAccount).text('The Account is not valid!');
            $(captionErrorAccount).show();
            error = true;
        }

        if (subscribers.length == 0) {
            $('span', captionErrorSubscribers).text('Subscribers is required select!');
            $(captionErrorSubscribers).show();
            error = true;
        }

        if (error) {
            $(content).hide();
            return false;
        }

        $(captionErrorAccount).hide();
        $(captionErrorSubscribers).hide();

        return true;
    }

    function RemoveMessageError() {
        $(captionErrorSubscribers).hide();
    }

    inputAccount.keyup((e) => {
        $(captionErrorAccount).hide();        

        if (e.which == 13) {
            SearchSubscriberAccount($(e.target).val());
        }
    });

    $(btnSearch).click(() => {
        SearchSubscriberAccount($(inputAccount).val());
    });

    $(captionErrorAccount).hide();
    $(captionErrorSubscribers).hide();

    Init();
}

$(search_account);