function fraud_case_details() {
    var ticket_grid = $("#ticket-tab table"),
        detail_tabs = $("#detailTabs"),
        customer_grid = $("#customer-tab table"),
        associated_accounts = $("#associated-accounts"),
        ticket_last_7_days = $("#tickets-last-7-days"),
        ticket_by_ip = $("#ticket-by-ip");

    var from_date = $("#transDateFrom");
    var to_date = $("#transDateTo");

    var global_idx = 0;

    var filter = {};

    var totalTicket = 0;    
    var arrGroupTicket = [];

    from_date.datepicker({       
        changeMonth: true,
        changeYear: true,
        firstDay: 1,
        minDate: moment(`${moment().year() - 1}/${moment().subtract(10, "months").month()}/1`, "M dd, yy").toDate()
    }).datepicker("setDate", to_date.datepicker("getDate")).change(restrictToDate);
   
    function restrictToDate() {
        var from_val = from_date.datepicker("getDate");
        to_date.datepicker("option", "minDate", from_val);
    };
    
    function handleMultipleSelect() {           
        $("#ddlEvidence").multiselect({
            menuHeight: 350,
            menuWidth: 350
        });


    };

    function getAllCurrency() {
        var url = common.modifyRelativeUrl("/MatchMonitor/GetAllCurrency");        

        $.get(url, {}, function (json) {
            $("#currency").append("<option value=-1>All</option>");

            for (var i = 0; i < json.length; i++) {
                $("#currency").append(`<option value='${json[i].CurrencyId}'>${json[i].CurrencyName}</option>`)
            }
        })
    };

    function getScoreHDP() {
        var url = common.modifyRelativeUrl("/MatchMonitor/GetScoreHDP");        

        $.get(url, parameters, function (json) {
            $("#scoreHDP").append("<option value=-1>All</option>");
                     
            for (var i = 0; i < json.length; i++) {
                $("#scoreHDP").append(`<option value='${json[i].KeyName}'>${json[i].KeyName}</option>`);
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

            $(".group-action").controlgroup();

            $(".group-action button").click(e => $(e.target).next().selectmenu("open"));

            common.closeLoading();
        });
    }

    function loadTicketGridArbitrage(criteria) {
        if (isArbitrage) {            
            loadArbitrageGroups(criteria);
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
            });
        }      
    }

    function loadCustomerGridArbitrage(criteria) {
        var url = common.modifyRelativeUrl("/MatchMonitor/GetFraudCaseCustomerDetails");

        $.post(url, criteria, function (json) {
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

            common.closeLoading();
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
        if (isArbitrage) {
            evidenceCode = getEvidenceSelected();

            var criteria = {
                liveStatus: $("#market").val(),
                evidenceCodeList: evidenceCode,
                currencyId: $("#currency").val(),
                strDateFrom: $("#transDateFrom").val(),
                strDateTo: $("#transDateTo").val(),
                keyName: $("#scoreHDP").val(),
                matchId: parameters.matchId,
                eventDate: parameters.eventDate
            };

            var url = common.modifyRelativeUrl("/MatchMonitor/ExportCSVArbitrage");

            common.downloadFile(url, criteria);
        } else {
            var url = common.modifyRelativeUrl("/MatchMonitor/ExportExcelForDetail");

            common.downloadFile(url, parameters);
        }
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

    function generateArbitrageGridNext(json) {
        $("#tickets").text(`By Tickets (...)`);

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

        ticket_grid.DataTable().rows.add(data).draw(false);

        var n = (json || []).reduce((total, obj) => total + Math.max(obj.Home.length, obj.Away.length), 0);

        var t = (json || []).reduce((total, obj) => total + obj.Home.length + obj.Away.length, 0);
        
        totalTicket = totalTicket + t;
        $("#tickets").text(`By Tickets (${totalTicket})`);

        if (n != 0) {
            $("#DataTables_Table_0_info").text(`Displaying 1 to ${n} of ${n} item(s)`);
        }    
        common.closeLoading();

        return totalTicket;
    };

    function renderArbitrageGroups(json) {
        $("#tickets").text(`By Tickets (...)`);

        var n = (json || []).reduce((total, obj) => total + Math.max(obj.Home.length, obj.Away.length), 0);

        var t = (json || []).reduce((total, obj) => total + obj.Home.length + obj.Away.length, 0);

        var data = (json || []).reduce(function (arr, obj) {
            obj.key = true;
            //obj.Home = '';
            //obj.Away = '';
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
        common.closeLoading();

        return totalTicket;
    };

    function renderArbitrageByGroup(json) {       
        ticket_grid.DataTable().clear();

        var n = (json || []).reduce((total, obj) => total + Math.max(obj.Home.length, obj.Away.length), 0);

        var t = (json || []).reduce((total, obj) => total + obj.Home.length + obj.Away.length, 0);
       
        var keyName = "";
        var data = (json || []).reduce(function (arr, obj) {
            obj.key = true;
            arr.push(obj);
            keyName = obj.Score + " " + obj.BettypeName + " " + obj.Handicap;

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
            var keyTemp = value.Score + " " + value.BettypeName + " " + value.Handicap;
            if (keyName == keyTemp) {
                var newArray = $.merge(dataGroup, data);
                dataGroup = newArray;
            }
            else {
                dataGroup.push(value);
            }
        });                       

        ticket_grid.DataTable().rows.add(dataGroup).draw();      

        if (n != 0) {
            $("#DataTables_Table_0_info").text(`Displaying 1 to ${n} of ${n} item(s)`);
        }
        common.closeLoading();

        return totalTicket;
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

        ticket_grid.DataTable().rows.add(data).draw();

        var n = (json || []).reduce((total, obj) => total + Math.max(obj.Home.length, obj.Away.length), 0);

        var t = (json || []).reduce((total, obj) => total + obj.Home.length + obj.Away.length, 0);

        $("#tickets").text(`By Tickets (${t})`);

        if (n != 0) {
            $("#DataTables_Table_0_info").text(`Displaying 1 to ${n} of ${n} item(s)`);
        }
        
        common.closeLoading();
    }    

    function renderArbitrageCSV(e) {
        var url = common.modifyRelativeUrl("/MatchMonitor/ExportArbitrageWithFilterCSV");

        var obj = e.data;

        evidenceCode = getEvidenceSelected();

        var criteria = {
            liveStatus: $("#market").val(),
            evidenceCodeList: evidenceCode,
            currencyId: $("#currency").val(),
            strDateFrom: $("#transDateFrom").val(),
            strDateTo: $("#transDateTo").val(),
            keyName: $("#scoreHDP").val(),
            matchId: parameters.matchId,
            eventDate: parameters.eventDate,
            groupId: obj.GroupId,
            score: obj.Score,
            handicap: obj.Handicap
        };
            
        common.downloadFile(url, criteria);
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
            scrollX: "1870px",
            scrollCollapse: true,
            columnDefs: [
                { targets: "_all", defaultContent: "-" },
                { targets: [-5, -4, -3, -2, 5, 6, 7, 8], className: "text-right" },
                { targets: [-1, 2, 3, 4, 9, 13, 14, 15], className: "text-center" },
                { targets: [0, 11], width: "110px" },
                { targets: [1, 12], width: "120px" },
                { targets: [2, 13], width: "80px" },
                { targets: [3, 14], width: "65px" },
                { targets: [4, 15], width: "100px" },
                { targets: [5, 16], width: "42px" },
                { targets: [6, 7, 8, 17, 18, 19], width: "70px" },
                { targets: [9, 20], width: "100px" }
            ],
            columns: [
                { data: obj => obj.Home.Username },
                { data: obj => common.makeWrapString((obj.Home.Evidences || []).join(", "), 18)},
                { data: obj => obj.Home.DangerousLevel },
                { data: obj => obj.Home.Currency },
                { data: obj => renderIPFraud(obj.Home) },
                { data: obj => common.formatGridNumber(obj.Home.Odds) },
                { data: obj => common.formatGridNumber((obj.Home.Stake || {}).Virtual) },
                { data: obj => common.formatGridNumber((obj.Home.Stake || {}).ComPT) },
                { data: obj => common.formatGridNumber((obj.Home.Stake || {}).Full) },
                { data: obj => obj.Home.TransDate },
                { data: obj => "", className: "plit-table", width: "5px" },
                { data: obj => obj.Away.Username },
                { data: obj => common.makeWrapString((obj.Away.Evidences || []).join(", "), 18) },
                { data: obj => obj.Away.DangerousLevel },
                { data: obj => obj.Away.Currency },
                { data: obj => renderIPFraud(obj.Away) },
                { data: obj => common.formatGridNumber(obj.Away.Odds) },
                { data: obj => common.formatGridNumber((obj.Away.Stake || {}).Virtual) },
                { data: obj => common.formatGridNumber((obj.Away.Stake || {}).ComPT) },
                { data: obj => common.formatGridNumber((obj.Away.Stake || {}).Full) },
                { data: obj => obj.Away.TransDate }
            ],
            createdRow: function (row, obj) {
                if (obj.key) {
                    var totalTicket = obj.Home.length + obj.Away.length;
                    obj.Home = '';
                    obj.Away = '';
                    var td = (totalTicket > 1000) ? $(`<td colspan='21' class='group' style='cursor: initial'><span class='icon-chevron-down'></span> <span class='bage'>${obj.BettypeName} ${obj.Handicap}</span> <span class="bage">${obj.Score}</span> <span class='bage'>${totalTicket}</span> <span class="bage" style="color:red;">Too much data. Export CSV file instead!</span> <button class="button -export"><span class="icon-export-excel"></span></button></td>`)
                        : $(`<td colspan='21' class='group' style='cursor: initial'><span class='icon-chevron-down' style='cursor: pointer;'></span><span class='bage' style='cursor: pointer;'>${obj.BettypeName} ${obj.Handicap}</span> <span class='bage' style='cursor: pointer;'>${obj.Score}</span> <span class='bage' style='cursor: pointer;'>${totalTicket}</span><button class="button -export"><span class="icon-export-excel"></span></button></td>`);

                    $(".button.-export", td).click(obj, renderArbitrageCSV);

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
            }         
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
  
    function loadArbitrages(criteria) {
        var url = common.modifyRelativeUrl("/MatchMonitor/GetArbitrageTicketDetails");        
        common.showLoading();
        
        var objAll = "-1";
        
        if (criteria.keyName == objAll) {
           
            var listHDPScore = getListHDPScore();

            listHDPScore = $.grep(listHDPScore, function (value) {
                return value != objAll;
            });    
            
            //clear grid Arbitrage
            ticket_grid.DataTable().clear();
            totalTicket = 0;

            listHDPScore.forEach(function (item, index) {
                criteria.keyName = item;

                $.ajax({
                    type: 'POST',
                    url: url,
                    data: criteria,
                    success: function (response) {
                        if (response != null) {
                            generateArbitrageGridNext(response.Content);
                        }
                    },
                    error: function (err) {
                        loadError(err);
                    }
                });
            });
        }
        else {
            $.ajax({
                type: 'POST',
                url: url,
                data: criteria,
                success: function (response) {
                    if (response != null) {
                        renderArbitrageGrid(response.Content);
                    }
                },
                error: function (err) {
                    loadError(err);
                }
            });
        }       
    };

    function loadArbitrageGroups(criteria) {
        var url = common.modifyRelativeUrl("/MatchMonitor/GetArbitrageTicketDetails");
        var urlGroup = common.modifyRelativeUrl("/MatchMonitor/GetArbitrageGroupsTicket");

        common.showLoading();
        ticket_grid.DataTable().clear();

        //case all    
        if (criteria.keyName == "-1") {
            $.ajax({
                type: 'POST',
                url: urlGroup,
                data: criteria,
                success: function (response) {
                    if (response != null) {
                        renderArbitrageGroups(response.Content);
                    }
                },
                error: function (err) {
                    loadError(err);
                }
            });
        }
        else {
            $.ajax({
                type: 'POST',
                url: url,
                data: criteria,
                success: function (response) {
                    if (response != null) {
                        renderArbitrageGrid(response.Content);
                    }
                },
                error: function (err) {
                    loadError(err);
                }
            });
        }
    };

    $('#ticket-tab table tbody').on('click', 'tr', function () {
        if (isArbitrage) {
            var table = ticket_grid.DataTable();
            var data = table.row(this).data();
            if (data.Home == "" && data.Away == "") {
                collapseExpandGroup(data);
            }            
        }        
    });

    function collapseExpandGroup(obj) {
        common.showLoading();

        var url = common.modifyRelativeUrl("/MatchMonitor/GetListArbitrageTickets");
        
        evidenceCode = getEvidenceSelected();
        var keyName = obj.Score + " " + obj.BettypeName + " " + obj.Handicap;

        var criteria = {
            liveStatus: $("#market").val(),
            evidenceCodeList: evidenceCode,
            currencyId: $("#currency").val(),
            strDateFrom: $("#transDateFrom").val(),
            strDateTo: $("#transDateTo").val(),
            keyName: keyName,
            matchId: parameters.matchId,
            eventDate: parameters.eventDate
        };                

        $.ajax({
            type: 'GET',
            url: url,
            data: criteria,
            success: function (response) {
                if (response != null) {                    
                    renderArbitrageByGroup(response);
                }
            },
            error: function (err) {
                loadError(err);
            }
        });
    };
   
    function loadError(error) {
        console.log(error);
    };

    function loadGrids() {
        arrCollapse = [];
        evidenceCode = getEvidenceSelected();
        var transDateFrom = $("#transDateFrom").val();
        var transDateTo = $("#transDateTo").val();
        var transDateFromParent = $("#transDateFrom").parent();
        var transDateToParent = $("#transDateTo").parent();

        if (transDateFrom !== "" && transDateTo === "") {
            transDateFromParent.css({ 'border-color': '#666', 'border-style': 'solid', 'border-width': '0px' });
            transDateToParent.css({'border-color': 'red', 'border-style': 'solid', 'border-width': '1px' });
        }
        else if (transDateFrom === "" && transDateTo !== "") {
            transDateFromParent.css({ 'border-color': 'red', 'border-style': 'solid', 'border-width': '1px' });
            transDateToParent.css({ 'border-color': '#666', 'border-style': 'solid', 'border-width': '0px' });
        }
        else {
            transDateFromParent.css({ 'border-color': '#666', 'border-style': 'solid', 'border-width': '0px' });
            transDateToParent.css({ 'border-color': '#666', 'border-style': 'solid', 'border-width': '0px' });

            var criteria = {
                liveStatus: $("#market").val(),
                evidenceCodeList: evidenceCode,
                currencyId: $("#currency").val(),
                transDateFrom: $("#transDateFrom").val(),
                transDateTo: $("#transDateTo").val(),
            	keyName: $("#scoreHDP").val(),
                matchId: parameters.matchId,
                eventDate: parameters.eventDate
            };

            loadCustomerGridArbitrage(criteria);

            loadTicketGridArbitrage(criteria);
        }
    };

    function getEvidenceSelected()
    {
        var arrEvidences = $("#ddlEvidence").multiselect("getChecked");
        var array = [];

        for (var i = 0; i < arrEvidences.length; i++) {            
            array.push($(arrEvidences[i]).attr('value'));            
        }

        if (arrEvidences != null && arrEvidences.length > 0) {
           var evidenceCode = array.join(",");
        }
        return evidenceCode;
    }

    function getListHDPScore()
    {
        var ddlArray = new Array();
        var ddl = document.getElementById('scoreHDP');
        for (i = 0; i < ddl.options.length; i++) {
            ddlArray[i] = ddl.options[i].value;
        }
        return ddlArray;
    }

    function init() {
        if (isArbitrage) {
            getAllCurrency();
            getScoreHDP();
            handleMultipleSelect();
        }
        else {
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