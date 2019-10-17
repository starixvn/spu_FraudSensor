function form_place_evidence() {
    var form = $('#form-place-evidence');
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
            $('#place-evidence-description').html(desc);
        });

        $('#place-evidence-account').text(USERNAME);
        $('#place-evidence-subscriber').text(SUBSCRIBER);
        $('#place-evidence-comment').val('');

        GetAllEvidences();
    }

    this.Submit = function () {
        var url = common.modifyRelativeUrl('/Account/PlaceEvidenceByAccount');
        var paramsPlace = {};
        paramsPlace.AccountId = ACCOUNTID;
        paramsPlace.EvidenceTypeID = ddlEvidence.find('option:selected').val();
        paramsPlace.Comment = $('#place-evidence-comment').val();

        $.ajax(url, {
            type: 'POST',
            data: paramsPlace,
            success: function (response) {
                common.showSuccess('Place the evidence successful!');
                form.dialog('close');
            },
            error: function (errorMessage) {
                console.log(errorMessage);
                common.showError('Place the evidence fail!');
            }
        });
    }
   
    function GetAllEvidences() {
        var url = common.modifyRelativeUrl("/Account/GetAllEvidences");

        $.get(url, function (response) {
            BindEvidences(response);

            if (form.hasClass("ui-dialog-content")) {
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
            if (groupId !== data[i].EvidenceGroupID) {
                ddlEvidence.append('<optgroup label="' + data[i].EvidenceGroupID + " " + data[i].EvidenceGroupName + '">');
                ddlEvidence.append('<option desc="' + data[i].Description + '" value="' + data[i].EvidenceTypeID + '">'
                    + data[i].EvidenceTypeCode + " " + data[i].EvidenceTypeName + '</option>');
                ddlEvidence.append('</optgroup>');
            } else {
                ddlEvidence.append('<option desc="' + data[i].Description + '" value="' + data[i].EvidenceTypeID + '">'
                    + data[i].EvidenceTypeCode + " " + data[i].EvidenceTypeName + '</option>');
            }

            groupId = data[i].EvidenceGroupID;
        }

        ddlEvidence.multiselect('refresh');
        $('#ddlPlaceEvidence_ms').width('300px');
    }

    $('#btn-place-evidence-cancel').click(function () {
        form.dialog('close');
    });
}