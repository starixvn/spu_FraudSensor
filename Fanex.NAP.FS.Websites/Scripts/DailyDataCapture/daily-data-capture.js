function daily_data_capture() {
    var from_date = $("#from-date"),
        to_date = $("#to-date"),
        match_table = $("#match-table");
    var inputScannedDate = $("#inputScannedDate");
    var lblSwitchScannedDate = $("#lblSwitchScannedDate");
    var const_note_length_of_collapse = 50;

    function restrict_from_date() {
        var to_val = to_date.datepicker("getDate");

        from_date.datepicker("option", "maxDate", to_val);
    }

    function restrict_to_date() {
        var from_val = from_date.datepicker("getDate");

        to_date.datepicker("option", "minDate", from_val);
    }

    function generate_report() {
        var url = common.modifyRelativeUrl("/DailyDataCapture/GetMatches");

        var params = {};
        params.fromDate = from_date.val();
        params.toDate = to_date.val();

        common.showLoading();

        $.get(url, params, function (json) {
            match_table.DataTable().clear().rows.add(json).draw();
            $('tbody > tr:first', match_table).before('<tr><td></td><td></td><td></td><td></td><td>Stuff</td><td>Stuff</td><td>Stuff</td></tr>');
        }).done(function () {
            common.closeLoading();
        }).fail(function (err) {
                common.closeLoading();
                console.log(err);                
            });
    }

    function generate_excel() {
        var url = common.modifyRelativeUrl("/DailyDataCapture/GetMatchesExcel");

        var params = {};
        params.fromDate = from_date.val();
        params.toDate = to_date.val();

        common.downloadFile(url, params);
    }

    function render_history(history, isInt) {
        var old_value = history.Old;
        var new_value = history.New;

        if (old_value == new_value) {
            if (isInt) {
                return common.formatGridNumber(old_value, "0,0");
            }

            return common.formatGridNumber(old_value);
        }

        if (isInt) {
            return `${old_value == null ? "-" : common.formatGridNumber(old_value, "0,0")}<br/><span class='new-value'>${new_value == null ? "-" : common.formatGridNumber(new_value, "0,0")}</span>`;
        }

        return `${old_value == null ? "-" : common.formatGridNumber(old_value)}<br/><span class='new-value'>${new_value == null ? "-" : common.formatGridNumber(new_value)}</span>`
    }

    from_date.datepicker({
        dateFormat: "M dd, yy",
        changeMonth: true,
        changeYear: true,
        firstDay: 1,
        minDate: moment(`${moment().year() - 1}/${moment().subtract(10, "months").month()}/1`, "Y/M/D").toDate()
    })
        .datepicker("setDate", new Date())
        .change(restrict_to_date);

    to_date.datepicker({
        dateFormat: "M dd, yy",
        changeMonth: true,
        changeYear: true,
        firstDay: 1,
        minDate: moment(`${moment().year() - 1}/${moment().subtract(10, "months").month()}/1`, "Y/M/D").toDate()
    })
        .datepicker("setDate", new Date())
        .change(restrict_from_date);

    match_table.DataTable({
        ordering: false,
        dom: "t<'pagination'<'page-size'lp>i>",
        language: common.language(),
        lengthMenu: [1, 10, 20, 50, 100, 500],
        pageLength: 1,
        scrollY: '65vh',
        scrollCollapse: true,
        columnDefs: [
            { targets: "_all", defaultContent: "-" },
            { targets: [0], className: "text-center", width: "5%" },
            { targets: [2], className: "text-center", width: "7%" },
            { targets: [3], className: "text-center", width: "8%" },
            { targets: [4, 5, 6, 7, 8, 9, 10], className: "text-right" },
            { targets: [4, 5, 6, 7], width: "7%" },
            { targets: [8, 9, 10], width: "7%" },
            { targets: 11, width: "10%" }          
        ],
        columns: [
            { data: obj => `${obj.MatchId}<br/><span style='color:#003399;'>${obj.MatchCode}</span>` },
            { data: obj => `<span class='team-home'>${obj.Home}</span> - vs - <span class='team-away'>${obj.Away}</span><br/><span style='color:#003399;'>${obj.League}</span>`, className: "col-daily-league" },
            { data: obj => moment(obj.EventDate, "MMM D, YYYY HH:mm:ss").format("MMM D, YYYY") },
            { data: obj => moment(obj.ScannedDate, "MMM D, YYYY HH:mm:ss").format("MMM D, YYYY<br/>hh:mm:ss A"), className: "scanned-date" },
            { data: obj => render_history(obj.CompanyTO) },
            { data: obj => render_history(obj.SuperTO) },
            { data: obj => render_history(obj.MasterTO) },
            { data: obj => render_history(obj.AgentTO) },
            { data: obj => render_history(obj.Tickets, true) },
            { data: obj => render_history(obj.SettledTickets, true) },
            { data: obj => render_history(obj.CancelledTickets, true) },
            {
                data: renderNote, className: "col-daily-note text-left", createdCell: (cell, _, row) => {
                    $('.btn-note-change', cell).click(() => handleChangeNote(cell, row));
                    $('.btn-note-ellipsis', cell).click(() => handleExpandNote(cell));
                }
            }          
        ],
        rowCallback: function (cell, row, index) {
            adjustSizeOfNote(cell);            
        },
        drawCallback: function () {
            $('.paginate_button').on('click', function () {
                var isDrawRowTotal = !$(this).hasClass('current') && !$(this).hasClass('disabled');

                if (isDrawRowTotal) {
                    $('tbody > tr:first', match_table).before('<tr><td>Total</td><td></td><td></td><td></td><td>Stuff</td><td>Stuff</td><td>Stuff</td></tr>');
                }
            });
        }
    });

    function renderNote(row) {
        var noteForDisplay = wrapTextForDipslayNote(row.Note, const_note_length_of_collapse);

        if (noteForDisplay == '-') {
            var domElementNoteForDisplay = '<div class="note-content-display note-blank" title=' + row.Note + '>' + noteForDisplay + '</div>';
        } else {
            var domElementNoteForDisplay = '<div class="note-content-display" title=' + row.Note + '>' + noteForDisplay + '</div>';
        }

        return '<div class="container-note">'
            + '<textarea class="txt-note-input hide" data-match-id="' + row.MatchId + '" rows="10">' + (row.Note != '-' ? row.Note : '') + '</textarea>'
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
        match_table.DataTable().columns.adjust();

        adjustHeightNoteInput(domElementsNote);
        handleNoteInputEvents(domElementsNote);
    }

    function adjustHeightNoteInput(domElementsNote) {
        $(domElementsNote.txtNoteInput).height($(domElementsNote.noteForDisplay).height());
    }

    function handleNoteInputEvents(domElementsNote) {
        $(domElementsNote.txtNoteInput).mouseout(function () {
            match_table.DataTable().columns.adjust();
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

        match_table.DataTable().columns.adjust();
    }

    function saveChangedNote(domElementsNote, noteToUpdate, cell) {
        var url = common.modifyRelativeUrl("/DailyDataCapture/UpdateDailyDataCaptureOfNote");
        var paramsUpdate = {};
        paramsUpdate.MatchIds = $(domElementsNote.txtNoteInput).attr('data-match-id');
        paramsUpdate.Note = noteToUpdate;

        common.showLoading();
        $.ajax({
            type: 'POST',
            url: url,
            data: paramsUpdate,
            success: function (response) {
                common.closeLoading();
                if (response.ErrorCode == 0) {
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
                    common.showError(response.ErrorMsg);
                    console.log(response.ErrorMsg);
                }
            },
            error: function (xhr, status, error) {
                console.log('data of ' + xhr.responseText);
                common.showError();
                common.closeLoading();
            }
        });
    }

    function adjustSizeOfNote(cell) {        
        var domElementNoteInput = $(cell).find(".txt-note-input");
        var domElemntNoteBlank = $(cell).find(".note-blank");
        var domElementNoteDisplay = $(cell).find(".note-content-display");       

        if ($(window).width() < 1660) {
            domElementNoteDisplay.css({ 'min-height': '95px'});
            domElementNoteInput.css({ 'min-height': '92px' });
            domElemntNoteBlank.css({ 'padding-top': '37px' });
        } else {
            domElementNoteDisplay.css({ 'min-height': '75px' });
            domElementNoteInput.css({ 'min-height': '73px' });
            domElemntNoteBlank.css({ 'padding-top': '27px' });
        }
    }

    function handleExpandNote(cell) {
        var noteForDisplay = $(cell).find('.txt-note-input').val();

        noteForDisplay += "  " + '<span class="btn-note-collapse icon-chevron-up" title="click to collapse up"></span>';

        $(cell).find(".note-content-display").html(noteForDisplay);

        $('.btn-note-collapse', cell).click(() => handleCollapseNote(cell));
    }

    function handleCollapseNote(cell) {
        var txtNoteInput = $(cell).find('.txt-note-input').val();
        var noteForDisplay = wrapTextForDipslayNote(txtNoteInput, const_note_length_of_collapse);
        $(cell).find(".note-content-display").html(noteForDisplay);

        match_table.DataTable().columns.adjust();
        $(".btn-note-ellipsis", cell).click(() => handleExpandNote(cell));
    }

    function handleSpecialCharacter(str) {
        if (str !== null) {
            return str.replace(/&/g, "&amp;").replace(/>/g, "&gt;").replace(/</g, "&lt;");
        }
        return str;
    }

    function switchScannedDate() {
        inputScannedDate.is(':checked')
            ? lblSwitchScannedDate.text("Scanned Date")
            : lblSwitchScannedDate.text("Event Date");
    }

    $("#generate").click(generate_report);

    $("#export").click(generate_excel);

    $(".icon-calendar-text").click(e => $(e.target).prev().focus());

    inputScannedDate.on('click', function () {
        switchScannedDate();
    });

    generate_report();
}

$(daily_data_capture);