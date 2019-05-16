function check_bet_report() {
    var from_date = $("#from-date"),
        to_date = $("#to-date"),
        ticket_table = $("#ticket-table");

    function restrict_from_date() {
        var to_val = to_date.datepicker("getDate");

        from_date.datepicker("option", "maxDate", to_val);
    }

    function restrict_to_date() {
        var from_val = from_date.datepicker("getDate");

        to_date
            .datepicker("option", "minDate", from_val)
            .datepicker("option", "maxDate", moment(from_val).add(1, "year").toDate());
    }

    function generate_report() {
        group = null;
        highlight = false;

        var url = common.modifyRelativeUrl("/CheckBetReport/GenerateReport");

        common.showLoading();

        $.get(url, { fromDate: from_date.val(), toDate: to_date.val() }, function (json) {
            ticket_table.DataTable().clear().rows.add(json).draw();
            ticket_table.DataTable().columns.adjust();
            common.closeLoading();
        });
    }

    function generate_excel() {
        var url = common.modifyRelativeUrl("/CheckBetReport/GenerateExcel");

        common.downloadFile(url, { fromDate: from_date.val(), toDate: to_date.val() });
    }

    function render_history(history, isInt, isString) {
        var old_value = history.Old;
        var new_value = history.New;

        if (old_value == new_value) {
            if (isInt) {
                return common.formatGridNumber(old_value, "0,0");
            }

            if (isString) {
                return old_value;
            }

            return common.formatGridNumber(old_value);
        }

        if (isInt) {
            return `${old_value != null ? common.formatGridNumber(old_value, "0,0") : "-"}<br/><span class='new-value'>${new_value != null ? common.formatGridNumber(new_value, "0,0") : "-"}</span>`;
        }

        if (isString) {
            return `${old_value != null && old_value.length != 0 ? old_value : "-"}<br/><span class='new-value'>${new_value != null && new_value.length != 0 ? new_value : "-"}</span>`;
        }

        return `${old_value != null ? common.formatGridNumber(old_value) : "-"}<br/><span class='new-value'>${new_value != null ? common.formatGridNumber(new_value) : "-"}</span>`
    }

    var group = null;

    var highlight = false;

    from_date.datepicker({
        dateFormat: "M dd, yy",
        changeMonth: true,
        changeYear: true,
        firstDay: 1,
    })
        .datepicker("setDate", new Date())
        .change(restrict_to_date);

    to_date.datepicker({
        dateFormat: "M dd, yy",
        changeMonth: true,
        changeYear: true,
        firstDay: 1,
    })
        .datepicker("setDate", new Date())
        .change(restrict_from_date);

    ticket_table.DataTable({
        ordering: false,
        paging: false,
        dom: "t<'pagination'<'page-size'lp>i>",
        language: common.language(),
        scrollY: "61vh",
        scrollCollapse: true,
        columnDefs: [
            { targets: "_all", defaultContent: "-" },
            { targets: [-2, 1, 2], className: "text-center"},
            { targets: [5, 8], className: "text-right" },       
            { targets: [11], className: "col-note" },            
            { targets: [0], width: "6%" },
            { targets: [1], width: "6%" },
            { targets: [2], width: "5%" },          
            { targets: [3], width: "7%" },
            { targets: [4], width: "9%" },
            { targets: [5], width: "8%" },
            { targets: [6], width: "8%" },
            { targets: [7], width: "10%" },
            { targets: [8], width: "8%" },
            { targets: [9], width: "8%" },
            { targets: [10], width: "9%" }            
        ],
        columns: [
            { data: obj => obj.BetId },
            { data: obj => moment(obj.TransDate, "MMM D, YYYY HH:mm:ss").format("MMM D, YYYY<br/>hh:mm:ss A") },
            { data: obj => obj.Currency },
            { data: obj => obj.Bettype },
            { data: obj => obj.Username },
            { data: obj => render_history(obj.Stake) },
            { data: obj => render_history(obj.Odds, false, true) },
            { data: obj => render_history(obj.Choice, false, true) },
            { data: obj => render_history(obj.PoistionTaking) },
            { data: obj => render_history(obj.Status, false, true) },
            { data: obj => obj.Creator },            
            {
                data: renderNote, createdCell: (cell, _, row) => {
                    $(".icon-note", cell).click(() => renderNoteEdit(cell, row));
                    $(".note-ellipsis", cell).click(() => expandNote(cell))
                }
            }
        ]
    });    

    function renderNote(row) {
        var noteWrap = wrapString(row.Note, 100);

        return '<div class="container-note">'
            + '<textarea class="input-note-content hide" rows="5">' + (row.Note != null ? row.Note : '') + '</textarea>'
            + '<div class="note-content" id="note-content">' + noteWrap + '</div>'
            + '<div class="icon-note-text icon-note" id="icon"></div>'
            + '</div>';
    }

    function wrapString(str, maxLength) {
        if (str == null) {
            return '';
        } 

        if (str.length > maxLength) {
            return str.substring(0, maxLength - 3) + '<span class="note-ellipsis" title="click to see whole note"> ... </span>';
        }

        return str;
    }

    function expandNote(cell) {
        var inputNoteContent = $(cell).find('.input-note-content');
        var noteContent = $(cell).find("#note-content");
        
        $(noteContent).html($(inputNoteContent).val());
        ticket_table.DataTable().columns.adjust();
    }

    function renderNoteEdit(cell, row) {
        var inputNoteContent = $(cell).find('.input-note-content');
        var noteContent = $(cell).find("#note-content");
        var noteIcon = $(cell).find("#icon");

        var noteContentOld = $(noteContent).text();

        if ($(inputNoteContent).hasClass('hide')) {
            $(inputNoteContent).removeClass('hide').focus().select();
            $(noteContent).addClass('hide');

            // Change icon
            $(noteIcon).removeClass('icon-note-text').addClass('icon-note-save');

            var updated;        
            var noteContentNew;
            // Handle key up event
            $(inputNoteContent).keyup(function (event) {
                noteContentNew = $(this).val();               
            });

            $(noteIcon).click(function () {
                if ($(inputNoteContent).hasClass('submit') && !updated) {
                    if (typeof (noteContentNew) === 'undefined') {
                        $(inputNoteContent).addClass('hide').removeClass('submit');
                        $(noteContent).removeClass('hide');

                    } else if (noteContentOld !== noteContentNew) {
                        // Update note to database
                        var url = "/CheckBetReport/UpdateNote";

                        $.get(url, { Note: noteContentNew, TransId: row.BetId }, function (result) {
                            if (result.ErrorCode == 0) {
                                // Render new note content
                                var noteWrap = wrapString(noteContentNew, 100);
                                $(noteContent).html(noteWrap);
                                $(".note-ellipsis", cell).click(() => expandNote(cell));                               

                                common.showSuccess('Note updated!');

                            } else {
                                common.showError('Note fail!');
                            }
                            $(inputNoteContent).addClass('hide').removeClass('submit');
                            $(noteContent).removeClass('hide');

                            updated = true;
                        });
                    }   
                    // Change icon
                    $(noteIcon).removeClass('icon-note-save').addClass('icon-note-text');
                }
            });
        } else {
            $(inputNoteContent).addClass('submit');
            updated = false;
        }
    }

    $("#generate").click(generate_report);

    $("#export").click(generate_excel);

    $(".icon-calendar-text").click(e => $(e.target).prev().focus());

    generate_report();
}

$(check_bet_report);