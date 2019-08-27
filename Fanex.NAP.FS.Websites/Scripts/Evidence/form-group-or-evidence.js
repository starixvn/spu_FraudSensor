function form_group_or_evidence() {
    var form = $('#form-add-group-or-evidence');

    this.OpenModal = function (captian) {       
        if (form.is('dialog')) {
            form.dialog('open');
        } else {
            $(form).dialog({
                modal: true,
                width: '450px',
                title: captian,
                resizable: false,
                draggable: false,
                close: function () { $(this).dialog('close'); }
            })
        }
    }

    this.AddGroupOrEvidence = function () {
        var name = $('#group-or-evidence-input').val();
        var url = common.modifyRelativeUrl('/Account/AddGroupEvidence');

        //$.ajax(url, {
        //    type: 'POST',
        //    data: {
        //        name: name,                
        //        comment: comment
        //    },
        //    success: function (response) {
        //        common.showSuccess('Evidence has added!');
        //    },
        //    error: function (errorMessage) {
        //        console.log(errorMessage);
        //        common.showError('Evidence add fail!');
        //    }
        //});

        form.dialog('close');
    }
}