function notification() {
    var notificationIcon = $("#notificationIcon"),
        messageList = $("#messageList");

    var top20Messages = null;
    var unreadMsgCounter = 0;

    function getUnreadMessageCounter() {
        var url = common.modifyRelativeUrl("/Notification/ReCheckNotification");

        $.get(url, function (json) {
            unreadMsgCounter = json.unreadMessageCounter;
        });
    }

    function getTop20Messages(target, fraudCase) {
        var url = common.modifyRelativeUrl("/Notification/GetAllMessages");

        $.get(url, function (json) {
            top20Messages = json;
        });
    }

    function renderUnreadMessageCounter()
    {
        if (unreadMsgCounter <= 0) {
            $('#noti-unread-msg').hide();
        }
        else {
            $('#noti-unread-msg').show();
            $('#noti-unread-msg').html(unreadMsgCounter);
        }
    }

    function renderMessage() {
        $('#noti-unread-msg').hide();

        var messagesHtmlString = "";
        var messagesCount = top20Messages.length;
        var htmlTemplateRead = "<div class='notify-text-wrap notify-text-wrap-link' style='width: 100%; cursor: pointer;'>{0}</div><div class='notify-text-wrap notify-text-wrap-link' style='width: 100%; cursor: pointer;'>{1}</div><div class='notify-text-wrap notify-text-time' style='width: 100%'>{2}</div><hr class='style-two'>";
        var htmlTemplateUnRead = "<div class='notify-text-wrap notify-text-bold notify-text-wrap-link' style='width: 100%; cursor: pointer;'>{0}</div><div class='notify-text-wrap notify-text-bold notify-text-wrap-link' style='width: 100%; cursor: pointer;'>{1}</div><div class='notify-text-wrap notify-text-time' style='width: 100%'>{2}</div><hr class='style-two'>";
        for (var i = 0; i < messagesCount; i++) {
            var message = top20Messages[i];
            if (message.IsViewed == true) {
                messagesHtmlString += htmlTemplateRead.format(message.FraudCaseName, message.MessageContent, message.LogDate);
            }
            else {
                messagesHtmlString += htmlTemplateUnRead.format(message.FraudCaseName, message.MessageContent, message.LogDate);
            }
        }

        $('#notificationsBody').html(messagesCount > 0 ? messagesHtmlString : "<div class='notify-text-wrap notify-text-nomsg' style='width: 100%'>There is no message.</div>");
    }
}

$(notification);