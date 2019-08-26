function form_add_evidence(accountId) {
    var form = $('#form-evidence');

    this.OpenModal = function () {
        if (form.is('dialog')) {
            form.dialog('open');
        } else {
            $(form).dialog({
                modal: true,
                width: '750px',
                title: 'ADD EVIDENCE',
                resizable: true,
                draggable: true,
                close: function () { $(this).dialog('close'); }
            })
        }
    }

    this.Submit = function () {
        var comment = $('#comment').val();
        var url = common.modifyRelativeUrl('/Account/UpdateEvidence');

        $.ajax(url, {
            type: 'POST',
            data: {
                accountId: accountId,
                comment: comment
            },
            success: function (response) {
                common.showSuccess('Add Evidence has added!');
            },
            error: function (errorMessage) {
                console.log(errorMessage);
                common.showError('Add Evidence add fail!');
            }
        });

        form.dialog('close');
    }

    function CloseModal() {
        form.dialog('close');
    }

    $('#btn-cancel-evidence').click(CloseModal);
    
}