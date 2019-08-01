function chains_to_evidence(accountId) {
    var form = $('#addException');
   
    this.OpenAddException = function () {
        if (form.is('dialog')) {
            form.dialog('open');
        } else {
            $("#addException").dialog({
                modal: true,
                width: '650px',
                resizable: false,
                draggable: false,
                close: function () { $(this).dialog('close'); }
            })
        }
    }

    this.AddException = function (exceptionAccountId) {
        var comment = $('#comment').val();
        var url = common.modifyRelativeUrl('/SearchAccount/AddException');

        $.ajax(url, {
            type: 'POST',
            data: {
                accountId: accountId,
                exceptionAccountId: exceptionAccountId,
                comment: comment
            },
            success: function (response) {
                common.showSuccess('Exception has added!');
            },
            error: function (errorMessage) {
                console.log(errorMessage);
                common.showError('Exception add fail!');
            }
        });
        
        form.dialog('close');
    }

    this.CloseException = function () {
        form.dialog('close');
    }
}