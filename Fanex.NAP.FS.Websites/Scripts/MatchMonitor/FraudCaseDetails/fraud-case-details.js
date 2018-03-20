function fraud_case_details() {
    var ticket_grid = $("#ticket-tab table"),
        detail_tabs = $("#detailTabs"),
        customer_grid = $("#customer-tab table");

    var global_idx = 0,
        first_time = true;

    var filter = {};

    function load_ticket_grid() {
        global_idx = 0;

        var url = common.modifyRelativeUrl("/MatchMonitor/GetFraudCaseTicketDetails");

        $.get(url, parameters, function (json) {
            var table = ticket_grid.DataTable();

            table.clear().rows.add(process_ticket_json(json)).draw();

            $("#tickets").text(`By Tickets (${json.reduce((a, arr) => a + arr.length, 0)})`)
        });
    }

    function load_customer_grid() {
        var url = common.modifyRelativeUrl("/MatchMonitor/GetFraudCaseCustomerDetails");

        $.get(url, parameters, function (json) {
            customer_grid.DataTable().clear().rows.add(json).draw();

            $("#customers").text(`By Customers (${json.length})`);
        });
    }

    function process_ticket_json(json) {
        var result = [];

        for (var bt in json) {
            result.push({ key: true, value: bt });

            var data = json[bt];

            for (var i = 0; i < data.length; i++) {
                result.push(data[i]);
            }
        }

        return result;
    }

    function show_hide_bettype_items(bettype_value) {
        var table = ticket_grid.DataTable();

        if (filter[bettype_value] != null) {
            $.fn.dataTable.ext.search[filter[bettype_value]] = () => true;
            filter[bettype_value] = null;
        }
        else {
            var n = $.fn.dataTable.ext.search.push(function (settings, data, idx) {
                return $(table.row(idx).node()).attr("data-bettype-item") != bettype_value;
            });

            filter[bettype_value] = n - 1;
        }

        table.draw(false);
    }

    function switchTab() {
        if (first_time) {
            customer_grid.DataTable().draw();
            customer_grid.DataTable().draw();

            first_time = false;
        }
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
            infoFiltered: ""
        },
        autoWidth: false,
        scrollY: '57vh',
        columns: [
            { data: obj => obj.key ? obj.value : ++global_idx, className: "text-center"  },
            { data: obj => obj.Member },
            { data: obj => obj.TransDate, className: "text-center" },
            { data: obj => obj.BetID },
            { data: obj => obj.Choice },
            { data: obj => obj.Score, className: "text-center" },
            { data: obj => obj.Odds },
            { data: obj => obj.Stake == null ? null : obj.Stake.toFixed(2) },
            { data: obj => obj.WinLoss == null ? null : obj.WinLoss.toFixed(2) },
            { data: obj => obj.Status, className: "text-center" },
            { data: obj => obj.IP }
        ],
        columnDefs: [{ targets: "_all", defaultContent: "-" }],
        createdRow: function (row, obj) {
            if (obj.key) {
                var td = $(`<td colspan='11' class='group'>${obj.value}</td>`);

                $(row).empty().html(td).click(() => show_hide_bettype_items(obj.value));
            } else {
                $(row).attr("data-bettype-item", obj.Bettype);
            }
        },
    });

    customer_grid.DataTable({
        ordering: false,
        searching: false,
        paging: false,
        dom: "t<'pagination'<'page-size'lp>i>",
        language: {
            lengthMenu: "Page size _MENU_",
            paginate: {
                previous: "Back"
            },
            info: "Displaying _START_ to _END_ of _TOTAL_ item(s)",
            infoEmpty: ""
        },
        autoWidth: false,
        scrollY: '57vh',
        columns: [
            { data: (obj, type, set, meta) => meta.row + 1, className: "text-center" },
            { data: obj => obj.Member },
            { data: obj => obj.Tickets },
            { data: obj => obj.TotalStake.toFixed(2) },
            { data: obj => obj.TotalWinLoss.toFixed(2) },
            { data: obj => obj.Evidence },
            { data: obj => obj.AssociatedAccounts },
            { defaultContent: '<span class="icon-settings icon-config"></span>', className: "has-icon text-center" }
        ]
    })

    load_ticket_grid();

    load_customer_grid();

    detail_tabs.tabs({
        activate: (e, ui) => switchTab(ui)
    });
}

$(fraud_case_details)