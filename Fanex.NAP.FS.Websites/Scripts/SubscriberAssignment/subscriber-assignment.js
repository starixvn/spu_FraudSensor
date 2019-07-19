function subscriber_assignment() {
    var inputUserName = $('#username');
    var ddlSubscriber = $('#ddlSubscriber');
    var cbbStatus = $('#cbbStatus');
    var contentSubscribers = $("#content-subscribers");
    var regex = new RegExp("[0-9]");
    var currentUserId = '';
    var subscribersAssigned = [];
    var subscribersUnAssign = [];
    var subscriberHidden = [];

    function init() {
        ddlSubscriber.multiselect({
            menuWidth: 225,
            menuHeight: 350,         
            selectedText: "# of # subscribers selected",
            noneSelectedText: "Select subscribers",
            click: CheckSubscriberItem,
            checkAll: CheckAllSubscribers,
            uncheckAll: UnCheckAllSubscribers,            
        }).multiselectfilter({
            width: 140,
            placeholder: 'Enter key words...'
        });
    }

    function Generate() {
        currentUserId = '';
        subscribersAssigned = [];
        subscribersUnAssign = [];
        subscriberHidden = [];

        var userName = inputUserName.val().trim();

        if (userName.length == 0) {
            return;
        }

        var url = common.modifyRelativeUrl('/SubscriberAssignment/GetUserId');

        $.get(url, { userName: userName }, function (data) {
            if (data == '') {
                inputUserName.next('span').text('The username is not valid').show();

                contentSubscribers.html('');
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

            BindSubscribersToHtml(subscribersAssigned, subscribersUnAssign);
        });

        currentUserId = id;
    }

    function BindSubscribersToControl(data) {
        ddlSubscriber.multiselect().empty();

        for (var i = 0; i < data.length; i++) {
            var subscriber = new Subscriber(data[i].SubscriberID, data[i].SubscriberName, data[i].isAssigned);

            ddlSubscriber.append('<option value="' + subscriber.id + '">' + subscriber.name + '</option>');            

            if (subscriber.isAssigned) {
                subscribersAssigned.push(subscriber);
            }
            else {
                subscribersUnAssign.push(subscriber);
            }
        }
        
        ddlSubscriber.multiselect('refresh').multiselect("checkAll");
    }

    function BindSubscribersToHtml(dataAssigned, dataUnAssign) {
        var html = '';       

        $(dataAssigned).each((index, item) => {
            html += ToHtmlSubscriberItem(item, 'btn-assigned');
        });

        $(dataUnAssign).each((index, item) => {
            html += ToHtmlSubscriberItem(item, 'btn-default');
        });

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
        var value = ui.text.trim();

        if (ui.checked) {
            AddSubscriberToGroup("name", value);
        }
        else {
            RemoveSubscriberFromGroup("name", value);
        }
    }

    function AddSubscriberToGroup(key, value) {
        var subs = FindObjectByKey(subscriberHidden, key, value);

        if (subs != null) {
            subscriberHidden.splice(subscriberHidden.indexOf(subs), 1);

            contentSubscribers.after(ToHtmlSubscriberItem(subs, "btn-default"));

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
    }

    function RemoveSubscriberFromGroup(key, value) {
        var subs = FindObject(subscribersAssigned, subscribersUnAssign, key, value);

        if (subs != null) {
            subscriberHidden.push(subs);

            $(".group", contentSubscribers).each((index, group) => {
                var result = $(group).find('[value="' + subs.id + '"]');

                if (result.length) {
                    $(result).remove();
                }
            });
        }
    }

    function CheckAllSubscribers() {
        contentSubscribers.html('');

        BindSubscribersToHtml(subscribersAssigned, subscribersUnAssign);
    }

    function UnCheckAllSubscribers() {
        contentSubscribers.html('');
    }

    function ClickHandleSubscriberItem(event) {
        var item = $(event.target);
        var value = $(item).val();

        if ($(item).hasClass('btn-assigned')) {
            var subs = FindObjectByKey(subscribersAssigned, "id", value);
            subscribersAssigned.splice(subscribersAssigned.indexOf(subs), 1);
            subscribersUnAssign.push(subs);         

            $(item).removeClass('btn-assigned').addClass('.btn-default');
        }
        else {          
            var subs = FindObjectByKey(subscribersUnAssign, "id", value);
            subscribersUnAssign.splice(subscribersUnAssign.indexOf(subs), 1);
            subscribersAssigned.push(subs);

            $(item).removeClass('.btn-default').addClass('btn-assigned');
        }
    }

    function CheckHandleIsMaster(event) {
        var disabled = $(event.target).is(':checked');

        $(ddlSubscriber).attr("disabled", disabled);
        $(cbbStatus).attr("disabled", disabled);

        $(".group", contentSubscribers).each((index, group) => {            
            var items = $(group).find('#item-subscriber-btn');

            $(items).each((index, item) => {
                if (disabled) {
                    $(item).addClass('disabled');
                } else {
                    $(item).removeClass('disabled');
                }
            });
        });
    }
   
    function ChangeStatus(event) {
        contentSubscribers.html('');
        var value = $(event.target).val();
       
        if (value == 1) {            
            BindSubscribersToHtml(subscribersAssigned, []);

        } else if (value == 2) {            
            BindSubscribersToHtml([], subscribersUnAssign);
        }
        else {
            BindSubscribersToHtml(subscribersAssigned, subscribersUnAssign);
        }
    }

    function ToHtmlSubscriberItem(item, btnClassName) {
        var htmlString = "<div class='group width-50' id='" + item.name + "'>";
        htmlString += "<button class='button " + btnClassName + "' id='item-subscriber-btn' value='" + item.id + "'>" + item.name + "</button>";        
        htmlString += "</div>";
      
        return htmlString;
    }

    function FindObject(array1, array2, key, value) {
        var object = FindObjectByKey(array1, key, value);

        if (object == null) {
            object = FindObjectByKey(array2, key, value);
        }

        return object;
    }

    function FindObjectByKey(array, key, value) {
        for (var i = 0; i < array.length; i++) {
            var object = array[i];

            if (object[key] == value) {
                return object;
            }
        }

        return null;
    }

    function JoinIds(array) {
        var ids = [];
        for (var i = 0; i < array.length; i++) {
            var object = array[i];
            ids.push(object.id);
        }

        return ids.join(',');
    }

    function Save() {
        var subscriberIds = JoinIds(subscribersAssigned);
        var isMasterSub = $('#chkMasterSubcriber').is(':checked');

        var url = common.modifyRelativeUrl('/SubscriberAssignment/AssignSubscribers');

        $.ajax(url, {
            type: 'POST',
            data: {
                userId: currentUserId,
                subscriberIds: subscriberIds,
                isMasterSub: isMasterSub
            },
            success: function (response) {
                common.showSuccess('Assigned subscribers!');
            },
            error: function (errorMessage) {
                console.log(errorMessage);
                common.showError('Assign subscribers fail!');
            }
        });
    }   

    inputUserName.keyup(function(e) {
        inputUserName.next('span').hide();

        if (e.which == 13) {
            Generate();
        }
    });

    $('#chkMasterSubcriber').change(CheckHandleIsMaster);

    $('#btnSearch').click(Generate);

    $('#btnSubmit').click(Save);

    $('#cbbStatus').change(ChangeStatus);

    init();
}

$(subscriber_assignment);