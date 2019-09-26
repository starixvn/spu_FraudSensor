function associated_account() {
    var filter_evidences = $('#filter-evidences');
    var filter_verify_status = $('#filter-verify-status');
    var filter_date_from = $('#filter-associated-date-from');
    var filter_date_to = $('#filter-associated-date-to');
    var btn_generate = $('#btn-generate');
    var grid_associated_account = $('#grid-associated-account');
    var const_note_max_length = 500;
    var const_note_length_of_collapse = 100;

    function init() {
        $(filter_evidences).multiselect({
            menuWidth: 330,
            menuHeight: 350
        }).multiselectfilter({
            width: 250,
            placeholder: '',
            label: 'Filter'
        });

        $(filter_date_from).datepicker({
            dateFormat: "M dd, yy",
            changeMonth: true,
            changeYear: true,
            firstDay: 1,
            minDate: "-1y +1d"
        });

        $(filter_date_to).datepicker({
            dateFormat: "M dd, yy",
            changeMonth: true,
            changeYear: true,
            firstDay: 1
        });

        renderAssociatedAccountGridOptions();
    }

    function renderAssociatedAccountGridOptions() {
        grid_associated_account.DataTable({
            searching: false,
            lengthMenu: [10, 20, 50, 100, 500],
            pageLength: 50,
            ordering: false,
            scrollY: "61vh",
            scrollX: true,
            scrollCollapse: true,
            columnDefs: [
                { targets: "_all", defaultContent: "-" },
                { targets: 10, type: "check-box" }
            ],
            columns: [
                { data: (row, type, set, meta) => meta.row + 1, className: "text-center", width: "10%" },
                { data: row => row.AssociatedAccountUsername, className: "text-center", width: "10%" },
                { data: row => row.AssociatedAccountSite, className: "text-center", width: "10%" },
                { data: row => row.AssociatedAccountLevel, className: "text-center", width: "10%" },
                { data: row => renderEvidence(row.AssociatedAccountEvidences), className: "text-center", width: "10%" },
                { data: row => row.MainAccountUsername, className: "text-center", width: "10%" },
                { data: row => row.MainAccountSite, className: "text-center", width: "10%" },
                { data: row => row.MainAccountLevel, className: "text-center", width: "10%" },
                { data: row => renderEvidence(row.MainAccountEvidences), className: "text-center", width: "10%" },
                { data: row => row.FirstAssociated, className: "text-center", width: "10%" },
                {
                    data: renderverificationCheckBox, className: "text-center", width: "10%", createdCell: (cell, _, row) => {
                        $('.chk-verification', cell).change(() => handleUpdateVerification(cell, row))
                    }
                },
                { data: row => row.VerifiedBy, className: "text-center", width: "10%" },
                {
                    data: renderNote, className: "text-center", width: "10%", createdCell: (cell, _, row) => {
                        $('.btn-note', cell).click(() => handleEditNote(cell, row));
                        $('.btn-note-ellipsis', cell).click(() => handleExpandNote(cell));
                    }
                }
            ],
            dom: "t<'pagination'<'page-size'lp>i>",
            language: {
                lengthMenu: "Page size _MENU_",
                paginate: {
                    previous: "Back"
                },
                info: "Displaying _START_ to _END_ of _TOTAL_ item(s)",
                infoEmpty: "",
                zeroRecords: "There's no data"
            }
        });
    }

    function renderEvidence(evidence) {
        return evidence;
    }

    function renderverificationCheckBox(row) {
        return "<label class='checkbox'>"
            + "<input type= 'checkbox' class='chk-verification'" + (row.Verified ? "checked" : "") + "/>"
            + "<span></span></label>";
    }

    function renderNote(row) {
        var noteForDisplay = wrapTextForDipslayNote(row.Note, const_note_length_of_collapse);

        return '<div class="container-note">'
            + '<textarea class="txt-note hide" rows="5">' + (row.Note != '-' ? row.Note : '') + '</textarea>'
            + '<div class="note-content">' + noteForDisplay + '</div>'
            + '<div class="btn-note icon-edit icon"></div>'
            + '</div>';
    }

    function wrapTextForDipslayNote(textInput, maxLengthDisplay) {
        if (textInput == null || textInput == '') {
            return '-';
        }
        var textOutPut = '';
        var textBreaks = textInput.split('\n');
        var spanHtml = '<span class="btn-note-ellipsis" title="click to see whole note"> ... </span>';

        for (var i = 0; i < textBreaks.length; i++) {
            if (i == 5) {
                textOutPut = textOutPut + textBreaks[i];
                return textOutPut.substring(0, maxLengthDisplay - (i * 2) - 3) + spanHtml;
            }

            textOutPut = textOutPut + textBreaks[i] + "\n";
            if (textOutPut.length > maxLengthDisplay) {
                return textOutPut.substring(0, maxLengthDisplay - (i * 2) - 3) + spanHtml;
            }
        }

        return textInput;
    }

    function generateReport() {
        var url = common.modifyRelativeUrl("/Account/GetAssociatedAccounts");
        var paramFilter = getParamsFilter();
        common.showLoading();

        $.ajax({
            type: 'POST',
            url: url,
            data: paramFilter,
            global: false,
            crossDomain: true,
            success: function (response) {
                renderAssociatedAccountGrid(response);
            },
            error: function (xhr, status, error) {
                console.log('data of ' + xhr.responseText);
                common.showLoading();
            }
        });
    }

    function getParamsFilter() {
        var filter = {};

        filter.AssociatedDateFrom = $('').val();
        filter.AssociatedDateTo = $('').val();
        filter.VerifyStatus = $('').val();
        filter.EvidenceCodes = getSeletedEvidences();

        return filter;
    }

    function getSeletedEvidences() {
        var evidences = [];
        $('option:selected', "#filter-evidences").each(function (idx, ele) {
            evidences.push($(ele).val());
        });

        return evidences.join(",");
    }

    function renderAssociatedAccountGrid(json) {
        grid_associated_account.DataTable().clear().rows.add(json).draw();
        grid_associated_account.DataTable().columns.adjust();
        common.closeLoading();
    }

    function handleUpdateVerification(cell, row) {
        var $target = $(cell).find('.chk-verification');
        common.showConfirmation('Do you want to change verify ?', function () {
            var url = common.modifyRelativeUrl('/Account/UpdateAssociatedAccountOfVerify');
            var paramsUpdate = {};
            paramsUpdate.verified = $target.is(":checked");

            common.showLoading();
            $.ajax({
                type: 'POST',
                url: url,
                data: paramsUpdate,
                success: function (response) {
                    if (paramsUpdate.verified) {                        
                        var html = username + " " + getCurrentDate();
                        $target.parents("td").next().text(html);
                    }
                    else {
                        $target.parents("td").next().text("-");
                    }
                    common.closeLoading();
                    common.showSuccess();
                },
                error: function (xhr, status, error) {
                    console.log('data of ' + xhr.responseText);
                    common.closeLoading();
                }
            });
        });
    }    

    function getCurrentDate() {
        const months = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];

        var currentDatetime = new Date();

        return months[currentDatetime.getMonth()] + " " + currentDatetime.getDate() + ", " + currentDatetime.getFullYear();
    } 

    function handleEditNote(cell, row) {
        var txtNote = $(cell).find('.txt-note');
        var noteForDisplay = $(cell).find(".note-content");
        var iconNote = $(cell).find(".icon");        

        if ($(txtNote).hasClass('hide')) {
            $(txtNote).removeClass('hide').focus().select();

            // adjust height input Note
            $(txtNote).height($(noteForDisplay).height());

            $(noteForDisplay).addClass('hide').hide();
            $(iconNote).removeClass('icon-edit').addClass('icon-save').attr('title', 'save');

            $(txtNote).mouseout(function () {
                grid_associated_account.DataTable().columns.adjust();
            });

            $(txtNote).keyup(function () {
                $(txtNote).addClass('change');
            });

            grid_associated_account.DataTable().columns.adjust();

        } else if ($(iconNote).hasClass('icon-save')) {
            handleSaveNote(cell, row);
        }
    }

    function handleExpandNote(cell) {
        var noteForDisplay = $(cell).find('.txt-note').val();

        var noteForDisplay = noteForDisplay + "  " + '<span class="btn-note-collapse icon-chevron-up" title="click to collapse up"></span>';

        $(cell).find(".note-content").html(noteForDisplay);

        $('.btn-note-collapse', cell).click(() => handleCollapseNote(cell));
    }

    function handleCollapseNote(cell) {
        var txtNote = $(cell).find('.txt-note').val();    
        var noteForDisplay = wrapTextForDipslayNote(txtNote, const_note_length_of_collapse);
        $(cell).find(".note-content").html(noteForDisplay);
       
        grid_associated_account.DataTable().columns.adjust();
        $(".btn-note-ellipsis", cell).click(() => handleExpandNote(cell));
    }

    function handleSaveNote(cell, row) {
        var txtNote = $(cell).find('.txt-note');
        var content = $(cell).find(".note-content");
        var noteToUpdate = $(txtNote).val().trim();
        noteToUpdate = handleSpecialCharacter(noteToUpdate);

        if ($(txtNote).hasClass('change')) {            
            var url = common.modifyRelativeUrl("/Account/UpdateAssociatedAccountOfNote");
            var paramsUpdate = {};
            paramsUpdate.Note = noteToUpdate;

            common.showLoading();
            $.ajax({
                type: 'POST',
                url: url,
                data: paramsUpdate,
                success: function (response) {
                    var noteForDisplay = wrapTextForDipslayNote(noteToUpdate, const_note_length_of_collapse);
                    $(content).html(noteForDisplay);
                    $(".btn-note-ellipsis", cell).click(() => handleExpandNote(cell));
                    common.closeLoading();
                    common.showSuccess();
                },
                error: function (xhr, status, error) {
                    console.log('data of ' + xhr.responseText);
                    common.closeLoading();
                }
            });
        }

        $(txtNote).removeClass('change').addClass('hide');
        $(content).removeClass('hide').show();       
        $(cell).find("#icon").removeClass('icon-note-save').addClass('icon-note-text').attr('title', 'edit');
        grid_associated_account.DataTable().columns.adjust();
    }

    function handleSpecialCharacter(str) {
        if (str != null) {
            return str.replace(/&/g, "&amp;").replace(/>/g, "&gt;").replace(/</g, "&lt;");
        }
        return str;
    }

    init();

    btn_generate.click(generateReport);
}

$(associated_account);