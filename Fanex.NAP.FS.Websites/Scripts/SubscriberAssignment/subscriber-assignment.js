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

    function Init() {
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

    function GenerateContent() {
        ClearResult();

        var userName = inputUserName.val().trim();
        var specialCharacter = ContainSpecialCharacter(userName);

        if (userName.length == 0) {
            HiddenFilter();
            alert("Please, input the Username!"); return;            
        }

        if (ContainSpecialCharacter(userName)) {
            HiddenFilter();
            alert("There is a special character in the Username input. Please correct!"); return;            
        }

        var url = common.modifyRelativeUrl('/SubscriberAssignment/GetUserId');

        $.get(url, { userName: userName }, function (data) {
            if (data == '') {
                inputUserName.next('span').text('The username is not valid').show();

                contentSubscribers.html('');
                HiddenFilter();
            }
            else {
                GetSubscribers(data);
            }
        });
    }

    function GetSubscribers(id) {        
        var url = common.modifyRelativeUrl('/SubscriberAssignment/GetSubscribers');

        $.get(url, { userId: id }, function (data) {
            DisplayFilter();

            BindSubscribersToControl(data);

            BindSubscribersContentToHtml(subscribersAssigned, subscribersUnAssign);
        });
        
        currentUserId = id;
    }    

    function BindSubscribersToControl(data) {
        ddlSubscriber.multiselect().empty();
        var isMaster = false;

        for (var i = 0; i < data.length; i++) {

            if (data[i].SubscriberID == 0 && data[i].isAssigned == 1) {
                isMaster = true;
                continue;
            }

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

        if (isMaster) {
            $('#chkMasterSubcriber').prop("checked", true);
            HandleIsMaster(isMaster);
        }
    }

    function BindSubscribersContentToHtml(dataAssigned, dataUnAssign) {
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
        var parent = $(this).parent();

        if (regex.test(letter)) {           
            AppendSubscriberToHtml("0-9", this, parent);
        }
        else {            
            AppendSubscriberToHtml(letter.toUpperCase(), this, parent);
        }

        $('#item-subscriber-btn', this).click(ClickHandleSubscriberItem);
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

            subscribersUnAssign.push(subs);
            subscribersUnAssign.sort(Compare);

            contentSubscribers.html('');

            BindSubscribersContentToHtml(subscribersAssigned, subscribersUnAssign);
        }
    }

    function AppendSubscriberToHtml(letter, group, parent) {
        if (!$(parent).length) {
            parent = contentSubscribers;
        }

        if (!$(parent).find('[data-letter="' + letter + '"]').length) {

            var html = '<div class="margin-top-10" data-letter="' + letter + '">';
            html += '<div><label class="text letter">' + letter + '</label></div>';
            html += '<div class="form-filter"></div>';
            html += '</div>';

            $(parent).append(html);
        }

        $(parent).find('[data-letter="' + letter + '"]').find('.form-filter').append(group);
    }

    function RemoveSubscriberFromGroup(key, value) {
        var subs = FindObject(subscribersAssigned, subscribersUnAssign, key, value);

        if (subs != null) {
            subscriberHidden.push(subs);

            $(".group", contentSubscribers).each((index, group) => {
                var result = $(group).find('[value="' + subs.id + '"]');

                if (result.length) {
                    $(result).parent().remove();
                }
            });
        }
    }

    function CheckAllSubscribers() {
        contentSubscribers.html('');
        subscribersUnAssign = subscribersUnAssign.concat(subscriberHidden);
        subscriberHidden = [];

        BindSubscribersContentToHtml(subscribersAssigned, subscribersUnAssign);
    }

    function UnCheckAllSubscribers() {
        subscriberHidden = subscriberHidden.concat(subscribersAssigned, subscribersUnAssign);
        subscribersAssigned = [];
        subscribersUnAssign = [];
        contentSubscribers.html('');
    }

    function ClickHandleSubscriberItem(event) {
        if ($('#chkMasterSubcriber').is(':checked')) {
            return;
        }

        var item = $(event.target);

        if ($(item).hasClass('btn-assigned')) {
            var subs = FindObjectByKey(subscribersAssigned, "id", $(item).val());
            subscribersAssigned.splice(subscribersAssigned.indexOf(subs), 1);
            subscribersUnAssign.push(subs);

            $(item).removeClass('btn-assigned').addClass('btn-default');
        }
        else {
            var subs = FindObjectByKey(subscribersUnAssign, "id", $(item).val());
            subscribersUnAssign.splice(subscribersUnAssign.indexOf(subs), 1);
            subscribersAssigned.push(subs);

            $(item).removeClass('.btn-default').addClass('btn-assigned');
        }
    }

    function CheckHandleIsMaster(event) {
        var checked = $(event.target).is(':checked');

        HandleIsMaster(checked);
    }

    function HandleIsMaster(checked) {
        $(ddlSubscriber).multiselect(checked ? 'disable' : 'enable');
        $(cbbStatus).attr("disabled", checked);

        $(".group", contentSubscribers).each((index, group) => {
            var items = $(group).find('#item-subscriber-btn');

            $(items).each((index, item) => {
                if (checked) {
                    $(item).addClass('disabled').addClass('btn-default').removeClass('btn-assigned');

                    var subs = FindObjectByKey(subscribersAssigned, "id", $(item).val());
                    if (subs != null) {
                        subscribersAssigned.splice(subscribersAssigned.indexOf(subs), 1);
                        subscribersUnAssign.push(subs);
                    }
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
            BindSubscribersContentToHtml(subscribersAssigned, []);

        } else if (value == 2) {            
            BindSubscribersContentToHtml([], subscribersUnAssign);
        }
        else {
            BindSubscribersContentToHtml(subscribersAssigned, subscribersUnAssign);
        }
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

    function Compare(subsA, subsB) {
        if (subsA.name < subsB.name) {
            return -1;
        }
        if (subsA.name > subsB.name) {
            return 1;
        }
        return 0;
    }

    function ContainSpecialCharacter(obj) {
        if (obj != null && obj.length > 0) {
            var regex = new RegExp("^[a-zA-Z0-9$ ]*$");
            if (!regex.test(obj)) {

                return true;
            }
        }

        return false;
    }

    function ToHtmlSubscriberItem(item, btnClassName) {
        var htmlString = "<div class='group group-subscriber' id='" + item.name + "'>";
        htmlString += "<button class='button " + btnClassName + "' id='item-subscriber-btn' value='" + item.id + "'>" + item.name + "</button>";
        htmlString += "</div>";

        return htmlString;
    }

    function SaveSubscribers() {  
        if (currentUserId == '') {
            return;
        }

        var url = common.modifyRelativeUrl('/SubscriberAssignment/AssignSubscribers');
        var isMasterSub = $('#chkMasterSubcriber').is(':checked');
        var subscriberIds = isMasterSub ? '' : JoinIds(subscribersAssigned);        

        $.ajax(url, {
            type: 'POST',
            data: {
                userId: currentUserId,
                subscriberIds: subscriberIds,
                isMasterSub: isMasterSub
            },
            success: function (response) {
                if (response.ErrorCode == 0) {  
                    common.showSuccess('Assigned subscribers!');
                }
                else {
                    common.showError(response.ErrorMsg);
                }
            },
            error: function (errorMessage) {
                console.log(errorMessage);
                common.showError('Assign subscribers fail!');
            }
        });
    }   

    function ClearResult() {
        $('#chkMasterSubcriber').prop('checked', false);
        $('#cbbStatus').val('0');
        currentUserId = '';
        contentSubscribers.html('');
        subscribersAssigned = [];
        subscribersUnAssign = [];
        subscriberHidden = [];
    }

    function DisplayFilter() {
        $('#filter-ismaster').show();
        $('#filter-subs').show();
    }

    function HiddenFilter() {
        $('#filter-ismaster').hide();
        $('#filter-subs').hide();
    }

    inputUserName.keyup(function(e) {
        inputUserName.next('span').hide();

        if (e.which == 13) {
            GenerateContent();
        }
    });

    $('#chkMasterSubcriber').change(CheckHandleIsMaster);

    $('#btnSearch').click(GenerateContent);

    $('#btnSubmit').click(SaveSubscribers);

    $('#cbbStatus').change(ChangeStatus);

    HiddenFilter();

    Init();
}

$(subscriber_assignment);