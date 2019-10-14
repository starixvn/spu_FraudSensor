function form_place_evidence(accountId) {
    var form = $('#form-evidence');
    var ddlEvidence = $('#ddlPlaceEvidence');

    this.OpenModal = function () {
        ddlEvidence.multiselect({
            multiple: false,
            selectedList: 1,
            menuWidth: 300,
            menuHeight: 300,
            create: function () {
                $('.ui-multiselect-all').parent().parent().remove();
            }
        }).multiselectfilter({
            width: 220,
            placeholder: ''
        });

        ddlEvidence.bind("change", function (e) {
            var desc = $('#ddlPlaceEvidence :selected').attr('desc');
            if (desc === undefined) desc = '';
            $('#lblPlaceEvidenceDescription').html(desc);
        });

        Clear();

        GetAllEvidences();
    }

    this.Submit = function () {
        var comment = $('#comment').val();
        var url = common.modifyRelativeUrl('/Account/UpdateEvidenceByAccount');

        $.ajax(url, {
            type: 'POST',
            data: {
            },
            success: function (response) {
                common.showSuccess('The Evidence has added!');
                CloseModal();
            },
            error: function (errorMessage) {
                console.log(errorMessage);
                common.showError('The Evidence add fail!');
            }
        });
    }

    function CloseModal() {
        form.dialog('close');
    }

    function Clear() {
         
    }    

    function GetAllEvidences() {
        var url = common.modifyRelativeUrl("/Account/GetAllEvidences");

        $.get(url, function (response) {
            BindEvidences(response);

            if (form.is('dialog')) {
                form.dialog('open');
            } else {
                form.dialog({
                    modal: true,
                    width: '650px',
                    title: 'PLACE EVIDENCE',                   
                    resizable: true,
                    draggable: true,
                    close: function () { $(this).dialog('close'); }
                })
            }
        });
    }

    function BindEvidences(data) {
        ddlEvidence.multiselect().empty();
        var groupId = '';

        for (var i = 0; i < data.length; i++) {
            if (groupId != data[i].EvidenceGroupID) {
                ddlEvidence.append('<optgroup label="' + data[i].EvidenceGroupID + " " + data[i].EvidenceGroupName + '">');
                ddlEvidence.append('<option desc="' + data[i].Description + '" value="' + data[i].EvidenceTypeCode + '">'
                    + data[i].EvidenceTypeCode + " " + data[i].EvidenceTypeName + '</option>');
                ddlEvidence.append('</optgroup>');
            } else {
                ddlEvidence.append('<option desc="' + data[i].Description + '" value="' + data[i].EvidenceTypeCode + '">'
                    + data[i].EvidenceTypeCode + " " + data[i].EvidenceTypeName + '</option>');
            }

            groupId = data[i].EvidenceGroupID;
        }

        ddlEvidence.multiselect('refresh');
        $('#ddlPlaceEvidence_ms').width('300px');
    }

    $('#btn-place-evidence-cancel').click(CloseModal);
}