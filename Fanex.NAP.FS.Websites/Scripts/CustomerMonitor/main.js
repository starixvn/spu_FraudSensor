$(function () {
    var customer_grid = $("#customer-grid"),
        gen_btn = $("#generate"),
        from_date = $("#from_date"),
        to_date = $("#to_date"),
        date_type = $("#date_type"),
        timeout_holder = null;

    customer_grid.DataTable({
        language: common.language,
        dom: "t<'pagination'<'page-size'lp>i>",
        searching: false,
        ordering: false,
        lengthMenu: [10, 20, 50, 100, 500],
        pageLength: 50,
        scrollY: "61vh",
        scrollX: true,
        scrollCollapse: true,
        columnDefs: [
            { targets: [0, 2], className: "text-center" },
            { targets: 0, width: "4%" },
            { targets: 1, width: "20%" },
            { targets: 2, width: "10%" }
        ],
        columns: [
            { data: (row, type, set, meta) => meta.row + 1 },
            { data: row => row.Username },
            { data: row => `${row.TicketCount}/${row.ViolationCount}/${row.MatchCount}` },
            { data: renderNote, createdCell: (cell, _, row) => $(".icon-note", cell).click(() => renderNotePopup(row)) }    
        ]
    });

    gen_btn.click(gen_data);

    $("#refresh_btn").click(gen_data);

    common.makeDatePicker(from_date, { dateFormat: "M dd, yy" });

    common.makeDatePicker(to_date, { dateFormat: "M dd, yy" });

    from_date.change(restrict_to_date);

    to_date.change(restrict_from_date);

    date_type.change(restrict_date_range);

    $("#refresh_interval").val(60).change(e => refreshChange($(e.target))).change();

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

    function renderNote(obj) {
        var result = obj.Note;

        if (result == null) {
            result = "";
        }
        else if (result == "-") {
            result = "-" + new Array(42).join(" ");
        }
        else if (result.length > 145) {
            result = common.makeWrapString(result, 142);
        }

        return "<div class='container-note'>"
            + '<div class="note-content">' + result + '</div>'
            + '<div class="icon-note-text icon-note"></div>'
            + "</div>";
    }

    function renderNotePopup(row) {
        $("textarea", "#match-note-popup").val(row.Note == null ? "" : row.Note);

        function updateNote(popup) {
            var note = $("textarea", popup).val();

            if (note != null && note != "") {
                var url = "/CustomerMonitor/UpdateNote";

                $.post(url, { CustId: row.CustId, Notes: note }, function (result) {
                    if (result.ErrorCode == 0) {
                        $(popup).dialog("close");
                        gen_data(false);
                        common.showSuccess("Note updated!");
                    }
                });
            }         
        }

        $("#match-note-popup").clone().dialog({
            modal: true,
            resizable: false,
            buttons: [
                { text: "Submit", class: "button -primary", click: function () { updateNote(this) } }
            ],
            width: 625
        });
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

        from_date.datepicker("option", "minDate", moment(to_val).add(-1, "y").toDate());

        date_type.val(7).change();
    }

    function restrict_to_date() {
        var from_val = from_date.datepicker("getDate");

        to_date.datepicker("option", "minDate", from_val);

        date_type.val(7).change();
    }
});