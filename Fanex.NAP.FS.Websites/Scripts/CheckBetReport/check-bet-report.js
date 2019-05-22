function check_bet_report() {
    var from_date = $("#from-date"),
        to_date = $("#to-date"),
        ticket_table = $("#ticket-table");

    var noteMaxLength = 500;
    var noteLengthCollapse = 180;    

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
        scrollX: true,
        scrollCollapse: true,
        columnDefs: [
            { targets: "_all", defaultContent: "-" },
            { targets: [-2, 1, 2], className: "text-center"},
            { targets: [5, 8], className: "text-right" },       
            { targets: [11], className: "col-note" },    
            { targets: [-2], width: "1%" },
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
                    $(".note-ellipsis", cell).click(() => expandNote(cell));
                }
            }
        ]
    });    

    function renderNote(row) {
        var noteWrap = wrapString(row.Note, noteLengthCollapse);

        return '<div class="container-note">'
            + '<textarea class="input-note-content hide" rows="5">' + (row.Note != null ? row.Note : '') + '</textarea>'
            + '<div class="note-content" id="note-content">' + noteWrap + '</div>'
            + '<div class="icon-note-text icon-note" id="icon"></div>'
            + '</div>';
    }

    function wrapString(str, maxLength) {

        if (str == null || str == '') {
            return '-';
        } 

        var newStr = '';        
        var breaksStr = str.split('\n');
      
        for (var i = 0; i < breaksStr.length; i++) {

            if (i == 4) {
                newStr = newStr + breaksStr[i];
                return newStr.substring(0, maxLength - (i * 2) - 3) + '<span class="note-ellipsis" title="click to see whole note"> ... </span>';
            }

            newStr = newStr + breaksStr[i] + "\n";
            if (newStr.length > maxLength) {
                return newStr.substring(0, maxLength - (i * 2) - 3) + '<span class="note-ellipsis" title="click to see whole note"> ... </span>';
            } 
        }

        return str;
    }

    function splitString(inputNote, str, maxLength) {       
        if (str != null && str.length > maxLength) {

            var strSplited = str.substring(0, maxLength);

            $(inputNote).val(strSplited);

            return strSplited;
        }

        return str;
    }

    function replaceSpecialCharacter(str) {
        if (str != null) {
            return str.replace(/&/g, "&amp;").replace(/>/g, "&gt;").replace(/</g, "&lt;");
        }
        return str;
    }

    function expandNote(cell) {
        var noteExpand = $(cell).find('.input-note-content').val();

        var noteExpand = noteExpand + "  " + '<span class="note-collapse icon-chevron-up" title="click to collapse up"></span>';

        $(cell).find("#note-content").html(noteExpand);

        $(".note-collapse", cell).click(() => collapseNote(cell));

        ticket_table.DataTable().columns.adjust();
    }

    function collapseNote(cell) {
        var noteCollapse = $(cell).find('.input-note-content').val();
        noteCollapse = wrapString(noteCollapse, noteLengthCollapse);

        $(cell).find("#note-content").html(noteCollapse);

        $(".note-ellipsis", cell).click(() => expandNote(cell));

        ticket_table.DataTable().columns.adjust();
    }

    function renderNoteEdit(cell, row) {
        var inputNoteContent = $(cell).find('.input-note-content');
        var noteContent = $(cell).find("#note-content");
        var noteIcon = $(cell).find("#icon");
      
        if ($(inputNoteContent).hasClass('hide')) {
            $(inputNoteContent).removeClass('hide').focus().select();
            $(noteContent).addClass('hide'); 
            $(noteIcon).removeClass('icon-note-text').addClass('icon-note-save').attr('title', 'save');

            $(inputNoteContent).keyup(function () {               
                $(inputNoteContent).addClass('change');
            });

        } else if ($(noteIcon).hasClass('icon-note-save')){
            saveNote(cell, row);
        }
    }

    function saveNote(cell, row) {

        var inputContent = $(cell).find('.input-note-content');        
        var content = $(cell).find("#note-content");
        var noteNew = $(inputContent).val().trim(); 
        noteNew = replaceSpecialCharacter(noteNew);

        if ($(inputContent).hasClass('change')) {
            var url = common.modifyRelativeUrl("/CheckBetReport/UpdateNote");
            noteNew = splitString(inputContent, noteNew, noteMaxLength);

            $.get(url, { Note: noteNew, TransId: row.BetId }, function (result) {
                if (result.ErrorCode == 0) {
                    // wrap text on Note
                    var noteWrap = wrapString(noteNew, noteLengthCollapse);
                    $(content).html(noteWrap);

                    $(".note-ellipsis", cell).click(() => expandNote(cell));

                    common.showSuccess('Note updated!');

                } else {
                    console.log(result.ErrorCode);

                    common.showError('Note fail!');
                }               
            });
        }

        $(inputContent).removeClass('change').addClass('hide');
        $(content).removeClass('hide');
        
        $(cell).find("#icon").removeClass('icon-note-save').addClass('icon-note-text').attr('title', 'edit');
    }

    $("#generate").click(generate_report);

    $("#export").click(generate_excel);

    $(".icon-calendar-text").click(e => $(e.target).prev().focus());

    generate_report();
}

$(check_bet_report);