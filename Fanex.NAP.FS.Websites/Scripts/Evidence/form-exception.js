function form_exception(accountId, userName, subscriberId) {
    var form = $('#form-exception');
    var exceptionAccountId;
    var exceptionUserName;
   
    this.OpenModal = function (_exceptionAccountId, _exceptionUserName) {
        exceptionAccountId = _exceptionAccountId;
        exceptionUserName = _exceptionUserName;        

        if (form.is('dialog')) {
            form.dialog('open');
        } else {
            form.dialog({
                modal: true,
                width: '650px',
                title: 'ADD EXCEPTION',
                resizable: false,
                draggable: false,
                close: function () { $(this).dialog('close'); }
            })
        } 

        BindData();
    }

    this.AddException = function () {
        var comment = $('#add-exception-comment').val();
        var url = common.modifyRelativeUrl('/Account/AddExceptionEvidence');

        $.ajax(url, {
            type: 'POST',
            data: {
                accountId: accountId,
                exceptionAccountId: exceptionAccountId,
                subscriberId: subscriberId,
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

    function CloseModal() {
        form.dialog('close');
    }

    function BindData() {
        var noteText = "Note: Account " + userName + " is no longer infected Evidence from account " + exceptionUserName;
        $('#note-caption').text(noteText);
        $('#node-root').text(userName);
        $('#node-exception').text(exceptionUserName);
    }

    $('#btn-cancel-exception').click(CloseModal);
}