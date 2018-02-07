/*exported ReCheckin*/
"use strict";

function ReCheckin() {
    var urlRecheck = window.rootUrl + "UserLogin/Login/ReCheckLogin";
    var urlLogout = window.rootUrl + "UserLogin/Login/LogOut";

    $.ajax({
        url: urlRecheck,
        type: "POST",
        dataType: "json",
        success: function (result) {
            if (result.IsKickedOut === "1") {
                //alert('You have logged in from another session or your login session has expired!');
                window.location.href = urlLogout;
            }
        },
        error: function () {
        }
    });
}

$(document).ready(function () {
    ReCheckin();
    setInterval(ReCheckin, 10000);
});