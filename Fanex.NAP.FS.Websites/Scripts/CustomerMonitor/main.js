$(function () {
    var customer_grid = $("#customer-grid"),
        gen_btn = $("#generate"),
        from_date = $("#from_date"),
        to_date = $("#to_date"),
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

    $("#refresh_interval").val(60).change(e => refreshChange($(e.target))).change();

    function gen_data(full = true) {
        from = moment(from_date.val());
        to = moment(to_date.val());

        if (from > to) {
            common.showError("From is bigger than To");
            return;
        }

        url = "customerMonitor/GetCustomers";

        common.showLoading();

        $.post(url, { FromDate: from.format(), ToDate: to.format() }, function (data) {
            customer_grid.DataTable().clear().rows.add(data).draw(full);
            common.closeLoading();
        });
    }

    function renderNote(obj) {
        var result = obj.Note;

        if (result == "-") {
            result = "-" + new Array(42).join(" ");
        }
        else if (result.length > 40) {
            result = result.substring(0, 27) + "...";
        }

        return "<div class='container-note'>"
            + '<div class="note-content">' + result + '</div>'
            + '<div class="icon-note-text icon-note"></div>'
            + "</div>";
    }

    function renderNotePopup(row) {
        $("textarea", "#match-note-popup").val(row.Note == null ? "" : row.Note);

        function updateNote(popup) {
            $(popup).dialog("close");
            gen_data(false);
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
});