function form_exception(accountId, useName) {
    var form = $('#addException');
    var exceptionAccountId, exceptionUserName;
   
    this.OpenAddException = function (_exceptionAccountId, _exceptionUserName) {
        exceptionAccountId = _exceptionAccountId;
        exceptionUserName = _exceptionUserName;

        if (form.is('dialog')) {
            form.dialog('open');
        } else {
            $("#addException").dialog({
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
        var comment = $('#comment').val();
        var url = common.modifyRelativeUrl('/Account/AddException');

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

    function BindData() {
        var noteText = "Note: Account " + useName + "is no longer infected Evidence from account " + exceptionUserName;
        $('#note-caption').text(noteText);
        $('#nodeRoot').text(useName);
        $('#nodeException').text(exceptionUserName);
    }
}