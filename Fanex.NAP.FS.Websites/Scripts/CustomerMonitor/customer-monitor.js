$(function () {
    var customer_grid = $("#customer-grid"),
        gen_btn = $("#generate"),
        from_date = $("#from_date"),
        to_date = $("#to_date"),
        date_type = $("#date_type"),
        timeout_holder = null;

    var noteMaxLength = 500;
    var noteLengthCollapse = 180;    

    customer_grid.DataTable({
        dom: "t<'pagination'<'page-size'lp>i>",
        language: {
            lengthMenu: "Page size _MENU_",
            paginate: {
                previous: "Back"
            },
            info: "Displaying _START_ to _END_ of _TOTAL_ item(s)",
            infoEmpty: "",
            zeroRecords: "There's no data"
        },
        searching: false,
        ordering: false,
        lengthMenu: [10, 20, 50, 100, 500],
        pageLength: 50,
        scrollY: "61vh",
        scrollX: true,
        scrollCollapse: true,
        columnDefs: [
            { targets: [0, 2, 3], className: "text-center" },
            { targets: 0, width: "40px" },
            { targets: 1, width: "185px" },
            { targets: 2, width: "90px" },
            { targets: 3, width: "90px" },
            { targets: 4, width: "190px" },
            { targets: 5, className: "col-note" }
        ],
        columns: [
            { data: (row, type, set, meta) => meta.row + 1 },
            { data: row => row.Username },
            { data: row => row.TotalTicket },
            { data: row => renderViolation(row) },
            { data: row => renderRemark(row) },
            {
                data: renderNote, createdCell: (cell, _, row) => {
                    $(".icon-note", cell).click(() => renderNoteEdit(cell, row));
                    $(".note-ellipsis", cell).click(() => expandNote(cell));
                }
            }    
        ],
        headerCallback: function (thread) {
            $("th", thread).eq(3).attr("title", "[Ticket Count] / [Violation Count] / [Match Count]");
        }        
    });

    from_date.datepicker({
        dateFormat: "M dd, yy",
        minDate: balanceUpDay,
        changeMonth: true,
        changeYear: true,
        firstDay: 1       
    });

    to_date.datepicker({
        dateFormat: "M dd, yy",
        minDate: balanceUpDay,
        changeMonth: true,
        changeYear: true,
        firstDay: 1
    });

    gen_btn.click(gen_data);

    date_type.select2();

    $("#refresh_btn").click(gen_data);

    from_date.change(restrict_to_date);

    to_date.change(restrict_from_date);

    date_type.change(restrict_date_range);

    $("#refresh_interval").val(60).change(e => refreshChange($(e.target))).change();

    $("#export").click(export_excel);
       
    function renderViolation(obj) {
        fromDate = moment(from_date.val()).format("MMM D, YYYY");
        toDate = moment(to_date.val()).format("MMM D, YYYY");
        return `<a class='info-bng' href='CustomerMonitor/GetCustomerTicketDetails?CustId=${obj.CustId}&Username=${obj.Username}&FromDate=${fromDate}&ToDate=${toDate}&TicketCount=${obj.TicketCount}&ViolationCount=${obj.ViolationCount}&MatchCount=${obj.MatchCount}' target='_blank'>
            ${obj.TicketCount}/${obj.ViolationCount}/${obj.MatchCount}
        </a>`;
    }

    function renderRemark(obj) {
        var remark = [];

        if (obj.TicketWON > 0) {
            remark.push(obj.TicketWON + ' WON');
        }
        if (obj.TicketReject > 0) {
            remark.push(obj.TicketReject + ' Reject');
        }
        if (obj.TicketVoid > 0) {
            remark.push(obj.TicketVoid + ' Void');
        }
        
        return remark.join(', ');
    }

    function gen_data(full = true) {
        from = moment(from_date.val());
        to = moment(to_date.val());

        url = "customerMonitor/GetCustomers";

        common.showLoading();

        $.post(url, { FromDate: from.format("MMM D, YYYY"), ToDate: to.format("MMM D, YYYY") }, function (data) {
            customer_grid.DataTable().clear().rows.add(data).draw(full);
            common.closeLoading();
        });
    }

    function renderNote(row) {
        var noteWrap = wrapString(row.Note, noteLengthCollapse);

        return '<div class="container-note">'
            + '<textarea class="input-note-content hide" rows="5">' + (row.Note != null ? row.Note : '') + '</textarea>'
            + '<div class="note-content" id="note-content">' + noteWrap + '</div>'
            + '<div class="icon-note-text icon-note" id="icon" title="edit"></div>'
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

        customer_grid.DataTable().columns.adjust();
    }

    function collapseNote(cell) {
        var noteCollapse = $(cell).find('.input-note-content').val();
        noteCollapse = wrapString(noteCollapse, noteLengthCollapse);

        $(cell).find("#note-content").html(noteCollapse);

        $(".note-ellipsis", cell).click(() => expandNote(cell));

        customer_grid.DataTable().columns.adjust();
    }

    function renderNoteEdit(cell, row) {
        var inputNoteContent = $(cell).find('.input-note-content');
        var noteContent = $(cell).find("#note-content");
        var noteIcon = $(cell).find("#icon");
        var heightNote = $(noteContent).height();

        if ($(inputNoteContent).hasClass('hide')) {
            $(inputNoteContent).removeClass('hide').focus().select();

            // adjust height input Note
            $(inputNoteContent).height(heightNote + 4);

            $(noteContent).addClass('hide').hide();
            $(noteIcon).removeClass('icon-note-text').addClass('icon-note-save').attr('title', 'save');

            $(inputNoteContent).mouseout(function () {
                customer_grid.DataTable().columns.adjust();
            });

            $(inputNoteContent).keyup(function () {
                $(inputNoteContent).addClass('change');
            });

        } else if ($(noteIcon).hasClass('icon-note-save')) {
            saveNote(cell, row);
        }
    }

    function saveNote(cell, row) {    
        var inputContent = $(cell).find('.input-note-content');
        var content = $(cell).find("#note-content");
        var noteNew = $(inputContent).val().trim();
        noteNew = replaceSpecialCharacter(noteNew);

        if ($(inputContent).hasClass('change')) {
            var url = common.modifyRelativeUrl("/CustomerMonitor/UpdateNote");
            noteNew = splitString(inputContent, noteNew, noteMaxLength);

            $.post(url, { Notes: noteNew, CustId: row.CustId }, function (result) {
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
        $(content).removeClass('hide').show();

        customer_grid.DataTable().columns.adjust();
        $(cell).find("#icon").removeClass('icon-note-save').addClass('icon-note-text').attr('title', 'edit');
    }

    function startCountingDown($target, originalVal, val) {
        if (val != 0) {
            $target.text(val);

            timeout_holder = setTimeout(() => startCountingDown($target, originalVal, val - 1), 1000);
        }
        else {
            gen_btn.click();
            startCountingDown($target, originalVal, originalVal);
        }
    }

    function refreshChange($target) {
        var selectedVal = parseInt($target.val());
        var countdown = $("#refresh_countdown");
        clearTimeout(timeout_holder);

        if (isNaN(selectedVal)) {
            countdown.text("-");
        }
        else {
            countdown.text(selectedVal);
            startCountingDown(countdown, selectedVal, selectedVal);
        }
    }

    function restrict_date_range() {
        var type = parseInt(date_type.val());

        var from, to;

        if (type == 0) {
            from = moment().toDate();
            to = from;
        } //today
        else if (type == 1) {
            var m = moment();

            m.set("date", m.get("date") - 1);

            from = m.toDate();
            to = from;
        } // yesterday
        else if (type == 2) {
            from = moment().day(1).toDate();
            to = moment().day(7).toDate();
        } // this week
        else if (type == 3) {
            from = moment().day(-6).toDate();
            to = moment().day(0).toDate();
        } // last week
        else if (type == 4) {
            from = moment().date(1).toDate();
            to = moment().date(moment().daysInMonth()).toDate();
        } // this month
        else if (type == 5) {
            var m = moment();

            m.set("date", 1);
            m.set("month", m.get("month") - 1);

            from = m.toDate();

            to = m.date(m.daysInMonth()).toDate();
        } // last month
        else if (type == 6) {
            var m = moment();

            m.set("date", 1);
            m.set("month", m.get("month") - 1);

            from = m.toDate();
            to = moment().toDate();
        } // since last month

        if (from != null && to != null) {
            from_date.datepicker("setDate", from);
            to_date.datepicker("setDate", to);
            from_date.datepicker("option", "maxDate", null);
            to_date.datepicker("option", "minDate", "-1y");
        }
    }

    function restrict_from_date() {
        var to_val = to_date.datepicker("getDate");

        from_date.datepicker("option", "maxDate", to_val);

        var minDate = moment(to_val).add(-1, "y").toDate();

        if (minDate < balanceUpDay) {
            minDate = balanceUpDay;
        }

        from_date.datepicker("option", "minDate", minDate);

        date_type.val(7).change();
    }

    function restrict_to_date() {
        var from_val = from_date.datepicker("getDate");
        
        if (from_val < balanceUpDay) {
            from_val = balanceUpDay;
        }

        to_date.datepicker("option", "minDate", from_val);

        date_type.val(7).change();
    }

    function export_excel() {
        from = moment(from_date.val());
        to = moment(to_date.val());

        url = "customerMonitor/exportexcel";

        common.downloadFile(url, { FromDate: from.format("MMM D, YYYY"), ToDate: to.format("MMM D, YYYY") });
    }
});