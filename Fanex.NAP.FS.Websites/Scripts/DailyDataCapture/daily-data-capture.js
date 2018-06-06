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

    //match_table.DataTable({
    //    ordering: false,
    //    dom: "t<'pagination'<'page-size'lp>i>",
    //    language: common.language(),
    //    scrollY: '57vh',
    //    scrollCollapse: true,
    //    columns: [
    //        { data: (row, type, set, meta) => meta.row + 1, className: "text-center", width: "40px" },
    //        { data: obj => obj.Member, width: "100px" },
    //        { data: obj => obj.CurrencyName, className: "text-center", width: "10px" },
    //        { data: obj => moment(obj.TransDate, "MMM D, YYYY hh:mm:ss A").format("MMM D, YYYY<br/>hh:mm:ss A"), className: "text-center", width: "100px" },
    //        { data: obj => obj.BetID, width: "100px" },
    //        { data: obj => obj.Choice, className: "text-right" },
    //        { data: obj => obj.Odds != null ? obj.Odds.Html : null, width: "50px", className: "text-right" },
    //        { data: obj => common.formatGridNumber(obj.Stake), className: "text-right", width: "100px" },
    //        { data: obj => common.formatGridNumber(obj.WinLoss), className: "text-right", width: "100px" },
    //        { data: obj => obj.Status, width: "82px", className: "text-center uppercase" },
    //        { data: obj => `<span class='link ip-link'>${obj.IP}</span>`, width: "100px" }
    //    ],
    //    columnDefs: [{ targets: "_all", defaultContent: "-" }],
    //    headerCallback: tr => $(":nth-child(11)", tr).removeClass("uppercase")
    //});

    $("#generate").click(generate_report);

    $("#export").click(generate_excel);
}

$(daily_data_capture);