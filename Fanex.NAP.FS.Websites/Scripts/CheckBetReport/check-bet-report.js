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
        scrollY: '65vh',
        scrollCollapse: true,
        columnDefs: [
            { targets: "_all", defaultContent: "-" },
            { targets: [-2, 1, 2], className: "text-center"},
            { targets: [5, 8], className: "text-right" },
            { targets: [0], width: "5%" },
            { targets: [-2, 2], width: "4%" },
            { targets: [1, 5, 6, 8], width: "8%" },
            { targets: [-1, 3, 4], width: "10%" }
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
        ],
        createdRow: function (row, obj) {
            if (group != null && obj.Group != group) {
                highlight = !highlight;
            }

            if (highlight) {
                $(row).addClass("odd-group-db");
            }

            group = obj.Group;
        }
    });

    $("#generate").click(generate_report);

    $("#export").click(generate_excel);

    $(".icon-calendar-text").click(e => $(e.target).prev().focus());

    generate_report();
}

$(check_bet_report);