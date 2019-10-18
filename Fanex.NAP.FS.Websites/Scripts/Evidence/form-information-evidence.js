function form_information_evidence() {
    var information_evidence_form = $('#form-information-evidence');
    var retract_evidence_form = new form_retract_evidence();
    var history_table = $('#information-evidence-table');
    var btn_retract = $('#information-evidence-btn-retract');
    var btn_comment_edit = $('#info-evidence-comment-btn-edit');
    var btn_comment_save = $('#info-evidence-comment-btn-save');
    var comment_input = $('#info-evidence-comment-input');
    var comment_txtinput = $('#info-evidence-comment-txt-input');
    var comment_display = $('#info-evidence-comment-display');
    var evidence_type_code = '';

    this.OpenModal = function (evidenceTypeCode) {
        evidence_type_code = evidenceTypeCode;

        if (!information_evidence_form.hasClass("ui-dialog-content")) {
            generateGrid();
        }

        getEvidenceInformation();
        showCommentForDisplay();
    }

    function generateGrid() {
        history_table.DataTable({
            dom: 't<"pagination"<"page-size"><"bottom"lp<"clear">>"i>',
            language: {
                lengthMenu: "Page size _MENU_",
                paginate: {
                    previous: "Back"
                },
                info: "Displaying _START_ to _END_ of _TOTAL_ item(s)",
                infoEmpty: "",
                zeroRecords: "There's no data"
            },                 
            ordering: false,          
            paging: false,
            info: false,        
            scrollX: false,
            scrollCollapse: false,
            columnDefs: [
                { targets: "_all", defaultContent: "-" },
                { targets: [0], width: "20%", className: "text-center" },
                { targets: [1], width: "10%", className: "text-left" },
                { targets: [2], width: "10%", className: "text-left" },
                { targets: [3], width: "20%", className: "text-left" }
            ],
            columns: [
                { data: obj => obj.CreatedDate },
                { data: obj => obj.Action },
                { data: obj => obj.Creator },
                { data: obj => obj.EvidenceType },
                { data: obj => obj.Comment }
            ]
        });

        $(".pagination").hide();
    }

    function getEvidenceInformation() {
        var url = common.modifyRelativeUrl('/Account/GetInformationEvidenceByAccount');
        var paramsFilter = {};
        paramsFilter.EvidenceTypeCode = evidence_type_code;
        paramsFilter.AccountId = ACCOUNTID;
        paramsFilter.CustId = CUSTID;

        $.ajax(url, {
            type: 'POST',
            data: paramsFilter,
            success: function (response) {
                bindEvidenceInformation(response);
            },
            error: function (errMsg) {
                console.log(errMsg);
                common.showError(errMsg);
            }
        });
    }

    function bindEvidenceInformation(data) {
        $('#info-evidence-account').text(data.UserName);
        $('#info-evidence-type').text(data.EvidenceType);
        $('#info-evidnece-createby').text(data.CreatedBy);
        comment_display.text(data.Comment);
        comment_txtinput.val(data.Comment);

        history_table.DataTable().clear().rows.add(data.History).draw();

        if (information_evidence_form.hasClass("ui-dialog-content")) {
            information_evidence_form.dialog('open');
        } else {
            information_evidence_form.dialog({
                modal: true,
                width: '850px',
                title: 'EVIDENCE INFORMATION',
                resizable: false,
                draggable: true,
                close: function () { $(this).dialog('close'); }
            });
        }
    }

    function verifyBeforeSave() {
        var oldComment = comment_display.text().trim();
        var newComment = comment_txtinput.val().trim();

        if (oldComment != newComment) {
            return true;
        }

        return false;
    }    

    function updateEvidenceByAccountOfComment() {
        var url = common.modifyRelativeUrl('/Account/UpdateEvidenceByAccountOfComment');
        var paramsUpdate = {};
        paramsUpdate.AccountId = ACCOUNTID;
        paramsUpdate.CustId = CUSTID;
        paramsUpdate.EvidenceTypeCode = evidence_type_code;
        paramsUpdate.Comment = common.handleSpecialCharacter(comment_txtinput.val());

        common.showLoading();

        $.ajax(url, {
            type: 'POST',
            data: paramsUpdate,
            success: function (response) {
                if (response.ErrorCode == 0) {
                    common.showSuccess();
                    getEvidenceInformation();
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

    function showCommentForDisplay() {
        comment_display.show();
        comment_input.hide();
        btn_comment_edit.show();
        btn_comment_save.hide();
    }

    function showCommentForInput() {
        comment_display.hide();
        comment_input.show();
        btn_comment_edit.hide();
        btn_comment_save.show();
    }

    btn_retract.click(function () {
        retract_evidence_form.OpenModal(evidence_type_code);
    });

    btn_comment_edit.click(function () {
        showCommentForInput();    
    });    

    btn_comment_save.click(function () {
        if (verifyBeforeSave()) {
            updateEvidenceByAccountOfComment();
        } 

        showCommentForDisplay();
    });
}