function form_retract_evidence() {
    var form = $('#form-retract-evidence');
    var form_confirmation = $('#retract-evidence-confirmation');    

    this.OpenModal = function () {
        if (form.is('dialog')) {
            form.dialog('open');
        } else {
            form.dialog({
                modal: true,
                width: '500px',
                title: 'RETRACT EVIDENCE',
                resizable: false,
                draggable: true,
                close: function () { $(this).dialog('close'); }
            });
        }
    }

    function ShowConfirmation(message) {
        $('#retract-evidence-confirmation-message').text(message);

        if (form_confirmation.is('dialog')) {
            form_confirmation.dialog('open');
        } else {
            form_confirmation.dialog({
                modal: true,
                width: '400px',
                title: 'RETRACT EVIDENCE',
                resizable: false,
                draggable: true,
                close: function () { $(this).dialog('close'); }
            });
        }

        $('#retract-evidence-confirmation-btn-cancel').click(function () {
            form_confirmation.dialog('close');
        });

        $('#retract-evidence-confirmation-btn-ok').click(function () {
            form_confirmation.dialog('close');
        });
    }

    $('#retract-evidence-btn-cancel').click(function () {
        form.dialog('close');
    });

    $('#retract-evidence-btn-retract').click(function () {
        ShowConfirmation("Are you sure you want to remove this evidence?");

    });
}