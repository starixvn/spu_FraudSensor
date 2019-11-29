function fraud_case_details() {
    var ticket_grid = $("#ticket-tab table"),
        detail_tabs = $("#detailTabs"),
        customer_grid = $("#customer-tab table"),
        associated_accounts = $("#associated-accounts"),
        ticket_last_7_days = $("#tickets-last-7-days"),
        ticket_by_ip = $("#ticket-by-ip");
    var loadingDone = false;   
    var from_date = $("#transDateFrom");
    var to_date = $("#transDateTo");

    var global_idx = 0;

    var filter = {};

    var totalTicket = 0;
    var arrGroupTicket = [];
    var groupExpand = "";
    var groupExportExcel = 0;
    var savedCriteria = null;

    from_date.datepicker({
        changeMonth: true,
        changeYear: true,
        firstDay: 1,
        minDate: moment(`${moment().year() - 1}/${moment().subtract(10, "months").month()}/1`, "M dd, yy").toDate()
    }).datepicker("setDate", to_date.datepicker("getDate")).change(restrictToDate);

    detail_tabs.removeClass("ui-widget");

    function restrictToDate() {
        var from_val = from_date.datepicker("getDate");
        to_date.datepicker("option", "minDate", from_val);
    };

    function initMultipleSelect() {
        $("#ddlSite").multiselect({
            menuWidth: 220,
            menuHeight: 350
        }).multiselectfilter({
            width: 140,
            placeholder: ''
        });

        $("#ddlCurrency").multiselect({
            menuWidth: 140,
            menuHeight: 350
        }).multiselectfilter({
            width: 62,
            placeholder: ''
        });

        $("#ddlEvidence").multiselect({
            menuWidth: 345,
            menuHeight: 350
        }).multiselectfilter({
            width: 270,
            placeholder: ''
        });
    };

    function getBetType() {
        var url = common.modifyRelativeUrl('/MatchMonitor/GetBetTypes');

        $.get(url, parameters, function (json) {
            $('#betType').append('<option value=-1>All</option>');

            for (var i = 0; i < json.length; i++) {
                $('#betType').append(`<option value='${json[i].BetType}'>${json[i].BetTypeName}</option>`);
            }

            $('#betType').append('<option value=0>Others</option>');
        });
    }

    function getScoreHDP() {
        var url = common.modifyRelativeUrl("/MatchMonitor/GetScoreHDP");

        $.get(url, parameters, function (json) {
            $("#scoreHDP").append("<option value=-1>All</option>");

            for (var i = 0; i < json.length; i++) {
                $("#scoreHDP").append(`<option value='${json[i].KeyValue}'>${json[i].KeyName}</option>`);
            }
        })
    }

    function loadTicketGrid() {
        common.showLoading();

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
            common.closeLoading();
        });
    }

    function loadCustomerGrid() {
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

            //$(".group-action").controlgroup();

            //$(".group-action button").click(e => $(e.target).next().selectmenu("open"));

            common.closeLoading();
        });
    }

    function loadCustomerGridFlaggedAccount(criteria) {
        var url = common.modifyRelativeUrl("/MatchMonitor/GetFlaggedAccountCustomers");

        $.post(url, criteria, function (json) {
            $("#customers").text(`By Customers (${json.length})`);
            customer_grid.DataTable().clear().rows.add(json).draw();                       

            actionSelectmenu();
            if (loadingDone) {
                common.closeLoading();
            } else {
                loadingDone = true;
            }

            if (json.length > 0) {
                var urlAssociationAccounts = common.modifyRelativeUrl("/MatchMonitor/GetFlaggedAccountAssociationCustomers");
                $.post(urlAssociationAccounts, { custIds: json[0].CustIdCaches }, function (aaResponse) {
                    renderCustomerAssociatedAccounts(aaResponse);
                })
            }
        });
    }

    function loadCustomerGridArbitrage(criteria) {
        var url = common.modifyRelativeUrl("/MatchMonitor/GetFraudCaseArbitrageCustomerDetails");

        $.post(url, criteria, function (json) {
            if ((json.length > 0 && typeof json[0].TotalCustomers !== "undefined") || json.length >= 1000) {
                customer_grid.DataTable().clear().rows.add([]).draw();
                var totalCustomers = json.length >= 1000 ? json.length : json[0].TotalCustomers;

                $("#customers").text(`By Customers (${totalCustomers})`);
                $(".dataTables_empty").text("Too much data. Export Excel file instead!").css("color", "red");

            } else {
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
            }

            if (loadingDone) {
                common.closeLoading();
            } else {
                loadingDone = true;
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
        }
        else {
            ticket_grid.DataTable().columns.adjust();
        }
    }

    function exportExcel() {
        if (isArbitrage) {
            exportArbitrage();
        } else if (isFlaggedAccount) {
            exportFlaggedAccounts();
        } else {
            var url = common.modifyRelativeUrl("/MatchMonitor/ExportExcelForDetail");

            common.downloadFile(url, parameters);
        }
    }

    function exportArbitrage() {
        common.showLoading();
        var selectedTab = $("#detailTabs li[aria-selected=true]").attr("aria-controls");
        var urlSaveTicketCustIds = common.modifyRelativeUrl("/MatchMonitor/GetArbitrageTicketsGroupSummary");
        var urlSaveCriteria = selectedTab === "customer-tab" ? common.modifyRelativeUrl("/MatchMonitor/SaveFileArbitrageCustomers")
            : common.modifyRelativeUrl("/MatchMonitor/SaveFileArbitrageTickets");

        var urlDownload = common.modifyRelativeUrl("/MatchMonitor/DownloadExcel");

        var criteria = {};
        if (savedCriteria == null) {
            criteria = getCriteriaArbitrage();
        } else {
            criteria = Object.assign({}, savedCriteria);
        }

        if (criteria.isByIPAndDevice) {
            $.post(urlSaveTicketCustIds, criteria, function (json) { });
        }

        $.ajax({
            type: 'POST',
            url: urlSaveCriteria,
            data: criteria,
            success: function (response) {
                common.closeLoading();
                common.downloadFile(urlDownload, { fileId: response.FileId, fileName: response.FileName });
            },
            error: function (xhr, status, error) {
                common.closeLoading();
                common.showError(error);
                console.log('data of ' + xhr.responseText);
            }
        });
    }

    function exportFlaggedAccounts() {
        common.showLoading();
        var urlSaveCriteria = common.modifyRelativeUrl("/MatchMonitor/SaveFileFlaggedAccounts");
        var urlDownload = common.modifyRelativeUrl("/MatchMonitor/DownloadExcel");
        var criteria = {};

        if (savedCriteria == null) {
            criteria = getCriteriaFlaggedAccounts();
        } else {
            criteria = Object.assign({}, savedCriteria);
        }

        $.ajax({
            type: 'POST',
            url: urlSaveCriteria,
            data: criteria,
            success: function (response) {
                common.closeLoading();
                common.downloadFile(urlDownload, { fileId: response.FileId, fileName: response.FileName });
            },
            error: function (xhr, status, error) {
                common.closeLoading();
                common.showError(error);
                console.log('data of ' + xhr.responseText);
            }
        });
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

        var url = common.modifyRelativeUrl("/MatchMonitor/GetTicketFromIP");

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

    function renderCustomerAssociatedAccounts(obj) {
        customer_grid.DataTable().rows().every(function () {
            var rowData = this.data();
            var htmlAssociatedAccounts = rowData.AssociatedAccounts;
            var custId = $(htmlAssociatedAccounts).attr("custId");
            rowData.AssociatedAccounts = getTotalAssociation(custId, obj);
            this.invalidate();
        });        

        customer_grid.DataTable().draw();
        actionSelectmenu();
    }

    function getTotalAssociation(custId, arrCustId) {
        for (var i = 0; i < arrCustId.length; i++) {
            if (arrCustId[i].CustId == custId) {
                return arrCustId[i].TotalAssociation;
            }
        }
        return 0;
    }

    function actionSelectmenu() {
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

    function renderTicketArbitrageGroups(json) {
        $("#tickets").text(`By Tickets (...)`);
        var n = (json || []).reduce((total, obj) => total + Math.max(obj.Home.length, obj.Away.length), 0);
        var t = (json || []).reduce((total, obj) => total + obj.Home.length + obj.Away.length, 0);

        var data = (json || []).reduce(function (arr, obj) {
            obj.key = true;
            arr.push(obj);
            arrGroupTicket.push(obj);

            return arr;
        }, []);

        ticket_grid.DataTable().rows.add(data).draw();
        totalTicket = t;
        $("#tickets").text(`By Tickets (${totalTicket})`);

        if (n != 0) {
            $("#DataTables_Table_0_info").text(`Displaying 1 to ${n} of ${n} item(s)`);
        }

        if (loadingDone) {
            common.closeLoading();
        } else {
            loadingDone = true;
        }
    };

    function renderArbitrageTicketDetailByGroup(json) {
        ticket_grid.DataTable().clear();
        var n = (json || []).reduce((total, obj) => total + Math.max(obj.Home.length, obj.Away.length), 0);
        var t = (json || []).reduce((total, obj) => total + obj.Home.length + obj.Away.length, 0);

        var keyName = "";
        var data = (json || []).reduce(function (arr, obj) {
            obj.key = true;
            keyName = obj.Score + " " + obj.BettypeName + " " + obj.Handicap + " " + obj.GroupId + " " + obj.IP;

            if (obj.Home.length + obj.Away.length < 1000) {

                var n = Math.max(obj.Home.length, obj.Away.length);

                for (var i = 0; i < n; i++) {
                    var a = {};

                    a.Home = obj.Home[i] || {};
                    a.Away = obj.Away[i] || {};
                    arr.push(a);
                }
            }

            return arr;
        }, []);

        //each list group
        var dataGroup = [];
        $.each(arrGroupTicket, function (index, value) {
            var keyTemp = value.Score + " " + value.BettypeName + " " + value.Handicap + " " + value.GroupId + " " + value.IP;
            dataGroup.push(value);
            if (keyName == keyTemp) {
                var newArray = $.merge(dataGroup, data);
                dataGroup = newArray;
            }
        });

        ticket_grid.DataTable().rows.add(dataGroup).draw();

        if (n != 0) {
            $("#DataTables_Table_0_info").text(`Displaying 1 to ${n} of ${n} item(s)`);
        }

        ticket_grid.DataTable().columns.adjust();

        $('.dataTables_scrollBody').animate({
            scrollTop: $('tr.cursor-index').offset().top - ($('.dataTables_scrollBody').height() / 2)
        }, 100);

        common.closeLoading();
    };

    function renderArbitrageGrid(json) {
        ticket_grid.DataTable().clear();

        var data = (json || []).reduce(function (arr, obj) {
            obj.key = true;

            arr.push(obj);

            if (obj.Home.length + obj.Away.length < 1000) {

                var n = Math.max(obj.Home.length, obj.Away.length);

                for (var i = 0; i < n; i++) {
                    var a = {};
                    a.Home = obj.Home[i] || {};
                    a.Away = obj.Away[i] || {};
                    arr.push(a);
                }
            }

            return arr;
        }, []);

        var n = (json || []).reduce((total, obj) => total + Math.max(obj.Home.length, obj.Away.length), 0);
        var t = (json || []).reduce((total, obj) => total + obj.Home.length + obj.Away.length, 0);

        ticket_grid.DataTable().rows.add(data).draw();

        $("#tickets").text(`By Tickets (${t})`);

        if (n != 0) {
            $("#DataTables_Table_0_info").text(`Displaying 1 to ${n} of ${n} item(s)`);
        }

        common.closeLoading();
    }

    function exportGroupTickets(e) {
        groupExportExcel = 1;
        var urlSaveCriteria = common.modifyRelativeUrl("/MatchMonitor/SaveFileArbitrageTickets");
        var urlDownload = common.modifyRelativeUrl("/MatchMonitor/DownloadExcel");

        var obj = e.data;
        var keyName = obj.Score + " " + obj.BettypeName + " " + obj.Handicap;

        var criteria = {};

        if (savedCriteria == null) {
            var criteria = {
                liveStatus: $("#market").val(),
                evidenceCodeList: getEvidences(),
                currencyIds: getCurrency(),
                currencyNames: getCurrencyNames(),
                sites: getSites(),
                transDateFromString: $("#transDateFrom").val(),
                transDateToString: $("#transDateTo").val(),
                keyName: $("#scoreHDP option:selected").text(),
                matchId: parameters.MatchId,
                eventDate: parameters.EventDate,
                groupId: obj.GroupId,
                bettype: obj.BettypeId,
                score: obj.Score,
                ip: obj.IP,
                liveHomeScore: obj.LiveHomeScore,
                liveAwayScore: obj.LiveAwayScore,
                handicap: obj.Handicap
            };
        } else {
            criteria = Object.assign({}, savedCriteria);
            criteria.keyName = keyName;
            criteria.groupId = obj.GroupId;
            criteria.bettype = obj.BettypeId;
            criteria.score = obj.Score;
            criteria.liveHomeScore = obj.LiveHomeScore;
            criteria.liveAwayScore = obj.LiveAwayScore;
            criteria.handicap = obj.Handicap;
            criteria.ip = obj.IP;
        }

        $.ajax({
            type: 'POST',
            url: urlSaveCriteria,
            data: criteria,
            success: function (response) {
                common.downloadFile(urlDownload, { fileId: response.FileId, fileName: response.FileName });
            },
            error: function (xhr, status, error) {
                console.log('data of ' + xhr.responseText);
            }
        });
    }

    if (isArbitrage) {
        ticket_grid.DataTable({
            ordering: false,
            paging: false,
            lengthMenu: [1000],
            pageLength: 1000,
            dom: "t<'pagination'<'page-size'lp>>",
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
            scrollX: "1855px",
            scrollCollapse: true,
            columnDefs: [
                { targets: "_all", defaultContent: "-" },
                { targets: [-5, -4, -3, -2, 6, 7, 8, 9, 19, 20, 21, 22], className: "text-right" },
                { targets: [-1, 2, 4, 17], className: "text-center" },
                { targets: [0, 13], width: "170px" },
                { targets: [1, 14], width: "145px" },
                { targets: [2, 15], width: "91px" },
                { targets: [3, 16], width: "90px" },
                { targets: [4, 17], width: "75px" },
                { targets: [5, 18], width: "107px" },
                { targets: [6, 19], width: "49px" },
                { targets: [7, 8, 9, 20, 21, 22], width: "70px" },
                { targets: [10, 23], width: "105px" },
                { targets: [11, 24], width: "105px" }
            ],
            columns: [
                { data: obj => obj.Home.Username },
                { data: obj => common.makeWrapString((obj.Home.Evidences || []).join(", "), 16), width: "40px" },
                { data: obj => obj.Home.DangerousLevel },
                { data: obj => obj.Home.Site },
                { data: obj => obj.Home.Currency },
                { data: obj => renderIPFraud(obj.Home) },
                { data: obj => common.formatGridNumber(obj.Home.Odds) },
                { data: obj => common.formatGridNumber((obj.Home.Stake || {}).Virtual) },
                { data: obj => common.formatGridNumber((obj.Home.Stake || {}).ComPT) },
                { data: obj => common.formatGridNumber((obj.Home.Stake || {}).Full) },
                { data: obj => obj.Home.TransDate },
                { data: obj => obj.Home.IP },
                { data: obj => "", className: "plit-table", width: "5px" },
                { data: obj => obj.Away.Username },
                { data: obj => common.makeWrapString((obj.Away.Evidences || []).join(", "), 16), width: "40px" },
                { data: obj => obj.Away.DangerousLevel },
                { data: obj => obj.Away.Site },
                { data: obj => obj.Away.Currency },
                { data: obj => renderIPFraud(obj.Away) },
                { data: obj => common.formatGridNumber(obj.Away.Odds) },
                { data: obj => common.formatGridNumber((obj.Away.Stake || {}).Virtual) },
                { data: obj => common.formatGridNumber((obj.Away.Stake || {}).ComPT) },
                { data: obj => common.formatGridNumber((obj.Away.Stake || {}).Full) },
                { data: obj => obj.Away.TransDate },
                { data: obj => obj.Away.IP }
            ],
            createdRow: function (row, obj) {
                if (obj.key) {
                    var totalTicket = obj.Home.length + obj.Away.length;
                    var td = "";
                    if (totalTicket > 1000) {
                        td = $(`<td colspan='25' class='group' style='cursor: initial'><span class='icon-chevron-down'></span> <span class='bage'>${obj.BettypeName} ${obj.Handicap}</span> <span class="bage">${obj.Score}</span> <span class='bage'>Tickets: ${totalTicket}</span> <span class="bage muchdata" style="color:red;">Too much data. Export Excel file instead!</span> <button class="button -export"><span class="icon-export-excel"></span></button></td>`);
                    }
                    else {
                        var keyName = obj.Score + " " + obj.BettypeName + " " + obj.Handicap + " " + obj.GroupId + " " + obj.IP;
                        if (groupExpand == keyName) {
                            td = $(`<td colspan='25' class='group' style='cursor: initial'><span class='icon-chevron-up' style='cursor: pointer;'></span><span class='bage' style='cursor: pointer;'>${obj.BettypeName} ${obj.Handicap}</span> <span class='bage' style='cursor: pointer;'>${obj.Score}</span> <span class='bage' style='cursor: pointer;'>Tickets: ${totalTicket}</span><button class="button -export"><span class="icon-export-excel"></span></button></td>`);
                            $(row).addClass('cursor-index');
                        } else {
                            td = $(`<td colspan='25' class='group' style='cursor: initial'><span class='icon-chevron-down' style='cursor: pointer;'></span><span class='bage' style='cursor: pointer;'>${obj.BettypeName} ${obj.Handicap}</span> <span class='bage' style='cursor: pointer;'>${obj.Score}</span> <span class='bage' style='cursor: pointer;'>Tickets: ${totalTicket}</span><button class="button -export"><span class="icon-export-excel"></span></button></td>`);
                        }
                    }
                    $(".button.-export", td).click(obj, exportGroupTickets);

                    $(row).empty().html(td);

                }
                else if (obj.Home.Username == null) {
                    var td = $("<td colspan='13'></td>");

                    $("td:lt(13)", row).remove();

                    $(row).prepend(td);

                }
                else if (obj.Away.Username == null) {
                    var td = $("<td colspan='13'></td>");

                    $("td:gt(13)", row).remove();

                    $(row).append(td);
                }
            }
        });
    } else if (isFlaggedAccount) {
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
            scrollX: "1855px",
            scrollY: '57vh',
            scrollCollapse: true,
            columns: [
                { data: obj => obj.key ? obj.value : ++global_idx, className: "text-center", width: "30px" },
                { data: obj => obj.Member, width: "120px" },
                { data: obj => common.makeWrapString((obj.Evidences || []).join(", "), 30), width: "160px" },
                { data: obj => obj.DangerousLevel, className: "text-center", width: "40px" },
                { data: obj => obj.Site, width: "120px" },
                { data: obj => obj.CurrencyName, className: "text-center", width: "10px" },
                { data: obj => moment(obj.TransDate, "MMM D, YYYY hh:mm:ss A").format("MMM D, YYYY<br/>hh:mm:ss A"), className: "text-center", width: "120px" },
                { data: obj => obj.BetID, width: "120px" },
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
                    var td = $(`<td colspan='15' class='group'><span class='icon-chevron-up'></span> ${obj.value} </td>`);

                    $(row).empty().html(td).click(e => show_hide_bettype_items(e.target, obj.value));
                } else {
                    $(row).attr("data-bettype-item", obj.Bettype);
                }
            },
            headerCallback: tr => $(":nth-child(14)", tr).removeClass("uppercase")
        });
    } else {
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
                    var td = $(`<td colspan='12' class='group'><span class='icon-chevron-up'></span> ${obj.value} </td>`);

                    $(row).empty().html(td).click(e => show_hide_bettype_items(e.target, obj.value));
                } else {
                    $(row).attr("data-bettype-item", obj.Bettype);
                }
            },
            headerCallback: tr => $(":nth-child(11)", tr).removeClass("uppercase")
        });
    }

    function renderIPFraud(obj) {
        var isIPFraud = obj.IsIPFraud;
        if (isIPFraud == true) {

            return "<div style='position: relative; width: 100%; height: 100%; background: #FFCCCA'>" + obj.BetId + "</div>";
        }
        return obj.BetId;
    };

    function loadFlaggedAccountsTicket(criteria) {
        var url = common.modifyRelativeUrl('/MatchMonitor/GetFlaggedAccountTickets');

        $.ajax({
            type: 'POST',
            url: url,
            data: criteria,
            success: function (response) {
                if (response != null) {
                    renderFlaggedAccountTicketGrid(response);
                }
            },
            error: function (err) {
                loadError(err);
                if (loadingDone) {
                    common.closeLoading();
                } else {
                    loadingDone = true;
                }
            }
        });
    }

    function renderFlaggedAccountTicketGrid(json) {
        var table = ticket_grid.DataTable();

        table.clear().rows.add(process_ticket_json(json)).draw();

        var n = json.reduce((a, arr) => a + arr.data.length, 0);

        $("#tickets").text(`By Tickets (${n})`);

        if (n != 0) {
            $("#DataTables_Table_0_info").text(`Displaying 1 to ${n} of ${n} item(s)`);
        }

        totalTicket = n;
        $(".ip-link").click(renderTicketByIp);

        if (loadingDone) {
            common.closeLoading();
        } else {
            loadingDone = true;
        }
    }

    function loadTicketGridArbitrage(criteria) {
        var urlGroup = common.modifyRelativeUrl("/MatchMonitor/GetArbitrageTicketsGroupSummary");
        ticket_grid.DataTable().clear();

        $.ajax({
            type: 'POST',
            url: urlGroup,
            data: criteria,
            beforeSend: function () {
                common.showLoading();
            },
            success: function (response) {
                loadCustomerGridArbitrage(criteria);

                if (response != null) {
                    renderTicketArbitrageGroups(response.Content);
                }
            },
            error: function (err) {
                loadError(err);
            }
        });
    }

    $('#ticket-tab table tbody').on('click', 'tr', function () {
        if (isArbitrage) {
            if (groupExportExcel == 0) {
                var table = ticket_grid.DataTable();
                var data = table.row(this).data();
                if (data.key == true) {
                    var spanTicketMuch1000 = $('td .muchdata', this);

                    if ($('td .muchdata', this).is("span.muchdata")) {
                        return;
                    }

                    var span = $('td span:first', this);

                    if ($(span).is("span.icon-chevron-down")) {
                        $(span).removeClass("icon-chevron-down").addClass("icon-chevron-up");
                        expandGroup(data);
                        // $(this).addClass('cursor-index');
                    }
                    else if ($(span).is("span.icon-chevron-up")) {
                        $(span).removeClass("icon-chevron-up").addClass("icon-chevron-down");
                        collapseGroup();
                    }
                }
            }
            groupExportExcel = 0;
        }
    });

    function expandGroup(obj) {
        common.showLoading();

        var url = common.modifyRelativeUrl("/MatchMonitor/GetArbitrageTickets");

        var keyName = obj.Score + " " + obj.BettypeName + " " + obj.Handicap + " " + obj.GroupId + " " + obj.IP;
        groupExpand = keyName;

        var criteria = {};

        if (savedCriteria == null) {
            var criteria = {
                liveStatus: $("#market").val(),
                evidenceCodeList: getEvidences(),
                currencyIds: getCurrency(),
                sites: getSites(),
                transDateFromString: $("#transDateFrom").val(),
                transDateToString: $("#transDateTo").val(),
                keyName: keyName,
                bettype: obj.BettypeId,
                handicap: obj.Handicap,
                liveHomeScore: obj.LiveHomeScore,
                liveAwayScore: obj.LiveAwayScore,
                groupId: obj.GroupId,
                score: obj.Score,
                ip: obj.IP,
                matchId: parameters.MatchId,
                eventDate: parameters.EventDate,
                otherEvidenceCodes: parameters.OtherEvidenceCodes
            };
        } else {
            criteria = Object.assign({}, savedCriteria);

            criteria.keyName = keyName;
            criteria.bettype = obj.BettypeId;
            criteria.handicap = obj.Handicap;
            criteria.liveHomeScore = obj.LiveHomeScore;
            criteria.liveAwayScore = obj.LiveAwayScore;
            criteria.groupId = obj.GroupId;
            criteria.score = obj.Score;
            criteria.ip = obj.IP;
        }

        $.post(url, criteria, function (json) {
            if (json != null) {

                renderArbitrageTicketDetailByGroup(json);
            }
        });
    };

    function collapseGroup() {
        ticket_grid.DataTable().clear();
        groupExpand = "";

        ticket_grid.DataTable().rows.add(arrGroupTicket).draw();
    };

    function loadError(error) {
        common.showError(error);
        console.log(error);
    };

    function loadGrids() {
        global_idx = 0;
        arrGroupTicket = [];
        groupExpand = "";
        var transDateFrom = $("#transDateFrom").val();
        var transDateTo = $("#transDateTo").val();
        var transDateFromInput = $("#transDateFrom");
        var transDateToInput = $("#transDateTo");

        if (transDateFrom !== "" && transDateTo === "") {
            transDateFromInput.css({ 'border': '1px solid #d6d6d6' });
            transDateToInput.css({ 'border-color': 'red', 'border-style': 'solid', 'border-width': '1px' });
        }
        else if (transDateFrom === "" && transDateTo !== "") {
            transDateFromInput.css({ 'border-color': 'red', 'border-style': 'solid', 'border-width': '1px' });
            transDateToInput.css({ 'border': '1px solid #d6d6d6' });
        }
        else {
            transDateFromInput.css({ 'border': '1px solid #d6d6d6' });
            transDateToInput.css({ 'border': '1px solid #d6d6d6' });
            loadingDone = false;
            var criteria = {};

            if (isArbitrage) {
                criteria = getCriteriaArbitrage();

                loadTicketGridArbitrage(criteria);
            }
            else if (isFlaggedAccount) {
                common.showLoading();
                criteria = getCriteriaFlaggedAccounts();

                loadCustomerGridFlaggedAccount(criteria);
                loadFlaggedAccountsTicket(criteria);
            }

            savedCriteria = criteria;
        }
    };

    function getCriteriaArbitrage() {
        var bettype = 0;
        var liveHomeScore = 0;
        var liveAwayScore = 0;
        var handicap = 0;
        var valBet = $("#scoreHDP").val();

        if (valBet == "-1") {
            bettype = "-1";
            liveHomeScore = "-1";
            liveAwayScore = "-1";
            handicap = "-1";
        } else if (valBet != "") {
            var arrBet = valBet.split(';');
            bettype = arrBet[0];
            liveHomeScore = arrBet[1];
            liveAwayScore = arrBet[2];
            handicap = arrBet[3];
        }

        var criteria = {
            liveStatus: $("#market").val(),
            evidenceCodeList: getEvidences(),
            currencyIds: getCurrency(),
            currencyNames: getCurrencyNames(),
            sites: getSites(),
            transDateFromString: $("#transDateFrom").val(),
            transDateToString: $("#transDateTo").val(),
            keyName: $("#scoreHDP option:selected").text(),
            bettype: bettype,
            liveHomeScore: liveHomeScore,
            liveAwayScore: liveAwayScore,
            handicap: handicap,
            matchId: parameters.MatchId,
            eventDate: parameters.EventDate,
            otherEvidenceCodes: parameters.OtherEvidenceCodes,
            odds: $("#txtOdds").val(),
            isOnlyCN88: $("#chkOnlyCN88:checked").length > 0,
            isByIPAndDevice: $("#chkByIPAndDevice:checked").length > 0
        };

        return criteria;
    }

    function getCriteriaFlaggedAccounts() {

        var criteria = {
            LiveStatus: $("#market").val(),
            EvidenceCodeList: getEvidences(),
            CurrencyIds: getCurrency(),
            CurrencyNames: getCurrencyNames(),
            Sites: getSites(),
            TransDateFromString: $("#transDateFrom").val(),
            TransDateToString: $("#transDateTo").val(),
            Bettype: $('#betType').val(),
            MatchId: parameters.MatchId,
            EventDate: parameters.EventDate,
            otherEvidenceCodes: parameters.OtherEvidenceCodes
        };

        return criteria;
    }

    function getEvidences() {
        var array = [];
        var arrEvidences = $("#ddlEvidence").multiselect("getChecked");
        var lengthEvidences = $("#ddlEvidence").children("option").length;

        for (var i = 0; i < arrEvidences.length; i++) {
            array.push($(arrEvidences[i]).attr('value'));
        }

        if (arrEvidences != null && arrEvidences.length > 0) {
            var evidenceCode = array.join(",");
        }
        return evidenceCode;
    }

    function getCurrency() {
        var array = [];
        var arrCurrency = $("#ddlCurrency").multiselect("getChecked");
        var lengthCurrency = $("#ddlCurrency").children("option").length;

        if (arrCurrency.length == lengthCurrency) {
            return "-1";
        }
        for (var i = 0; i < arrCurrency.length; i++) {
            array.push($(arrCurrency[i]).attr('value'));
        }

        if (arrCurrency != null && arrCurrency.length > 0) {
            var currencyIds = array.join(",");
        }
        return currencyIds;
    }

    function getCurrencyNames() {
        var array = [];
        var arrCurrency = $("#ddlCurrency").multiselect("getChecked");
        var lengthCurrency = $("#ddlCurrency").children("option").length;

        if (arrCurrency.length == lengthCurrency) {
            return "All";
        }
        for (var i = 0; i < arrCurrency.length; i++) {
            array.push($(arrCurrency[i]).next().text());
        }

        if (arrCurrency != null && arrCurrency.length > 0) {
            var currencyIds = array.join(",");
        }
        return currencyIds;
    }

    function getSites() {
        var array = [];
        var arrSites = $("#ddlSite").multiselect("getChecked");
        var lengthSites = $("#ddlSite").children("option").length;

        if (arrSites.length == lengthSites) {
            return "All";
        }
        for (var i = 0; i < arrSites.length; i++) {
            array.push($(arrSites[i]).next().text());
        }

        if (arrSites != null && arrSites.length > 0) {
            var sites = array.join(",");
        }
        return sites;
    }

    function getListHDPScore() {
        var ddlArray = new Array();
        var ddl = document.getElementById('scoreHDP');
        for (i = 0; i < ddl.options.length; i++) {
            ddlArray[i] = ddl.options[i].value;
        }
        return ddlArray;
    }

    function init() {
        if (isArbitrage) {
            getScoreHDP();
            initMultipleSelect();
        } else if (isFlaggedAccount) {
            getBetType();
            initMultipleSelect();
        } else {
            loadTicketGrid();
            loadCustomerGrid();
        }
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
            { data: obj => obj.Member, width: "200px" },
            { data: obj => obj.CurrencyName, className: "text-center", width: "20px" },
            { data: obj => obj.Tickets, className: "text-right", width: "40px" },
            { data: obj => numeral(obj.TotalStake).format("0,0.00"), className: "text-right", width: "100px" },
            { data: obj => obj.TotalWinLoss >= 0 ? numeral(obj.TotalWinLoss).format("0,0.00") : `<span class='text-red'>${numeral(obj.TotalWinLoss).format("0,0.00")}</span>`, className: "text-right", width: "100px" },
            { data: obj => obj.Evidence != null && obj.Evidence.length > 78 ? `<span title='${obj.Evidence}'>${obj.Evidence.substring(0, 75)}...</span>` : obj.Evidence, width: "500px" },
            { data: obj => obj.AssociatedAccounts, className: "text-right", width: "50px" },            
            { defaultContent: renderCustomerAction(), className: "has-icon text-center", width: "70px" }
        ]
    });

    detail_tabs.tabs({
        activate: (e, ui) => switchTab(ui)
    });

    $("#generate").click(loadGrids);

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

    init();
}

$(fraud_case_details);