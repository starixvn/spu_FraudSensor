function scanned_office_ip() {
    var from_date = $("#from-date"),
        to_date = $("#to-date"),
        ticket_table = $("#ticket-table");

    function restrict_from_date() {
        var to_val = to_date.datepicker("getDate");

        from_date.datepicker("option", "maxDate", to_val);
    }

    function restrict_to_date() {
        var from_val = from_date.datepicker("getDate");

        to_date.datepicker("option", "minDate", from_val);
    }

    function generate_report() {
        var url = common.modifyRelativeUrl("/ScannedOfficeIpReport/GetTickets");

        common.showLoading();

        $.get(url, { fromDate: from_date.val(), toDate: to_date.val() }, function (json) {
            ticket_table.DataTable().clear().rows.add(json).draw();

            common.closeLoading();
        });
    }

    function generate_excel() {
        var url = common.modifyRelativeUrl("/ScannedOfficeIpReport/ExportExcel");

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

    ticket_table.DataTable({
        ordering: false,
        dom: "t<'pagination'<'page-size'lp>i>",
        language: common.language(),
        scrollY: '70vh',
        scrollCollapse: true,
        lengthMenu: [10, 20, 50, 100, 500],
        pageLength: 50,
        columns: [
            { data: (row, type, set, meta) => meta.row + 1, className: "text-center", width: "40px" },
            { data: obj => obj.Member, width: "100px" },
            { data: obj => obj.CurrencyName, className: "text-center", width: "10px" },
            { data: obj => moment(obj.TransDate, "MMM D, YYYY hh:mm:ss A").format("MMM D, YYYY<br/>hh:mm:ss A"), className: "text-center", width: "100px" },
            { data: obj => obj.BetID, width: "100px" },
            { data: obj => obj.Choice, className: "text-right" },
            { data: obj => obj.Odds != null ? obj.Odds.Html : "-", width: "50px", className: "text-right" },
            { data: obj => obj.Stake != 0 ? common.formatGridNumber(obj.Stake) : "-", className: "text-right", width: "100px" },
            { data: obj => common.formatGridNumber(obj.WinLoss), className: "text-right", width: "100px" },
            { data: obj => obj.Status, width: "82px", className: "text-center uppercase" },
            { data: obj => obj.IP, width: "100px" }
        ],
        columnDefs: [{ targets: "_all", defaultContent: "-" }],
        headerCallback: tr => $(":nth-child(10)", tr).removeClass("uppercase")
    });

    $("#generate").click(generate_report);

    $("#export").click(generate_excel);

    generate_report();

    $(".icon-calendar-text").click(e => $(e.target).prev().focus());
}

$(scanned_office_ip);