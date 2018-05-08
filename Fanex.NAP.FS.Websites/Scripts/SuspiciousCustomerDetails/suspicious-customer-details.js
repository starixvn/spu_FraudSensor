function suspiciousCustomerDetails () {
    var summary = $("#summary"),
        choice = $("#choice"),
        topSport = $("#top-5-sport"),
        topBettype = $("#top-5-bettype"),
        violatedTickets = $("#violated-tickets");

    var ticketIdx = 0;

    var filter = {};

    let stopRender = false;

    function getStatistics() {
        var url = common.modifyRelativeUrl("/SuspiciousCustomerDetails/GetStatistics");

        $.get(url, generalParam, function (json) {
            summary.DataTable().rows.add(json.Summaries).draw();
            choice.DataTable().rows.add(json.Choices).draw();
            topSport.DataTable().rows.add(json.Sports).draw();
            topBettype.DataTable().rows.add(json.Bettypes).draw();
        });
    }

    function openCloseGroup(target, fraudCase) {
        stopRender = true;

        var table = violatedTickets.DataTable();

        if (filter[fraudCase] != null) {
            $.fn.dataTable.ext.search[filter[fraudCase]] = () => true;
            filter[fraudCase] = null;

            if ($(target).is("span")) {
                $(target).removeClass("icon-chevron-down").addClass("icon-chevron-up");
            } else {
                $("span", target).removeClass("icon-chevron-down").addClass("icon-chevron-up");
            }
        }
        else {
            var n = $.fn.dataTable.ext.search.push(function (settings, data, idx) {
                return $(table.row(idx).node()).attr("data-fraudcase") != fraudCase;
            });

            if ($(target).is("span")) {
                $(target).addClass("icon-chevron-down").removeClass("icon-chevron-up");
            }
            else {
                $("span", target).addClass("icon-chevron-down").removeClass("icon-chevron-up");
            }

            filter[fraudCase] = n - 1;
        }

        table.draw(false);
    }

    summary.DataTable({
        ordering: false,
        searching: false,
        info: false,
        paging: false,
        processing: true,
        dom: "tr",
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
            { targets: "_all", defaultContent: "-", className: "text-right"}
        ],
        columns: [
            { data: obj => common.formatNumber(obj.SumTO) },
            { data: obj => obj.SumWL > 0 ? common.formatNumber(obj.SumWL) : `<span class="text-red">${common.formatNumber(obj.SumWL)}</span>` },
            { data: obj => obj.Margin > 0 ? common.formatNumber(obj.Margin) : `<span class="text-red">${common.formatNumber(obj.Margin)}</span>` },
            { data: obj => obj.BetCount }
        ]
    });

    choice.DataTable({
        ordering: false,
        searching: false,
        info: false,
        paging: false,
        processing: true,
        dom: "tr",
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
            { targets: [1, 2, 3], className: "text-right" }
        ],
        columns: [
            { data: obj => obj.EventChoice },
            { data: obj => common.formatNumber(obj.SumTO) },
            { data: obj => obj.SumWL > 0 ? common.formatNumber(obj.SumWL) : `<span class="text-red">${common.formatNumber(obj.SumWL)}</span>` },
            { data: obj => obj.Margin > 0 ? common.formatNumber(obj.Margin) : `<span class="text-red">${common.formatNumber(obj.Margin)}</span>` }
        ]
    });

    topSport.DataTable({
        ordering: false,
        searching: false,
        info: false,
        paging: false,
        processing: true,
        dom: "tr",
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
            { targets: [1, 2, 3], className: "text-right" }
        ],
        columns: [
            { data: obj => obj.SportName },
            { data: obj => common.formatNumber(obj.SumTO) },
            { data: obj => obj.SumWL > 0 ? common.formatNumber(obj.SumWL) : `<span class="text-red">${common.formatNumber(obj.SumWL)}</span>` },
            { data: obj => obj.Margin > 0 ? common.formatNumber(obj.Margin) : `<span class="text-red">${common.formatNumber(obj.Margin)}</span>` }
        ]
    });

    topBettype.DataTable({
        ordering: false,
        searching: false,
        info: false,
        paging: false,
        processing: true,
        dom: "tr",
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
            { targets: [1, 2, 3], className: "text-right" }
        ],
        columns: [
            { data: obj => obj.BetTypeName },
            { data: obj => common.formatNumber(obj.SumTO) },
            { data: obj => obj.SumWL > 0 ? common.formatNumber(obj.SumWL) : `<span class="text-red">${common.formatNumber(obj.SumWL)}</span>` },
            { data: obj => obj.Margin > 0 ? common.formatNumber(obj.Margin) : `<span class="text-red">${common.formatNumber(obj.Margin)}</span>` }
        ]
    });

    violatedTickets.DataTable({
        ordering: false,
        info: false,
        paging: false,
        processing: true,
        dom: "tr",
        language: {
            lengthMenu: "Page size _MENU_",
            paginate: {
                previous: "Back"
            },
            info: "Displaying _START_ to _END_ of _TOTAL_ item(s)",
            infoEmpty: "",
            zeroRecords: "There's no data",
            processing: "loading..."
        },
        scrollY: "30vh",
        scrollCollapse: true,
        ajax: common.modifyRelativeUrl(`/SuspiciousCustomerDetails/GetViolatedTickets?custid=${generalParam2.custId}&from=${generalParam2.from}&to=${generalParam2.to}`),
        columnDefs: [
            { targets: "_all", defaultContent: "-" },
            { targets: [-3, -4, 3], className: "text-right" },
            { targets: [0, 1, 2, 4, -2], className: "text-center" }
        ],
        columns: [
            { data: (obj, typet, set, meta) => obj.value == null ? null : ++ticketIdx, width: "40px" },
            { data: obj => obj.value != null ? obj.value.TransDate : null },
            { data: obj => obj.value != null ? obj.value.BetID : null },
            { data: obj => obj.value != null ? obj.value.Choice : null },
            { data: obj => obj.value != null ? `<div class='row-ht'>HT: ${obj.value.Result.HTHomeScore} - ${obj.value.Result.HTAwayScore}</div><div class='row-ft'>FT: ${obj.value.Result.FinalHomeScore} - ${obj.value.Result.FinalAwayScore}</div>` : null },
            { data: obj => obj.value != null ? obj.value.Odds : null },
            { data: obj => obj.value != null ? common.formatGridNumber(obj.value.Stake) : null },
            { data: obj => obj.value != null ? common.formatGridNumber(obj.value.WinLoss) : null },
            { data: obj => obj.value != null ? obj.value.Status : null },
            { data: obj => obj.value != null ? obj.value.IP : null}
        ],
        createdRow: function (row, obj) {
            if (obj.value == null) {
                var td = $(`<td colspan='10' class='group'><span class='icon-chevron-up'></span> ${obj.key}</td>`);

                $(row).empty().html(td).click(e => openCloseGroup(e.target, obj.key));
            }
            else {
                $(row).attr("data-fraudcase", obj.key);
            }
        },
        drawCallback: function () {
            var n = violatedTickets.DataTable().rows().data().length;

            if (n > 0 && !stopRender) {
                $(".group").click();
            }
        }
    });

    getStatistics();

    $(".area").accordion({
        collapsible: true,
        activate: () => violatedTickets.DataTable().columns.adjust(),
        heightStyle: "content"
    })
}

$(suspiciousCustomerDetails);