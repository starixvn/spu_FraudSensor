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

        common.showLoading();

        $.get(url, { fromDate: from_date.val(), toDate: to_date.val() }, function (json) {
            match_table.DataTable().clear().rows.add(json).draw();                                
            common.closeLoading();
        });
    }

    function generate_excel() {
        var url = common.modifyRelativeUrl("/DailyDataCapture/GetMatchesExcel");

        common.downloadFile(url, { fromDate: from_date.val(), toDate: to_date.val() });
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
        lengthMenu: [10, 20, 50, 100, 500],
        pageLength: 50,
        scrollY: '70vh',
        scrollCollapse: true,
        columnDefs: [
            { targets: "_all", defaultContent: "-" },
            { targets: [1, 2], className: "text-center", width: "10%" },
            { targets: [3, 4, 5, 6, 7, 8, 9], className: "text-right" },
            { targets: [3, 4, 5, 6], width: "9%" },
            { targets: [7, 8, 9], width: "7%" }
        ],
        columns: [
            { data: obj => `<span class='team-home'>${obj.Home}</span> - vs - <span class='team-away'>${obj.Away}</span>` },
            { data: obj => moment(obj.EventDate, "MMM D, YYYY HH:mm:ss").format("MMM D, YYYY") },
            { data: obj => moment(obj.ScannedDate, "MMM D, YYYY HH:mm:ss").format("MMM D, YYYY<br/>hh:mm:ss A") },
            { data: obj => render_history(obj.CompanyTO) },
            { data: obj => render_history(obj.SuperTO) },
            { data: obj => render_history(obj.MasterTO) },
            { data: obj => render_history(obj.AgentTO) },
            { data: obj => render_history(obj.Tickets, true) },
            { data: obj => render_history(obj.SettledTickets, true) },
            { data: obj => render_history(obj.CancelledTickets, true) },
        ],
    });

    $("#generate").click(generate_report);

    $("#export").click(generate_excel);

    $(".icon-calendar-text").click(e => $(e.target).prev().focus());

    generate_report();
}

$(daily_data_capture);