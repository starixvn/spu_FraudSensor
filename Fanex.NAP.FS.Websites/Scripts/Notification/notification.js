var showNotification = false;
var updateStatus = false;
var unreadMsgCounter = 0;

function notification() {
    var top20Messages = null;
    
    String.prototype.format = function () {
        var a = this;
        for (var k in arguments) {
            a = a.replace(new RegExp("\\{" + k + "\\}", 'g'), arguments[k]);
        }
        return a
    }

    function getMessage()
    {
        var url = common.modifyRelativeUrl("/Notification/GetAllMessages");

        $.get(url, function (json) {
            top20Messages = json;
        });

        $('#noti-unread-msg').hide();
        $('#notificationContainer').show();

        var messagesHtmlString = "";
        var messagesCount = top20Messages.length;
        var htmlTemplateRead = "<div class='notification'><div class='notification-header'><div class='notification-title'>{0}</div> <div class='notification-date'>{1}</div></div><div class='notification-content'>{2}</div></div>";
        var htmlTemplateUnRead = "<div class='notification -unread'><div class='notification-header'><div class='notification-title'>{0}</div> <div class='notification-date'>{1}</div></div><div class='notification-content'>{2}</div></div>";
        for (var i = 0; i < messagesCount; i++) {
            var message = top20Messages[i];
            if (message.IsViewed == true) {
                messagesHtmlString += htmlTemplateRead.format(message.FraudCaseName, message.LogDate, message.MessageContent);
            }
            else {
                messagesHtmlString += htmlTemplateUnRead.format(message.FraudCaseName, message.LogDate, message.MessageContent);
            }
        }

        $('#notificationContainer').html(messagesCount > 0 ? messagesHtmlString : "<div class='notification'>There is no message.</div>");
    }

    function getUnreadMessageCount()
    {
        var url = common.modifyRelativeUrl("/Notification/ReCheckNotification");

        $.get(url, function (json) {
            unreadMsgCounter = json.UnreadMessageCounter;
        });

        if (unreadMsgCounter <= 0) {
            $('#noti-unread-msg').hide();
        }
        else {
            $('#noti-unread-msg').show();
            $('#noti-unread-msg').html(unreadMsgCounter);
        }
    }

    $('#notification-icon').off("click");
    $('#notification-icon').on("click", function (event) {
        $('#noti-unread-msg').hide();
        if (!$('#notificationContainer').is(':visible')) {
            updateStatus = true;
            getMessage();
        }
        else {
            $('#notificationContainer').hide();
        }
    });

    $(document).mouseup(function (e) {
        if (e.target.id != $("#notificationContainer").attr('id') && !$("#notificationContainer").has(e.target).length &&
            e.target.id != $("#notification-icon").attr('id') && !$("#notification-icon").has(e.target).length) {
            $("#notificationContainer").hide();
        }
    });

    getUnreadMessageCount();
    setInterval(getUnreadMessageCount, 3000);
}

$(notification);
