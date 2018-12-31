function customer_details() {
    var ticket_grid = $("#ticket-details");

    var global_idx = 0;

    var filter = {};

    var totalTicket = 0;

    var ticketLoad = false;

    function load_ticket_grid() {
        common.showLoading();

        global_idx = 0;

        var url = common.modifyRelativeUrl("/CustomerMonitor/GetCustomerTicketDetails");

        $.get(url, parameters, function (json) {
            var table = ticket_grid.DataTable();

            table.clear().rows.add(process_ticket_json(json)).draw();

            var n = json.reduce((a, arr) => a + arr.data.length, 0);

            if (n != 0) {
                $("#DataTables_Table_0_info").text(`Displaying 1 to ${n} of ${n} item(s)`);
            }

            totalTicket = n;

            ticketLoad = true;

            if (ticketLoad) {
                common.closeLoading();
            }
        });
    }

    function process_ticket_json(json) {
        var result = [];

        for (var i = 0; i < json.length; i++) {
            var bt = json[i];

            result.push({ key: true, value: bt.key });

            var data = bt.data;

            for (var j = 0; j < data.length; j++) {
                result.push(data[j]);
            }
        }

        return result;
    }

    function exportExcel() {
        var url = common.modifyRelativeUrl("/CustomerMonitor/ExportExcelForDetails");

        common.downloadFile(url, parameters);
    }

    ticket_grid.DataTable({
        ordering: false,
        paging: false,
        dom: "t<'pagination'<'page-size'lp>i>",
        language: {
            lengthMenu: "Page size _MENU_",
            paginate: {
                previous: "Back"
            },
            info: "Displaying _START_ to _MAX_ of _MAX_ item(s)",
            infoEmpty: "",
            infoFiltered: "",
            thousands: ",",
            zeroRecords: "There's no data"
        },
        scrollY: '57vh',
        scrollCollapse: true,
        columns: [
            { data: obj => obj.key ? obj.value : ++global_idx, className: "text-center", width: "40px" },
            { data: obj => moment(obj.TransDate, "MMM D, YYYY hh:mm:ss A").format("MMM D, YYYY<br/>hh:mm:ss A"), className: "text-center", width: "100px" },
            { data: obj => obj.BetID, width: "100px" },
            { data: obj => obj.Choice, className: "text-right" },
            { data: obj => obj.Score, className: "text-center", width: "40px" },
            { data: obj => obj.Odds != null ? obj.Odds.Html : null, width: "50px", className: "text-right" },
            { data: obj => obj.Stake == null ? null : numeral(obj.Stake).format("0,0.00"), className: "text-right", width: "100px" },
            { data: obj => obj.WinLoss == null ? null : obj.WinLoss >= 0 ? numeral(obj.WinLoss).format("0,0.00") : `<span class='text-red'>${numeral(obj.WinLoss).format('0,0.00')}</span>`, className: "text-right", width: "100px" },
            { data: obj => obj.Status, width: "82px", className: "text-center uppercase" },
            { data: obj => obj.IP, width: "100px" }
        ],
        columnDefs: [{ targets: "_all", defaultContent: "-" }],
        createdRow: function (row, obj) {
            if (obj.key) {
                var td = $(`<td colspan='12' class='group'><span class='icon-chevron-up'></span> ${obj.value}</td>`);

                $(row).empty().html(td).click(e => show_hide_bettype_items(e.target, obj.value));
            } else {
                $(row).attr("data-bettype-item", obj.Bettype);
            }
        },
        headerCallback: tr => $(":nth-child(11)", tr).removeClass("uppercase")
    });

    load_ticket_grid();

    $(".-export").click(exportExcel);
}

$(customer_details);