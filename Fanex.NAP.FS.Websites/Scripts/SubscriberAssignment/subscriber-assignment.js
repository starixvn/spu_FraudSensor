function subscriber_assignment() {
    var inputUserName = $('#username');
    var ddlSubscriber = $('#ddlSubscriber');
    var contentSubscribers = $("#content-subscribers");
    var regex = new RegExp("[0-9]");
    var currentUserId = '';
    var subscribersAssigned = [];

    function init() {
        ddlSubscriber.multiselect({
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
        currentUserId = '';
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

    function GetSubscribers(id) {
        
        var url = common.modifyRelativeUrl('/SubscriberAssignment/GetSubscribers');

        $.get(url, { userId: id }, function (data) {            
            BindSubscribersToControl(data);

            BindSubscribersToHtml(data);
        });

        currentUserId = id;
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
        var letter = $('#item-subscriber-btn', this).text().charAt(0);

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
            if (subscribersAssigned.indexOf(name) >= 0) {
                subscribersAssigned.remove(name);
            }

            $(item).removeClass('btn-assign').addClass('.btn-default');
        }
        else {
            if (subscribersAssigned.indexOf(name) < 0) {
                subscribersAssigned.push(name);
            }
            
            $(item).removeClass('.btn-default').addClass('btn-assign');
        }
    }

    function CheckHandleIsMaster(event) {
        $(".group", contentSubscribers).each((index, group) => {            
            var items = $(group).find('#item-subscriber-btn');

            $(items).each((index, item) => {         
                
                if ($(event.target).is(':checked')) {
                    $(item).addClass('disabled');
                } else {
                    $(item).removeClass('disabled');
                }
            });
        });
    }

    function ToHtmlSubscriberItem(name) {
        var htmlString = "<div class='group width-50' id='" + name + "'>";
        htmlString += "<button class='button btn-default' id='item-subscriber-btn' name='" + name + "'>" + name + "</button>";        
        htmlString += "</div>";
      
        return htmlString;
    }

    function Save() {
        var subscribers = subscribersAssigned.join(';');
        var url = common.modifyRelativeUrl('/SubscriberAssignment/AssignSubscribers');

        $.ajax(url, {
            type: 'POST',
            data: { userId: currentUserId, subscribers: subscribers },
            success: function (response) {
                common.showSuccess('Assigned subscribers!');
            },
            error: function (errorMessage) {
                console.log(errorMessage);
                common.showError('Assign subscribers fail!');
            }
        });
    }

    function ClearResult() {
        contentSubscribers.html('');
    }

    inputUserName.keyup(() => {
        inputUserName.next('span').hide();
    });

    $('#chkMasterSubcriber').change(CheckHandleIsMaster);

    $('#btnSearch').click(GetUserId);

    $('#btnSubmit').click(Save);

    init();
}

$(subscriber_assignment);