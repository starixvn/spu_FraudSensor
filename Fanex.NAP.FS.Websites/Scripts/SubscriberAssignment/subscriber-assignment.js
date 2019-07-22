function subscriber_assignment() {
    var inputUserName = $('#username');
    var captionError = $('#caption-error');
    var ddlSubscriber = $('#ddlSubscriber');
    var cbbStatus = $('#cbbStatus');
    var contentSubscribers = $("#content-subscribers");
    var regex = new RegExp("[0-9]");
    var currentUserId = '';
    var isMaster = false;
    var subscribersAssigned = [];
    var subscribersUnAssign = [];
    var subscribersAssignedHidden = [];
    var subscribersUnAssignHidden = [];

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
            $('span', captionError).text('Please, input the Username!');
            $(captionError).show();

            return;
        }

        if (ContainSpecialCharacter(userName)) {
            HiddenFilter();                        
            $('span', captionError).text('There is a special character in the Username input. Please correct!');
            $(captionError).show();
            
            return;            
        }

        var url = common.modifyRelativeUrl('/SubscriberAssignment/GetUserId');

        $.get(url, { userName: userName }, function (data) {
            if (data == '') {
                $('span', captionError).text('The Username is not valid!');
                $(captionError).show();

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

            if (isMaster) {
                $('#chkMasterSubcriber').prop("checked", true);
            }
            
            HandleIsMaster(isMaster);
        });
        
        currentUserId = id;
    }    

    function BindSubscribersToControl(data) {
        ddlSubscriber.multiselect().empty();
        isMaster = false;

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
    }

    function BindSubscribersContentToHtml(dataAssigned, dataUnAssign) {
        dataAssigned = dataAssigned.concat(dataUnAssign);
        dataAssigned.sort(Compare);

        var html = '';

        $(dataAssigned).each((index, item) => {
            html += ToHtmlSubscriberItem(item);
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
        var subs = FindObjectByKey(subscribersAssignedHidden, key, value);
        
        if (subs != null) {
            subscribersAssignedHidden.splice(subscribersAssignedHidden.indexOf(subs), 1);
            subscribersAssigned.push(subs);
            subscribersAssigned.sort(Compare);
        }

        subs = FindObjectByKey(subscribersUnAssignHidden, key, value);

        if (subs != null) {
            subscribersUnAssignHidden.splice(subscribersUnAssignHidden.indexOf(subs), 1);
            subscribersUnAssign.push(subs);
            subscribersUnAssign.sort(Compare);
        }

        contentSubscribers.html('');

        BindSubscribersContentToHtml(subscribersAssigned, subscribersUnAssign);
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
        var subs = FindObjectByKey(subscribersAssigned, key, value);

        if (subs != null) {
            subscribersAssignedHidden.push(subs);
            subscribersAssigned.splice(subscribersAssigned.indexOf(subs), 1);
        }
        else {
            subs = FindObjectByKey(subscribersUnAssign, key, value);

            if (subs != null) {
                subscribersUnAssignHidden.push(subs);
                subscribersUnAssign.splice(subscribersUnAssign.indexOf(subs), 1);
            }
        }

        if (subs != null) {
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

        subscribersAssigned = subscribersAssigned.concat(subscribersAssignedHidden);
        subscribersUnAssign = subscribersUnAssign.concat(subscribersUnAssignHidden);

        subscribersAssignedHidden = [];
        subscribersUnAssignHidden = [];

        BindSubscribersContentToHtml(subscribersAssigned, subscribersUnAssign);
    }

    function UnCheckAllSubscribers() {
        subscribersAssignedHidden = subscribersAssignedHidden.concat(subscribersAssigned);
        subscribersUnAssignHidden = subscribersUnAssignHidden.concat(subscribersUnAssign);

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
            subs.SetAssign(false);

            subscribersAssigned.splice(subscribersAssigned.indexOf(subs), 1);
            subscribersUnAssign.push(subs);

            $(item).removeClass('btn-assigned').addClass('btn-default');
        }
        else {
            var subs = FindObjectByKey(subscribersUnAssign, "id", $(item).val());
            subs.SetAssign(true);

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
            BindSubscribersContentToHtml(subscribersAssigned, []);

        } else if (value == 2) {            
            BindSubscribersContentToHtml([], subscribersUnAssign);
        }
        else {
            BindSubscribersContentToHtml(subscribersAssigned, subscribersUnAssign);
        }
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

    function ToHtmlSubscriberItem(item) {
        var className = item.isAssigned ? 'btn-assigned' : 'btn-default';

        var htmlString = "<div class='group group-subscriber' id='" + item.name + "'>";
        htmlString += "<button class='button " + className + "' id='item-subscriber-btn' value='" + item.id + "'>" + item.name + "</button>";
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
        subscribersUnAssignHidden = [];
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
        $('span', captionError).text('Please, input the Username!');
        $(captionError).hide();

        if (e.which == 13) {
            GenerateContent();
        }
    });

    $(captionError).hide();

    $('#chkMasterSubcriber').change(CheckHandleIsMaster);

    $('#btnSearch').click(GenerateContent);

    $('#btnSubmit').click(SaveSubscribers);

    $('#cbbStatus').change(ChangeStatus);

    HiddenFilter();

    Init();
}

$(subscriber_assignment);