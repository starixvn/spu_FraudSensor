function form_retract_evidence() {
    var form_retract = $('#form-retract-evidence');
    var form_confirmation = $('#retract-evidence-confirmation');
    var comment_input = $('#retract-evidence-comment');
    var evidence_type_code = '';

    this.OpenModal = function (evidenceTypeCode) {
        evidence_type_code = evidenceTypeCode;

        if (form_retract.is('dialog')) {
            form_retract.dialog('open');
        } else {
            form_retract.dialog({
                modal: true,
                width: '500px',
                title: 'RETRACT EVIDENCE',
                resizable: false,
                draggable: true,
                close: function () { $(this).dialog('close'); }
            });
        }
    }

    function showConfirmation(message) {
        $('#retract-evidence-confirmation-message').text(message);
        if (form_confirmation.hasClass("ui-dialog-content")) {
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

            retractEvidenceByAccount();

            form_confirmation.dialog('close');
            form_retract.dialog('close');
        });
    }

    function retractEvidenceByAccount() {
        var url = common.modifyRelativeUrl('/Account/RetractEvidenceByAccount');
        var paramsRetract = {};
        paramsRetract.AccountId = ACCOUNTID;
        paramsRetract.CustId = CUSTID;
        paramsRetract.EvidenceTypeCode = evidence_type_code;
        paramsRetract.Comment = common.handleSpecialCharacter(comment_input.val());

        common.showLoading();

        $.ajax(url, {
            type: 'POST',
            data: paramsRetract,
            success: function (response) {
                if (response.ErrorCode == 0) {
                    common.showSuccess();
                } else {
                    common.showError(response.ErrorMsg);
                }
                common.closeLoading();
            },
            error: function (errMsg) {
                console.log(errMsg);
                common.showError(errMsg);
                common.closeLoading();
            }
        });
    }

    $('#retract-evidence-btn-cancel').click(function () {
        form_retract.dialog('close');
    });

    $('#retract-evidence-btn-retract').click(function () {
        showConfirmation("Are you sure you want to remove this evidence?");
    });
}