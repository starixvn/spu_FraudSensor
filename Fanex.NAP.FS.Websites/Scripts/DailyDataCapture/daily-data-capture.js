function daily_data_capture() {
    var from_date = $("#from-date"),
        to_date = $("#to-date"),
        match_table = $("#match-table");

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

        $.get(url, { fromDate: from_date.val(), toDate: to_date.val() }, match_table.DataTable().clear().rows.add);
    }

    function generate_excel() {
        var url = common.modifyRelativeUrl("/DailyDataCapture/GetMatchesExcel");

        common.downloadFile(url, { fromDate: from_date.val(), toDate: to_date.val() });
    }

    function render_history(history) {
        var old_value = history.old;
        var new_value = history.new;

        return `${common.formatGridNumber(old_value)}<br/>${common.formatGridNumber(new_value)}`
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
        scrollY: '57vh',
        scrollCollapse: true,
        columns: [
            { data: obj => obj.match, width: "100px" },
            { data: obj => obj.eventDate, className: "text-center", width: "10px" },
            { data: obj => obj.scannedDate, className: "text-center", width: "100px" },
            { data: obj => render_history(obj.companyTO), width: "100px" },
            { data: obj => render_history(obj.superTO), className: "text-right" },
            { data: obj => render_history(obj.masterTO), className: "text-right" },
            { data: obj => render_history(obj.agentTO), width: "50px", className: "text-right" },
            { data: obj => render_history(obj.tickets), className: "text-right", width: "100px" },
            { data: obj => render_history(obj.settledTickets), className: "text-right", width: "100px" },
            { data: obj => render_history(obj.cancelledTickets), width: "82px", className: "text-center uppercase" },
        ],
        columnDefs: [{ targets: "_all", defaultContent: "-" }],
        headerCallback: tr => $(":nth-child(11)", tr).removeClass("uppercase")
    });

    $("#generate").click(generate_report);

    $("#export").click(generate_excel);
}

$(daily_data_capture);