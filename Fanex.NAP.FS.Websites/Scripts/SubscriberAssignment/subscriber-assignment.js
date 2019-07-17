function subscriber_assignment() {
    var btnSearch = $('#btnSearch');
    var btnSubmit = $('#btnSubmit');
    var ddlSubscriber = $('#ddlSubscriber');
    var contentSubscribers = $("#content-subscribers");
    var regex = new RegExp("[0-9]");

    function init() {
        $("#ddlSubscriber").multiselect({
            menuWidth: 225,
            menuHeight: 350,          
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

            BindSubscribersToControl(data);

            BindSubscribersToHtml(data);

        });
    }

    function BindSubscribersToControl(data) {
        ddlSubscriber.multiselect().empty();

        for (var i = 0; i < data.length; i++) {
            ddlSubscriber.append(`<option value='${data[i].Id}' selected='selected'>${data[i].Name}</option>`);
        }
        
        ddlSubscriber.multiselect('refresh');
    }

    function BindSubscribersToHtml(data) {
        var html = '';

        for (var i = 0; i < data.length; i++) {
            html += SubscriberToHtml(data[i].Name);
        }

        contentSubscribers.html(html);

        $(".group", contentSubscribers).each(GroupItemSubscribers);
    }

    function GroupItemSubscribers() {        

        var letter = $('#item-subscriber', this).val().charAt(0); 

        if (regex.test(letter)) {
            GroupWithNumber(this, letter);
        }
        else {
            GroupWithCharacter(this, letter);
        }
    }

    function GroupWithNumber(group, letter) {
        var parent = $(group).parent();

        if (!$(parent).find('[data-letter="0-9"]').length) {
            $(parent).append('<div class="form-filter" data-letter="0-9"><div><label class="text">0-9</label></div><br /></div>');
        }

        $(parent).find('[data-letter="0-9"]').append(group);
    }

    function GroupWithCharacter(group, letter) {
        var parent = $(group).parent();

        if (!$(parent).find('[data-letter="' + letter + '"]').length) {
            $(parent).append('<div class="form-filter" data-letter="' + letter + '"><label class="text">' + letter + '</label><br /></div>');
        }

        $(parent).find('[data-letter="' + letter + '"]').append(group);
    }

    function ClickEffectSubscribers(event, ui) {
        if (ui.checked) {
            AddItemToGroup(ui.text);
        }
        else {
            RemoveItemFromGroup(ui.text);
        }
    }

    function AddItemToGroup(name) {        
        contentSubscribers.after(SubscriberToHtml(name));

        $(".group", contentSubscribers).each((index, group) => {
            var letter = $('#item-subscriber', group).val().charAt(0); 

            if (regex.test(letter)) {
                GroupWithNumber($(group), letter);
            }
            else {
                GroupWithCharacter($(group), letter);
            }
        });
    }

    function RemoveItemFromGroup(name) {
        $(".group", contentSubscribers).each((index, group) => {
            var result = $(group).find('[value="' + name + '"]');

            if (result.length) {
                $(result).remove();
            }
        });
    }

    function CheckAllEffectSubscribers() {
       
    }

    function UnCheckAllEffectSubscribers() {
        
    }

    function SubscriberToHtml(name) {
        var htmlString = "<div class='group width-100' id='" + name + "'>";
        htmlString += "<button class='button - primary' id='item-subscriber' value='" + name + "'>" + name + "</button>";
        htmlString += "</div>";

        return htmlString;
    }

    function Save() {

    }

    btnSearch.click(GetSubscribers);

    btnSubmit.click(Save);

    init();
}

$(subscriber_assignment);