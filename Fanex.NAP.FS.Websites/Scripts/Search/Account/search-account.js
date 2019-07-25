function search_account() {
    var ddlSubscriber = $('#ddlSubscriber');
    var searchContent = $("#search-content");

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

    Init();
}

$(search_account);