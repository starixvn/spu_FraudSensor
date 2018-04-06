function fraud_case_details() {
    var ticket_grid = $("#ticket-tab table"),
        detail_tabs = $("#detailTabs"),
        customer_grid = $("#customer-tab table")
        associated_accounts = $("#associated-accounts");

    var global_idx = 0;

    var filter = {};

    var totalTicket = 0;

    function load_ticket_grid() {
        global_idx = 0;

        var url = common.modifyRelativeUrl("/MatchMonitor/GetFraudCaseTicketDetails");

        $.get(url, parameters, function (json) {
            var table = ticket_grid.DataTable();

            table.clear().rows.add(process_ticket_json(json)).draw();

            var n = json.reduce((a, arr) => a + arr.data.length, 0);

            $("#tickets").text(`By Tickets (${n})`);

            if (n != 0) {
                $("#DataTables_Table_0_info").text(`Displaying 1 to ${n} of ${n} item(s)`);
            }

            totalTicket = n;
            //removeMatchLeague();
        });
    }

    function load_customer_grid() {
        var url = common.modifyRelativeUrl("/MatchMonitor/GetFraudCaseCustomerDetails");

        $.get(url, parameters, function (json) {
            customer_grid.DataTable().clear().rows.add(json).draw();

            $("#customers").text(`By Customers (${json.length})`);

            $(".action").selectmenu({
                classes: {
                    "ui-selectmenu-button": "ui-button-icon-only demo-splitbutton-select"
                },
                position: { my: "left top", at: "left bottom", collision: "flip" },
                select: function () {
                    var tr = $(this).parents("tr");

                    var obj = customer_grid.DataTable().row(tr).data();

                    if (this.value == "2") {
                        renderAssociatedAccounts(obj, []);
                    }
                }
            });

            $(".group-action").controlgroup();

            $(".group-action button").click(e => $(e.target).next().selectmenu("open"));
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

    function show_hide_bettype_items(target, bettype_value) {
        var table = ticket_grid.DataTable();

        if (filter[bettype_value] != null) {
            $.fn.dataTable.ext.search[filter[bettype_value]] = () => true;
            filter[bettype_value] = null;

            if ($(target).is("span")) {
                $(target).removeClass("icon-chevron-down").addClass("icon-chevron-up");
            } else {
                $("span", target).removeClass("icon-chevron-down").addClass("icon-chevron-up");
            }
        }
        else {
            var n = $.fn.dataTable.ext.search.push(function (settings, data, idx) {
                return $(table.row(idx).node()).attr("data-bettype-item") != bettype_value;
            });

            if ($(target).is("span")) {
                $(target).addClass("icon-chevron-down").removeClass("icon-chevron-up");
            }
            else {
                $("span", target).addClass("icon-chevron-down").removeClass("icon-chevron-up");
            }
            
            filter[bettype_value] = n - 1;
        }

        table.draw(false);

        $("#DataTables_Table_0_info").text(`Displaying 1 to ${totalTicket} of ${totalTicket} item(s)`);
    }

    function switchTab(ui) {
        $.fn.dataTable.tables({ visible: true, api: true }).columns.adjust();
    }

    function exportExcel() {
        var url = common.modifyRelativeUrl("/MatchMonitor/ExportExcelForDetail");

        common.downloadFile(url, parameters);
    }

    function removeMatchLeague() {
        $(".match, .league").remove();
    }

    function renderCustomerAction() {
        var options = [
            "<option value='0'>View Suspicious History</option>",
            "<option value='1'>View tickets of last 7 days</option>",
            "<option value='2'>View Associated Accounts</option>"
        ]

        return `<div class='group-action'><button class='hide'></button><select class='action'>${options.join("")}</select></div>`;
    }

    function renderAssociatedAccounts(obj, json) {
        associated_accounts
            .dialog("option", "title", obj.Member)
            .dialog("open");

        $("table", associated_accounts).DataTable().clear().rows.add(json).draw();
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
            { data: obj => obj.Member, width: "100px" },
            { data: obj => obj.CurrencyName, className: "text-center", width: "10px" },
            { data: obj => moment(obj.TransDate).format("MMM D, YYYY<br/>hh:mm:ss A"), className: "text-center", width: "100px" },
            { data: obj => obj.BetID, width: "100px" },
            { data: obj => obj.Choice },
            { data: obj => obj.Score, className: "text-center", width: "40px" },
            { data: obj => obj.Odds != null ? obj.Odds.Html : null, width: "50px" },
            { data: obj => obj.Stake == null ? null : numeral(obj.Stake).format("0,0.00"), className: "text-right", width: "100px" },
            { data: obj => obj.WinLoss == null ? null : obj.WinLoss >= 0 ? numeral(obj.WinLoss).format("0,0.00") : `<font color='#B50000'>${numeral(obj.WinLoss).format('0,0.00')}</font>`, className: "text-right", width: "100px" },
            { data: obj => obj.Status, width: "60px", className: "text-center" },
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
        }
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
            infoEmpty: "",
            zeroRecords: "There's no data"
        },
        scrollY: '57vh',
        scrollCollapse: true,
        columns: [
            { data: (obj, type, set, meta) => meta.row + 1, className: "text-center", width: "40px" },
            { data: obj => obj.Member },
            { data: obj => obj.CurrencyName, className: "text-center", width: "20px" },
            { data: obj => obj.Tickets, className: "text-right", width: "40px" },
            { data: obj => numeral(obj.TotalStake).format("0,0.00"), className: "text-right" },
            { data: obj => obj.TotalWinLoss >= 0 ? numeral(obj.TotalWinLoss).format("0,0.00") : `<font color='#B50000'>${numeral(obj.TotalWinLoss).format("0,0.00")}</font>`, className: "text-right" },
            { data: obj => obj.Evidence != null && obj.Evidence.length > 78 ? `<span title='${obj.Evidence}'>${obj.Evidence.substring(0,75)}...</span>` : obj.Evidence, width: "500px" },
            { data: obj => obj.AssociatedAccounts, className: "text-right", width: "50px" },
            { defaultContent: renderCustomerAction(), className: "has-icon text-center", width: "70px" }
        ],
    })

    load_ticket_grid();

    load_customer_grid();

    detail_tabs.tabs({
        activate: (e, ui) => switchTab(ui)
    });

    $(".-export").click(exportExcel);

    associated_accounts.dialog({
        height: "auto",
        width: 500,
        modal: true,
        autoOpen: false,
        resizable: false,
        draggable: false
    });

    $("table", associated_accounts).DataTable({
        ordering: false,
        searching: false,
        paging: false,
        lengthMenu: [10, 20, 50, 100, 500],
        pageLength: 50,
        scrollCollapse: true,
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
        columnDefs: [
            { defaultContent: "-", targets: "_all" }
        ],
        columns: [
            { data: (obj, typet, set, meta) => meta.row + 1, className: "text-center", width: "40px" },
            { data: obj => obj.Member },
            { data: obj => obj.TransDate },
            { data: obj => obj.BetID },
            { data: obj => obj.Bettype },
            { data: obj => obj.Choice },
            { data: obj => obj.Result },
            { data: obj => obj.Odds },
            { data: obj => obj.Stake },
            { data: obj => obj.Winloss },
            { data: obj => obj.Status }
        ]
    })
}

$(fraud_case_details)