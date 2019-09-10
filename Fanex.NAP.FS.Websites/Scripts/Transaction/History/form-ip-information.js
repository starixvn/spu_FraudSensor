function form_ip_information() {
    var form = $('#form-ip-information');

    this.OpenModal = function (ip, offset) {
        GetIPInformation(ip);     

        if (form.is('dialog')) {
            form.dialog('open');
        } else {
            form.dialog({
                modal: true,                
                width: '550px',
                title: 'IP INFORMATION',
                resizable: false,
                draggable: false,
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