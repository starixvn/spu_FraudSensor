function form_ip_information() {
    var form = $('#form-ip-information');

    this.OpenModal = function (ip, targetElement) {
        GetIPInformation(ip);              

        var my = "left top+10";
        var at = "left top+10";
       
        if (form.is('dialog')) {
            form.dialog('open');
        } else {
            form.dialog({
                modal: true,
                width: '530px',
                position: {
                    my: my,
                    at: at,
                    of: targetElement
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
}