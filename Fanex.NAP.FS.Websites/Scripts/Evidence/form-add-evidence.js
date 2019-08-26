function form_add_evidence(accountId) {
    var form = $('#form-evidence');
    var evidences_groups = $('#evidences-groups');

    this.OpenModal = function () {
        if (form.is('dialog')) {
            form.dialog('open');
        } else {
            $(form).dialog({
                modal: true,
                width: '750px',
                title: 'ADD EVIDENCE',
                resizable: true,
                draggable: true,
                close: function () { $(this).dialog('close'); }
            })
        }

        GetAllEvidences();
    }

    this.Submit = function () {
        var comment = $('#comment').val();
        var url = common.modifyRelativeUrl('/Account/UpdateEvidence');

        $.ajax(url, {
            type: 'POST',
            data: {
                accountId: accountId,
                comment: comment
            },
            success: function (response) {
                common.showSuccess('Add Evidence has added!');
            },
            error: function (errorMessage) {
                console.log(errorMessage);
                common.showError('Add Evidence add fail!');
            }
        });

        form.dialog('close');
    }

    function CloseModal() {
        form.dialog('close');
    }

    function GetAllEvidences() {
        var url = common.modifyRelativeUrl("/Account/GetEvidences");

        $.get(url, function (json) {
            BindEvidences(json);
        });
    }

    function BindEvidences(data) {
        var html = '';
        var groups = [];
        var length = data.length;

        for (var i = 0; i < length; i++) {
            html += ToHtmlGroupStart(data[i], groups);
            html += ToHtmlEvidence(data[i], html);

            if (i == length - 1) {
                html += ToHtmlGroupEnd();
            }
        }

        $(evidences_groups).append(html);
    }

    function ToHtmlGroupStart(obj, groups) {
        var html = '';

        if (groups.indexOf(obj.EvidenceGroupID) < 0) {
            if (groups.length > 0) {
                html += ToHtmlGroupEnd();
            }
            html = '<div class="group">';
            html += '<div class="group-heading">'+ obj.EvidenceGroupName + '</div>';
            html += '<div class="group-content" group="' + obj.EvidenceGroupID + '"></div>';

            groups.push(obj.EvidenceGroupID);

            return html;
        }

        return html;
    }

    function ToHtmlGroupEnd() {
        return '</div>';
    }

    function ToHtmlEvidence(obj) {
        return '<div class="item"><span>' + obj.EvidenceTypeCode + " " + obj.EvidenceTypeName + '</span></div>';
    }

    $('#btn-cancel-evidence').click(CloseModal);
    
}