function subscriber_assignment() {
    var inputUserName = $('#username');
    var captionError = $('#caption-error');
    var ddlSubscriber = $('#ddlSubscriber');
    var cbbStatus = $('#cbbStatus');
    var contentSubscribers = $("#content-subscribers");
    var regex = new RegExp("[0-9]");
    var currentUserId = '';
    var isMaster = false;
    var subscribersOriginal = [];   

    function Init() {
        ddlSubscriber.multiselect({
            menuWidth: 225,
            menuHeight: 350,         
            selectedText: "# of # selected",
            noneSelectedText: "Select options",
            click: CheckSubscriberItem,
            checkAll: EffectAllSubscribers,
            uncheckAll: EffectAllSubscribers
        }).multiselectfilter({
            width: 150,
            placeholder: ''
        });
    }

    function GenerateContent() {
        ClearResult();        
        var userName = inputUserName.val().trim();
        var specialCharacter = ContainSpecialCharacter(userName);

        if (userName.length == 0) {
            HiddenFilter();
            $('span', captionError).text('The Username is not valid!');
            $(captionError).show();

            return;
        }

        if (ContainSpecialCharacter(userName)) {
            HiddenFilter();                        
            $('span', captionError).text('The Username is not valid!');
            $(captionError).show();
            
            return;            
        }      

        var url = common.modifyRelativeUrl('/SubscriberAssignment/GetUserId');

        $.get(url, { userName: userName }, function (data) {
            if (data == '') {
                $('span', captionError).text('The Username is not valid!');
                $(captionError).show();
                
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

            BindSubscribersContentToHtml(subscribersOriginal);

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
        subscribersOriginal = [];

        for (var i = 0; i < data.length; i++) {

            if (data[i].SubscriberID == 0) {
                if (data[i].isAssigned == 1) {
                    isMaster = true;
                }
                continue;
            }
            
            var subscriber = new Subscriber(data[i].SubscriberID, data[i].SubscriberName, data[i].isAssigned);
            
            ddlSubscriber.append('<option value="' + subscriber.id + '">' + subscriber.name + '</option>');

            subscribersOriginal.push(subscriber);
        }
        
        ddlSubscriber.multiselect('refresh').multiselect("checkAll");
    }

    function BindSubscribersContentToHtml(data) {        
        data.sort(Compare);
     
        var html = '';

        $(data).each((index, item) => {
            html += ToHtmlSubscriberItem(item);
        });
        
        $(contentSubscribers).html(html);

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

    function CheckSubscriberItem(event, ui) {
        var value = ui.text.trim();
        var subs = FindObjectByKey(subscribersOriginal, "name", value);

        if (ui.checked) {
            EffectAllSubscribers(subs, "add");
        }
        else {
            EffectAllSubscribers(subs);
        }
    } 

    function EffectAllSubscribers(subsCurrent, action) {
        $(contentSubscribers).empty();
        var dataRefresh = [];        
        
        $('option:selected', ddlSubscriber).each(function (idx, ele) {
            var subs = FindObjectByKey(subscribersOriginal, "id", $(ele).val());

            if (subs != null && CompareWithStatus(subs)) {
                dataRefresh.push(subs);                
            }
        });

        if (subsCurrent instanceof Subscriber) {
            if (action == "add") {
                dataRefresh.push(subsCurrent);
            }
            else {
                dataRefresh.splice(dataRefresh.indexOf(subsCurrent), 1);
            }
        }

        BindSubscribersContentToHtml(dataRefresh);
    }

    function ClickHandleSubscriberItem(event) {
        if ($('#chkMasterSubcriber').is(':checked')) {
            return;
        }

        var item = $(event.target);
        var subs = FindObjectByKey(subscribersOriginal, "id", $(item).val());

        if ($(item).hasClass('btn-assigned')) {
            subs.SetAssign(false);
            $(item).removeClass('btn-assigned').addClass('btn-default');
        }
        else {
            subs.SetAssign(true);
            $(item).removeClass('.btn-default').addClass('btn-assigned');
        }

        subscribersOriginal.splice(subscribersOriginal.indexOf(subs), 1);
        subscribersOriginal.push(subs);
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

    function GetAssgined(data) {
        var dataResult = [];

        for (var i = 0; i < data.length; i++) {
            var subs = data[i];

            if (subs.isAssigned) {
                dataResult.push(subs);
            }
        }

        return dataResult;
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
        var subscriberIds = isMasterSub ? '' : JoinIds(GetAssgined(subscribersOriginal));

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

                    GenerateContent();
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
        $(contentSubscribers).empty();        

        $('span', captionError).text('');
        $(captionError).hide();
    }

    function DisplayFilter() {
        $('#filter-ismaster').show();
        $('#filter-subs').show();
    }

    function HiddenFilter() {
        $('#filter-ismaster').hide();
        $('#filter-subs').hide();
    }

    function CompareWithStatus(subs) {
        var status = $('#cbbStatus').val();
 
        if (status == '0') {
            return true;
        }
        else if ((status == '1' && subs.isAssigned)
            || (status == '2' && !subs.isAssigned)) {
            return true;
        }

        return false;
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

    $('#cbbStatus').change(EffectAllSubscribers);

    HiddenFilter();

    Init();
}

$(subscriber_assignment);