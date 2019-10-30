function associated_account_monitor_detail() {
    var filter_evidences = $('#filter-evidences');
    var filter_date_from = $('#filter-associated-date-from');
    var filter_date_to = $('#filter-associated-date-to');
    var btn_generate = $('#btn-generate');
    var grid_associated_account = $('#grid-associated-account');
    var const_note_length_of_collapse = 50;

    function init() {
        filter_evidences.multiselect({
            menuWidth: 330,
            menuHeight: 350
        })
            .multiselect('checkAll')
            .multiselectfilter({
                width: 250,
                placeholder: '',
                label: 'Filter'
            });

        var dateFromMaxLimit = new Date();
        var dateFromMinLimit = new Date();
        dateFromMinLimit.setDate(dateFromMinLimit.getDate() - 29);

        filter_date_from.datepicker({
            dateFormat: "M dd, yy",
            changeMonth: true,
            changeYear: true,
            firstDay: 1,
            maxDate: "y"
        }).datepicker("option", "minDate", dateFromMinLimit)
            .datepicker("option", "maxDate", dateFromMaxLimit)
            .datepicker("setDate", new Date());

        filter_date_to.datepicker({
            dateFormat: "M dd, yy",
            changeMonth: true,
            changeYear: true,
            firstDay: 1,
            maxDate: "y"
        }).datepicker("setDate", new Date())
            .change(restrictFilterDateFrom);

        renderAssociatedAccountGridOptions();
    }

    function restrictFilterDateFrom() {
        var dateMinLimit = filter_date_to.datepicker("getDate");
        var dateMaxLimit = filter_date_to.datepicker("getDate");

        if (dateMinLimit != null) {
            dateMinLimit.setDate(dateMinLimit.getDate() - 29);
            filter_date_from.datepicker("option", "minDate", dateMinLimit)
                .datepicker("option", "maxDate", dateMaxLimit);
        }
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
                { targets: "_all", defaultContent: "-" }
            ],
            columns: [
                { data: (row, type, set, meta) => meta.row + 1, className: "text-center", width: "5%" },
                { data: row => row.MainAccountUsername, className: "text-center", width: "10%" },
                { data: row => row.MainAccountSite, className: "text-center", width: "10%" },
                { data: row => row.MainAccountLevelName, className: "text-center", width: "10%" },
                { data: row => renderEvidence(row.MainAccountEvidences), className: "text-left", width: "10%" },
                { data: row => row.FirstAssociatedDateToString, className: "text-center", width: "10%" },
                { data: row => row.Verified, className: "text-center", width: "10%" },
                {
                    data: renderverificationCheckBox, className: "text-center", width: "5%", createdCell: (cell, _, row) => {
                        $('.chk-verification', cell).change(() => handleUpdateVerification(cell, row))
                    }
                },
                { data: row => row.VerifiedOfView, className: "text-center", width: "8%" },
                {
                    data: renderNote, className: "col-account-note text-left", createdCell: (cell, _, row) => {
                        $('.btn-note-change', cell).click(() => handleChangeNote(cell, row));
                        $('.btn-note-ellipsis', cell).click(() => handleExpandNote(cell));
                    }
                }
            ],
            rowCallback: function (cell, row, index) {
                adjustSizeOfNote(cell);
            },
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

    function renderEvidence(evidences) {
        if (evidences != null && evidences != '') {
            var evidencesForDisplay = common.makeWrapString(evidences, 26);
            return "<span title='" + evidences + "'>" + evidencesForDisplay + "</span>";
        }
        return evidences;
    }

    function renderverificationCheckBox(row) {
        return "<label class='checkbox'>"
            + "<input type= 'checkbox' data-ass-acc-id = '" + row.AssAccId + "' class='chk-verification'" + (row.IsVerified ? "checked" : "") + "/>"
            + "<span></span></label>";
    }

    function renderNote(row) {
        var noteForDisplay = wrapTextForDipslayNote(row.Note, const_note_length_of_collapse);
        var domElementNoteForDisplay = '<div class="note-content-display" title=' + row.Note + '>' + noteForDisplay + '</div>';

        if (noteForDisplay == '-') {
            domElementNoteForDisplay = '<div class="note-content-display note-blank" title=' + row.Note + '>' + noteForDisplay + '</div>';
        }

        return '<div class="container-note">'
            + '<textarea class="txt-note-input hide" data-ass-acc-id="' + row.AssAccId + '" rows="10">' + (row.Note != '' ? row.Note : '') + '</textarea>'
            + domElementNoteForDisplay
            + '<div class="btn-note-change icon-edit icon"></div>'
            + '</div>';
    }

    function wrapTextForDipslayNote(textInput, maxLengthDisplay) {
        if (textInput == null || textInput == '') {
            return '-';
        }
        var textOutPut = '';
        var textBreaks = textInput.split('\n');
        var htmlSpan = '<span class="btn-note-ellipsis" title="click to see whole note"> ... </span>';

        for (var i = 0; i < textBreaks.length; i++) {
            if (i == 3) {
                textOutPut = textOutPut + textBreaks[i];
                return textOutPut.substring(0, maxLengthDisplay - 4) + htmlSpan;
            }

            textOutPut = textOutPut + textBreaks[i] + "\n";
            if (textOutPut.length > maxLengthDisplay) {
                return textOutPut.substring(0, maxLengthDisplay - 4) + htmlSpan;
            }
        }

        return textInput;
    }

    function generateReport() {
        var url = common.modifyRelativeUrl("/Account/GetAssociatedAccountsToMonitor");
        var paramFilter = getParamsFilter();

        if (validateFilter(paramFilter)) {
            common.showLoading();

            $.ajax({
                type: 'POST',
                url: url,
                data: paramFilter,
                success: function (response) {
                    renderAssociatedAccountGrid(response);
                },
                error: function (xhr, status, error) {
                    console.log('data of ' + xhr.responseText);
                    common.closeLoading();
                }
            });
        }
    }

    function getParamsFilter() {
        var filter = {};

        filter.AssociatedDateFrom = filter_date_from.val();
        filter.AssociatedDateTo = filter_date_to.val();
        filter.EvidenceCodes = getSeletedEvidences();
        filter.EvidencesCache = evidencesCache;

        return filter;
    }

    function getSeletedEvidences() {
        var evidences = [];
        $('option:selected', "#filter-evidences").each(function (idx, ele) {
            evidences.push($(ele).val());
        });

        return evidences.join(",");
    }

    function validateFilter(filter) {
        var isOK = true;
        $(filter_date_from).removeClass("fail-by-validate");
        $(filter_date_to).removeClass("fail-by-validate");

        if (filter.AssociatedDateFrom == "") {
            $(filter_date_from).addClass("fail-by-validate");
            isOK = false;
        }

        if (filter.AssociatedDateTo == "") {
            $(filter_date_to).addClass("fail-by-validate");
            isOK = false;
        }
        return isOK;
    }

    function renderAssociatedAccountGrid(json) {
        grid_associated_account.DataTable().clear().rows.add(json).draw();
        grid_associated_account.DataTable().columns.adjust();
        common.closeLoading();
    }

    function handleUpdateVerification(cell, row) {
        var $target = $(cell).find('.chk-verification');

        var url = common.modifyRelativeUrl('/Account/UpdateAssociatedAccountToMonitorOfVerify');
        var paramsUpdate = {};
        paramsUpdate.IsVerifying = $target.is(':checked');
        paramsUpdate.AssAccId = $target.attr('data-ass-acc-id');

        common.showLoading();
        $.ajax({
            type: 'POST',
            url: url,
            data: paramsUpdate,
            success: function (response) {
                common.closeLoading();
                if (response.IsSuccessStatusCode) {
                    if (paramsUpdate.IsVerifying) {
                        $target.parents("td").next().text(response.Content);
                    }
                    else {
                        $target.parents("td").next().text("-");
                    }
                    common.showSuccess();
                } else {
                    common.showError(response.Content);
                }
            },
            error: function (xhr, status, error) {
                console.log('data of ' + xhr.responseText);
                common.closeLoading();
            }
        });
    }

    function handleChangeNote(cell, row) {
        var domElementsNote = {};
        domElementsNote.txtNoteInput = $(cell).find('.txt-note-input');
        domElementsNote.noteForDisplay = $(cell).find(".note-content-display");
        domElementsNote.iconNote = $(cell).find(".icon");

        if ($(domElementsNote.txtNoteInput).hasClass('hide')) {
            handleEditNote(domElementsNote);
        } else if ($(domElementsNote.iconNote).hasClass('icon-save')) {
            handleSaveNote(cell, row);
        }
    }

    function handleEditNote(domElementsNote) {
        $(domElementsNote.txtNoteInput).removeClass('hide').focus().select();
        $(domElementsNote.noteForDisplay).addClass('hide').hide();
        $(domElementsNote.iconNote).removeClass('icon-edit').addClass('icon-save').attr('title', 'save');
        grid_associated_account.DataTable().columns.adjust();

        adjustHeightNoteInput(domElementsNote);
        handleNoteInputEvents(domElementsNote);
    }

    function adjustHeightNoteInput(domElementsNote) {
        $(domElementsNote.txtNoteInput).height($(domElementsNote.noteForDisplay).height());
    }

    function handleNoteInputEvents(domElementsNote) {
        $(domElementsNote.txtNoteInput).mouseout(function () {
            grid_associated_account.DataTable().columns.adjust();
        });

        $(domElementsNote.txtNoteInput).keyup(function () {
            $(domElementsNote.txtNoteInput).addClass('change');
        });
    }

    function handleSaveNote(cell, row) {
        var domElementsNote = {};
        domElementsNote.txtNoteInput = $(cell).find('.txt-note-input');
        domElementsNote.contentNoteDisplay = $(cell).find(".note-content-display");

        var noteToUpdate = $(domElementsNote.txtNoteInput).val().trim();
        noteToUpdate = handleSpecialCharacter(noteToUpdate);

        if ($(domElementsNote.txtNoteInput).hasClass('change')) {
            saveChangedNote(domElementsNote, noteToUpdate, cell);
        }

        $(domElementsNote.txtNoteInput).removeClass('change').addClass('hide');
        $(domElementsNote.contentNoteDisplay).removeClass('hide').show();
        $(cell).find(".icon").removeClass('icon-save').addClass('icon-edit').attr('title', 'edit');

        grid_associated_account.DataTable().columns.adjust();
    }

    function saveChangedNote(domElementsNote, noteToUpdate, cell) {
        var url = common.modifyRelativeUrl("/Account/UpdateAssociatedAccountToMonitorOfNote");
        var paramsUpdate = {};
        paramsUpdate.AssAccId = $(domElementsNote.txtNoteInput).attr('data-ass-acc-id');
        paramsUpdate.Note = noteToUpdate;

        common.showLoading();
        $.ajax({
            type: 'POST',
            url: url,
            data: paramsUpdate,
            success: function (response) {
                common.closeLoading();
                if (response.IsSuccessStatusCode) {
                    var noteForDisplay = wrapTextForDipslayNote(noteToUpdate, const_note_length_of_collapse);
                    if (noteForDisplay == '-') {
                        $(domElementsNote.contentNoteDisplay).addClass('note-blank');
                        adjustSizeOfNote(cell);
                    } else {
                        $(domElementsNote.contentNoteDisplay).removeClass('note-blank');
                        $(domElementsNote.contentNoteDisplay).css({ 'padding-top': '6px' });
                    }
                    $(domElementsNote.contentNoteDisplay).html(noteForDisplay);
                    $(".btn-note-ellipsis", cell).click(() => handleExpandNote(cell));

                    common.showSuccess();
                } else {
                    common.showError(response.Content);
                }
            },
            error: function (xhr, status, error) {
                console.log('data of ' + xhr.responseText);
                common.closeLoading();
            }
        });
    }

    function adjustSizeOfNote(cell) {
        var domElementNoteDisplay = $(cell).find(".note-content-display");
        var domElementNoteBlank = $(cell).find(".note-blank");

        if ($(window).width() < 1108) {
            domElementNoteDisplay.css({ 'min-height': '77px' });
            domElementNoteBlank.css({ 'padding-top': '27px' });
        } else if ($(window).width() < 1835) {
            domElementNoteDisplay.css({ 'min-height': '57px' });
            domElementNoteBlank.css({ 'padding-top': '17px' });
        }
    }

    function handleExpandNote(cell) {
        var noteForDisplay = $(cell).find('.txt-note-input').val();

        var noteForDisplay = noteForDisplay + "  " + '<span class="btn-note-collapse icon-chevron-up" title="click to collapse up"></span>';

        $(cell).find(".note-content-display").html(noteForDisplay);

        $('.btn-note-collapse', cell).click(() => handleCollapseNote(cell));
    }

    function handleCollapseNote(cell) {
        var txtNoteInput = $(cell).find('.txt-note-input').val();
        var noteForDisplay = wrapTextForDipslayNote(txtNoteInput, const_note_length_of_collapse);
        $(cell).find(".note-content-display").html(noteForDisplay);

        grid_associated_account.DataTable().columns.adjust();
        $(".btn-note-ellipsis", cell).click(() => handleExpandNote(cell));
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

$(associated_account_monitor_detail);