function form_information_evidence() {
    var information_evidence_form = $('#form-information-evidence');
    var retract_evidence_form = new form_retract_evidence();

    this.OpenModal = function () {
        GetEvidenceInformation();
    }

    function GetEvidenceInformation() {
        var url = common.modifyRelativeUrl('/Account/GetInformationEvidenceByAccount');
        var paramsFilter = {};
        paramsFilter.AccountId = ACCOUNTID;
        paramsFilter.CustId = CUSTID;

        $.ajax(url, {
            type: 'POST',
            data: paramsFilter,
            success: function (response) {
                BindEvidenceInformation(response);
            },
            error: function (errMsg) {
                console.log(errMsg);
                common.showError(errMsg);
            }
        });
    }

    function BindEvidenceInformation(data) {
        if (information_evidence_form.is('dialog')) {
            information_evidence_form.dialog('open');
        } else {
            information_evidence_form.dialog({
                modal: true,
                width: '750px',
                title: 'EVIDENCE INFORMATION',               
                resizable: false,
                draggable: true,
                close: function () { $(this).dialog('close'); }
            });
        }
    }

    $('#information-evidence-btn-retract').click(function () {
        retract_evidence_form.OpenModal();
    });
}