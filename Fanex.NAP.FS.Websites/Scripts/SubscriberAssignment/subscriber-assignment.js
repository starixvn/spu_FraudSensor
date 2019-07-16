function subscriber_assignment() {
    var btnSearch = $('#btnSearch');
    var btnSubmit = $('#btnSubmit');
    var ddlSubscriber = $('#ddlSubscriber');
    var contentSubscribers = $("#content-subscribers");

    function init() {
        $("#ddlSubscriber").multiselect({
            menuWidth: 225,
            menuHeight: 350,
            selectAll: true,
            click: ClickEffectSubscribers,
            checkAll: CheckAllEffectSubscribers,
            uncheckAll: UnCheckAllEffectSubscribers
        }).multiselectfilter({
            width: 140,
            placeholder: 'Enter key words...'
        });
    }    

    function GetSubscribers() {
        var userId = $("#username").val().trim();

        if (userId.length == 0) {
            return;
        }

        var url = common.modifyRelativeUrl('/SubscriberAssignment/GetSubscribers');

        $.get(url, { userId: userId }, function (data) {

            BindControlSubscribers(data);

            DrawContentSubscribers(data);

        });
    }

    function BindControlSubscribers(data) {
        for (var i = 0; i < data.length; i++) {
            ddlSubscriber.append(`<option value='${data[i].Id}'>${data[i].Name}</option>`);
        }

        ddlSubscriber.multiselect('refresh');
    }

    function DrawContentSubscribers(data) {
        var content = '';

        for (var i = 0; i < data.length; i++) {
            content += (`<input type='button' value='${data[i].Name} />`);
        }

        contentSubscribers.after(content);
    }

    function ClickEffectSubscribers(event, ui) {
        var clickedOption = ui.value + ' ' + (ui.checked ? 'checked' : 'unchecked');

        console.log(clickedOption);

        contentSubscribers.after(clickedOption);
    }

    function CheckAllEffectSubscribers() {
        var clickedOption = "Click All";    
        ddlSubscriber.after(clickedOption);
    }

    function UnCheckAllEffectSubscribers() {
        var clickedOption = "Unclick All";
        ddlSubscriber.after(clickedOption);
    }

    function Save() {

    }

    btnSearch.click(GetSubscribers);

    btnSubmit.click(Save);

    init();
}

$(subscriber_assignment);