function form_group_or_evidence() {
    var form = $('#form-add-group-or-evidence');
    var groupId = 0;

    this.OpenModal = function (captian, currentGroupId) {       
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

        groupId = currentGroupId;
    }

    this.AddGroupOrEvidence = function () {
        var name = $('#group-or-evidence-input').val();
       
        if (groupId > 0) {
            var url = common.modifyRelativeUrl('/Account/InsertEvidence');
            var param = {
                groupId: groupId,
                evidenceName: name
            }           
        } else {
            var url = common.modifyRelativeUrl('/Account/InsertGroupEvidence');
            var param = {
                groupName: name
            }
        }

        UpdateData(url, param);

        form.dialog('close');
    }

    function UpdateData(url, param) {
        $.ajax(url, {
            type: 'GET',
            data: param,
            success: function (response) {
                common.showSuccess('Data has added!');
            },
            error: function (errorMessage) {
                console.log(errorMessage);
                common.showError('Data add fail!');
            }
        });
    }
}