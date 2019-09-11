function form_ip_information() {
    var form = $('#form-ip-information');

    this.OpenModal = function (ip, targetElement) {
        GetIPInformation(ip);     
       
        if (form.is('dialog')) {
            form.dialog('open');
        } else {
            form.dialog({
                modal: true,
                width: '550px',
                position: {
                    my: "left top+10",
                    at: "left top+10",
                    of: targetElement,
                    collision: 'flip flip',
                    using: FlipPopup
                },
                title: 'IP INFORMATION',
                resizable: true,
                draggable: true,
                close: function () { $(this).dialog('close'); }
            })
        }
    }

    function GetIPInformation(ip) {
        var url = common.modifyRelativeUrl('/Transaction/GetIPInformation');

        $.ajax(url, {
            type: 'POST',
            data: {
                ip: ip               
            },
            success: function (response) {
                form.html(response);
                form.show();
            },
            error: function (errorMessage) {
                console.log(errorMessage);
                common.showError('Get IP Information fail!');
            }
        });
    }

    function FlipPopup(obj, info) {
        if (info.vertical != "top") {
            $(this).addClass("flipped");
        } else {
            $(this).removeClass("flipped");
        }

        if (info.horizontal != "left") {
            $(this).addClass("flipped");
        } else {
            $(this).removeClass("flipped");
        }

        $(this).css({
            left: obj.left + 'px',
            top: obj.top + 'px'
        });
    }
}