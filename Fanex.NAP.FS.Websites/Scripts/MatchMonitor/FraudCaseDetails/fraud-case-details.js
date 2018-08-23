function fraud_case_details() {
    var ticket_grid = $("#ticket-tab table"),
        detail_tabs = $("#detailTabs"),
        customer_grid = $("#customer-tab table"),
        associated_accounts = $("#associated-accounts"),
        ticket_last_7_days = $("#tickets-last-7-days"),
        ticket_by_ip = $("#ticket-by-ip");

    var global_idx = 0;

    var filter = {};

    var totalTicket = 0;

    var ticketLoad = false;

    var customerLoad = false;

    function load_ticket_grid() {
        common.showLoading();

        if (isArbitrage) {
            var url = common.modifyRelativeUrl("/MatchMonitor/GetArbitrageTicketDetails");

            $.get(url, parameters, renderArbitrageGrid);
        }
        else {
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

                $(".ip-link").click(renderTicketByIp);

                ticketLoad = true;

                if (ticketLoad && customerLoad) {
                    common.closeLoading();
                }
            });
        }
    }

    function load_customer_grid() {
        common.showLoading();

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
                        renderAssociatedAccounts(obj);
                    }
                    else if (this.value == "1") {
                        renderLast7DaysTickets(obj);
                    }
                    else {
                        openSuspiciousHistoryLink(obj);
                    }
                },
            });

            $(".group-action").controlgroup();

            $(".group-action button").click(e => $(e.target).next().selectmenu("open"));

            customerLoad = true;

            if (ticketLoad && customerLoad) {
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
        if (ui.newPanel.is("#customer-tab")) {
            customer_grid.DataTable().columns.adjust();
            customer_grid.DataTable().columns.adjust();
        }
        else {
            ticket_grid.DataTable().columns.adjust();
        }
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

    function renderAssociatedAccounts(obj) {
        var url = common.modifyRelativeUrl("/MatchMonitor/GetAssociation");

        common.showLoading();

        $.get(url, { custId: obj.CustId }, function (json) {
            associated_accounts
                .dialog("option", "title", `Associated accounts of customer ${obj.Member}`)
                .dialog("open");

            $("table", associated_accounts).DataTable().clear().columns.adjust().rows.add(json).draw();

            common.closeLoading();
        })
    }

    function renderLast7DaysTickets(obj) {
        var url = common.modifyRelativeUrl("/MatchMonitor/GetLast7DaysTicket")

        common.showLoading();

        $.get(url, { custId: obj.CustId }, function (json) {
            var from = moment().add(-6, "d");

            ticket_last_7_days
                .dialog("option", "title", `Tickets of customer ${obj.Member} from ${from.format("MMM D, YYYY")}`)
                .dialog("open");

            $("table", ticket_last_7_days).DataTable().clear().rows.add(json).draw().columns.adjust();

            common.closeLoading();
        });
    }

    function renderTicketByIp(e) {
        var parent = $(e.target).parents("tr");

        var obj = ticket_grid.DataTable().row(parent).data();

        var url = common.modifyRelativeUrl("/MatchMonitor/GetTicketFromIP")

        var p = {};

        p.ip = obj.IP;

        common.showLoading();

        $.get(url, p, function (json) {
            ticket_by_ip
                .dialog("option", "title", `Tickets of IP: ${obj.IP} from ${bu_date}`)
                .dialog("open");

            $("table", ticket_by_ip).DataTable().clear().rows.add(json).draw().columns.adjust();

            common.closeLoading();

            bindExportExcelByIP(obj);
        });
    }

    function bindExportExcelByIP(obj) {
        $(".-export", ticket_by_ip).off("click").click(function () {
            var url = common.modifyRelativeUrl("/MatchMonitor/GetExcelTicketFromIP");

            common.downloadFile(url, { ip: obj.IP });
        });
    }

    function openSuspiciousHistoryLink(obj) {
        var today = moment();

        var from = moment().add(-1, "months").date(1);

        var url = `/SuspiciousCustomerDetails?custid=${obj.CustId}&from=${from.format("MMM D, YYYY")}&to=${today.format("MMM D, YYYY")}&custName=${obj.Member}`;

        window.open(common.modifyRelativeUrl(url));
    }

    function renderArbitrageGrid(json) {
        var data = (json || []).reduce(function (arr, obj) {
            obj.key = true;

            arr.push(obj);

            var n = Math.max(obj.Home.length, obj.Away.length);

            for (var i = 0; i < n; i++) {
                var a = {};

                a.Home = obj.Home[i] || {};

                a.Away = obj.Away[i] || {};

                arr.push(a);
            }

            return arr;
        }, []);

        ticket_grid.DataTable().rows.add(data).draw();

        ticketLoad = true;

        if (ticketLoad && customerLoad) {
            common.closeLoading();
        }
    }

    if (isArbitrage) {
        ticket_grid.DataTable({
            ordering: false,
            paging: false,
            dom: "t<'pagination'<'page-size'lp>i>",
            language: common.language,
            scrollY: '57vh',
            scrollCollapse: true,
            columnDefs: [
                { targets: "_all", defaultContent: "-" },
            ],
            columns: [
                { data: obj => obj.Home.Username },
                { data: obj => (obj.Home.Evidences || []).join(", ") },
                { data: obj => obj.Home.DangerousLevel },
                { data: obj => obj.Home.Currency },
                { data: obj => obj.Home.BetId },
                { data: obj => common.formatGridNumber(obj.Home.Odds) },
                { data: obj => common.formatGridNumber((obj.Home.Stake || {}).Virtual) },
                { data: obj => common.formatGridNumber((obj.Home.Stake || {}).ComPT) },
                { data: obj => common.formatGridNumber((obj.Home.Stake || {}).Full) },
                { data: obj => obj.Home.TransDate },
                { data: obj => "" },
                { data: obj => obj.Away.Username },
                { data: obj => (obj.Away.Evidences || []).join(", ") },
                { data: obj => obj.Away.DangerousLevel },
                { data: obj => obj.Away.Currency },
                { data: obj => obj.Away.BetId },
                { data: obj => common.formatGridNumber(obj.Away.Odds) },
                { data: obj => common.formatGridNumber((obj.Away.Stake || {}).Virtual) },
                { data: obj => common.formatGridNumber((obj.Away.Stake || {}).ComPT) },
                { data: obj => common.formatGridNumber((obj.Away.Stake || {}).Full) },
                { data: obj => obj.Away.TransDate }
            ],
            createdRow: function (row, obj) {
                if (obj.key) {
                    var td = $(`<td colspan='21' class='group'>${obj.BettypeName} ${obj.Handicap} ${obj.Score}</td>`);

                    $(row).empty().html(td);
                }
                else if (obj.Home.Username == null) {
                    var td = $("<td colspan='10'></td>");

                    $("td:lt(10)", row).remove();

                    $(row).prepend(td);
                }
                else if (obj.Away.Username == null) {
                    var td = $("<td colspan='10'></td>");

                    $("td:gt(10)", row).remove();

                    $(row).append(td);
                }
            },
        });
    }
    else {
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
                { data: obj => moment(obj.TransDate, "MMM D, YYYY hh:mm:ss A").format("MMM D, YYYY<br/>hh:mm:ss A"), className: "text-center", width: "100px" },
                { data: obj => obj.BetID, width: "100px" },
                { data: obj => obj.Choice, className: "text-right" },
                { data: obj => obj.Score, className: "text-center", width: "40px" },
                { data: obj => obj.Odds != null ? obj.Odds.Html : null, width: "50px", className: "text-right" },
                { data: obj => obj.Stake == null ? null : numeral(obj.Stake).format("0,0.00"), className: "text-right", width: "100px" },
                { data: obj => obj.WinLoss == null ? null : obj.WinLoss >= 0 ? numeral(obj.WinLoss).format("0,0.00") : `<span class='text-red'>${numeral(obj.WinLoss).format('0,0.00')}</span>`, className: "text-right", width: "100px" },
                { data: obj => obj.Status, width: "82px", className: "text-center uppercase" },
                { data: obj => `<span class='link ip-link'>${obj.IP}</span>`, width: "100px" }
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
    }

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
            { data: obj => obj.TotalWinLoss >= 0 ? numeral(obj.TotalWinLoss).format("0,0.00") : `<span class='text-red'>${numeral(obj.TotalWinLoss).format("0,0.00")}</span>`, className: "text-right" },
            { data: obj => obj.Evidence != null && obj.Evidence.length > 78 ? `<span title='${obj.Evidence}'>${obj.Evidence.substring(0, 75)}...</span>` : obj.Evidence, width: "500px" },
            { data: obj => obj.AssociatedAccounts, className: "text-right", width: "50px" },
            { defaultContent: renderCustomerAction(), className: "has-icon text-center", width: "70px" }
        ],
    });

    load_ticket_grid();

    load_customer_grid();

    detail_tabs.tabs({
        activate: (e, ui) => switchTab(ui)
    });

    $(".-export").click(exportExcel);

    associated_accounts.dialog({
        width: 500,
        modal: true,
        autoOpen: false,
        resizable: false,
        draggable: false
    });

    ticket_last_7_days.dialog({
        maxHeight: 800,
        width: 1200,
        modal: true,
        autoOpen: false,
        resizable: false,
        draggable: false
    });

    ticket_by_ip.dialog({
        maxHeight: 800,
        width: 1200,
        modal: true,
        autoOpen: false,
        resizable: false,
        draggable: false
    });

    $("table", ticket_last_7_days).DataTable({
        ordering: false,
        searching: false,
        lengthMenu: [10, 20, 50, 100, 500],
        pageLength: 50,
        scrollCollapse: true,
        scrollY: "55vh",
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
            { defaultContent: "-", targets: "_all" },
            { targets: [0, 1, 4, -2], className: "text-center" },
            { targets: [3, 5, -3, -4], className: "text-right" }
        ],
        columns: [
            { data: (obj, typet, set, meta) => meta.row + 1, width: "40px" },
            { data: obj => moment(obj.TransDate, "MMM D, YYYY hh:mm:ss A").format("MMM D, YYYY<br/>hh:mm:ss A"), width: "100px" },
            { data: obj => obj.BetID, width: "100px" },
            { data: obj => obj.Choice },
            { data: obj => `<div class='row-ht'>HT: ${obj.Result.HTHomeScore} - ${obj.Result.HTAwayScore}</div><div class='row-ft'>FT: ${obj.Result.FinalHomeScore} - ${obj.Result.FinalAwayScore}</div>`, width: "60px" },
            { data: obj => obj.Odds != null ? obj.Odds.Html : null, width: "50px" },
            { data: obj => common.formatNumber(obj.Stake), width: "100px" },
            { data: obj => obj.WinLoss == null ? null : obj.WinLoss >= 0 ? common.formatNumber(obj.WinLoss) : `<span class='text-red'>${common.formatNumber(obj.WinLoss)}</span>`, width: "100px" },
            { data: obj => obj.Status, width: "82px", className: "text-center uppercase" },
            { data: obj => obj.IP, width: "100px" }
        ],
        headerCallback: tr => $("th", tr).removeClass("uppercase")
    });

    $("table", associated_accounts).DataTable({
        ordering: false,
        searching: false,
        paging: false,
        lengthMenu: [10, 20, 50, 100, 500],
        pageLength: 50,
        scrollCollapse: true,
        scrollY: "370px",
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
            { data: obj => obj.Currency, className: "text-center", width: "20px" },
            { data: obj => obj.Username },
        ]
    });

    $("table", ticket_by_ip).DataTable({
        ordering: false,
        searching: false,
        lengthMenu: [10, 20, 50, 100, 500],
        pageLength: 50,
        scrollCollapse: true,
        scrollY: "55vh",
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
            { defaultContent: "-", targets: "_all" },
            { targets: [0, 2, 3, -1, 6], className: "text-center" },
            { targets: [-2, -3, -4, 5], className: "text-right" },
            { targets: [3], width: "100px" },
            { targets: [0, 2], width: "40px" }
        ],
        columns: [
            { data: (obj, typet, set, meta) => meta.row + 1, className: "text-center" },
            { data: obj => obj.Member },
            { data: obj => obj.CurrencyName },
            { data: obj => moment(obj.TransDate, "MMM D, YYYY hh:mm:ss A").format("MMM D, YYYY<br/>hh:mm:ss A") },
            { data: obj => obj.BetID },
            { data: obj => obj.Choice },
            { data: obj => `<div class='row-ht'>HT: ${obj.Result.HTHomeScore} - ${obj.Result.HTAwayScore}</div><div class='row-ft'>FT: ${obj.Result.FinalHomeScore} - ${obj.Result.FinalAwayScore}</div>`, width: "60px" },
            { data: obj => obj.Odds != null ? obj.Odds.Html : null },
            { data: obj => obj.Stake == null ? null : numeral(obj.Stake).format("0,0.00") },
            { data: obj => obj.WinLoss == null ? null : obj.WinLoss >= 0 ? numeral(obj.WinLoss).format("0,0.00") : `<span class='text-red'>${numeral(obj.WinLoss).format('0,0.00')}</span>` },
            { data: obj => obj.Status, width: "50px", className: "text-center uppercase" }
        ],
        headerCallback: tr => $("th", tr).removeClass("uppercase")
    });
}

$(fraud_case_details);