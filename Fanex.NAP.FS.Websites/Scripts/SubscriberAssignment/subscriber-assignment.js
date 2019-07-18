function subscriber_assignment() {
    var inputUserName = $('#username');
    var btnSearch = $('#btnSearch');
    var btnSubmit = $('#btnSubmit');
    var ddlSubscriber = $('#ddlSubscriber');
    var contentSubscribers = $("#content-subscribers");
    var regex = new RegExp("[0-9]");

    function init() {
        $("#ddlSubscriber").multiselect({
            menuWidth: 225,
            menuHeight: 350,            
            click: CheckSubscriberItem,
            checkAll: CheckAllSubscribers,
            uncheckAll: UnCheckAllSubscribers
        }).multiselectfilter({
            width: 140,
            placeholder: 'Enter key words...'
        });
    }

    function GetUserId() {
        var userName = inputUserName.val().trim();

        if (userName.length == 0) {
            return;
        }

        var url = common.modifyRelativeUrl('/SubscriberAssignment/GetUserId');

        $.get(url, { userName: userName }, function (data) {
            if (data == '') {
                inputUserName.next('span').text('The username is not valid').show();

                ClearResult();
            }
            else {
                GetSubscribers(data);
            }            
        });
    }

    function GetSubscribers(userId) {
        var url = common.modifyRelativeUrl('/SubscriberAssignment/GetSubscribers');

        $.get(url, { userId: userId }, function (data) {
            BindSubscribersToControl(data);

            BindSubscribersToHtml(data);
        });
    }

    function BindSubscribersToControl(data) {
        ddlSubscriber.multiselect().empty();

        for (var i = 0; i < data.length; i++) {
            ddlSubscriber.append('<option value="' + data[i].Id + '" selected>' + data[i].Name + '</option>');
        }

        ddlSubscriber.multiselect('refresh');
    }

    function BindSubscribersToHtml(data) {
        var html = '';

        for (var i = 0; i < data.length; i++) {
            html += ToHtmlSubscriberItem(data[i].Name);
        }

        contentSubscribers.html(html);

        $(".group", contentSubscribers).each(GroupSubscribersItem);
    }

    function GroupSubscribersItem() {
        var item = $('#item-subscriber-btn', this);
        var letter = $(item).text().charAt(0);

        if (regex.test(letter)) {
            GroupWithNumber(this, letter);
        }
        else {
            GroupWithCharacter(this, letter);
        }

        $('#item-subscriber-btn', this).click(ClickHandleSubscriberItem);
    }

    function GroupWithNumber(group, letter) {
        var parent = $(group).parent();

        if (!$(parent).find('[data-letter="0-9"]').length) {

            var html = '<div class="" data-letter="0-9">';
            html += '<div><label class="text">0-9</label></div>';
            html += '<div class="form-filter"></div>';
            html += '</div>';

            $(parent).append(html);
        }

        $(parent).find('[data-letter="0-9"]').find('.form-filter').append(group);
    }

    function GroupWithCharacter(group, letter) {
        var parent = $(group).parent();

        if (!$(parent).find('[data-letter="' + letter + '"]').length) {

            var html = '<div class="" data-letter="' + letter + '">';
            html += '<div><label class="text">' + letter + '</label></div>';
            html += '<div class="form-filter"></div>';
            html += '</div>';

            $(parent).append(html);
        }

        $(parent).find('[data-letter="' + letter + '"]').find('.form-filter').append(group);
    }

    function CheckSubscriberItem(event, ui) {
        if (ui.checked) {
            AddItemToGroup(ui.text);
        }
        else {
            RemoveItemFromGroup(ui.text);
        }
    }

    function AddItemToGroup(name) {        
        contentSubscribers.after(ToHtmlSubscriberItem(name));

        $(".group", contentSubscribers).each((index, group) => {
            var letter = $('#item-subscriber-btn', group).text().charAt(0); 

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
            var result = $(group).find('[name="' + name + '"]');

            if (result.length) {
                $(result).remove();
            }
        });
    }

    function CheckAllSubscribers() {
        
    }

    function UnCheckAllSubscribers() {
        ClearResult();
    }

    function ClickHandleSubscriberItem(event) {
        var item = $(event.target);

        var name = $(item).text();

        if ($(item).hasClass('btn-assign')) {
            $(item).removeClass('btn-assign').addClass('.btn-default');
            console.log(name + ' is unassigned');
        }
        else {
            $(item).removeClass('.btn-default').addClass('btn-assign');
            console.log(name + ' is assigned');
        }
    }

    function ToHtmlSubscriberItem(name) {
        var htmlString = "<div class='group width-50' id='" + name + "'>";
        htmlString += "<button class='button btn-default' id='item-subscriber-btn' name='" + name + "'>" + name + "</button>";        
        htmlString += "</div>";
      
        return htmlString;
    }

    function Save() {

    }

    function ClearResult() {
        contentSubscribers.html('');
    }

    inputUserName.keyup(() => {
        inputUserName.next('span').hide();
    });

    btnSearch.click(GetUserId);

    btnSubmit.click(Save);

    init();
}

$(subscriber_assignment);