function suspiciousCustomerDetails () {
    var summary = $("#summary"),
        choice = $("#choice"),
        topSport = $("#top-5-sport"),
        topBettype = $("#top-5-bettype"),
        violatedTickets = $("#violated-tickets");

    summary.DataTable({
        ordering: false,
        searching: false,
        info: false,
        paging: false,
        processing: true,
        dom: "t",
        language: {
            lengthMenu: "Page size _MENU_",
            paginate: {
                previous: "Back"
            },
            info: "Displaying _START_ to _END_ of _TOTAL_ item(s)",
            infoEmpty: "",
            zeroRecords: "There's no data"
        },
        ajax: {
            url: common.modifyRelativeUrl(`SuspiciousCustomerDetails/GetSummary?custid=${custid}&from=${from}&to=${to}`)
        },
        columnDefs: [
            { targets: "_all", defaultContent: "-", className: "text-right"}
        ],
        columns: [
            { data: obj => common.formatNumber(obj.Turnover) },
            { data: obj => obj.WinLoss > 0 ? common.formatNumber(obj.WinLoss) : `<span class="text-red">${common.formatNumber(obj.WinLoss)}</span>` },
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
        dom: "t",
        language: {
            lengthMenu: "Page size _MENU_",
            paginate: {
                previous: "Back"
            },
            info: "Displaying _START_ to _END_ of _TOTAL_ item(s)",
            infoEmpty: "",
            zeroRecords: "There's no data"
        },
        ajax: {
            url: common.modifyRelativeUrl(`SuspiciousCustomerDetails/GetChoice?custid=${custid}&from=${from}&to=${to}`)
        },
        columnDefs: [
            { targets: [1, 2, 3], className: "text-right" }
        ],
        columns: [
            { data: obj => obj.Choice },
            { data: obj => common.formatNumber(obj.Turnover) },
            { data: obj => obj.WinLoss > 0 ? common.formatNumber(obj.WinLoss) : `<span class="text-red">${common.formatNumber(obj.WinLoss)}</span>` },
            { data: obj => obj.Margin > 0 ? common.formatNumber(obj.Margin) : `<span class="text-red">${common.formatNumber(obj.Margin)}</span>` }
        ]
    });

    topSport.DataTable({
        ordering: false,
        searching: false,
        info: false,
        paging: false,
        processing: true,
        dom: "t",
        language: {
            lengthMenu: "Page size _MENU_",
            paginate: {
                previous: "Back"
            },
            info: "Displaying _START_ to _END_ of _TOTAL_ item(s)",
            infoEmpty: "",
            zeroRecords: "There's no data"
        },
        ajax: {
            url: common.modifyRelativeUrl(`SuspiciousCustomerDetails/GetTop5Sports?custid=${custid}&from=${from}&to=${to}`)
        },
        columnDefs: [
            { targets: [1, 2, 3], className: "text-right" }
        ],
        columns: [
            { data: obj => obj.SportName },
            { data: obj => common.formatNumber(obj.Turnover) },
            { data: obj => obj.WinLoss > 0 ? common.formatNumber(obj.WinLoss) : `<span class="text-red">${common.formatNumber(obj.WinLoss)}</span>` },
            { data: obj => obj.Margin > 0 ? common.formatNumber(obj.Margin) : `<span class="text-red">${common.formatNumber(obj.Margin)}</span>` }
        ]
    });

    topBettype.DataTable({
        ordering: false,
        searching: false,
        info: false,
        paging: false,
        processing: true,
        dom: "t",
        language: {
            lengthMenu: "Page size _MENU_",
            paginate: {
                previous: "Back"
            },
            info: "Displaying _START_ to _END_ of _TOTAL_ item(s)",
            infoEmpty: "",
            zeroRecords: "There's no data"
        },
        ajax: {
            url: common.modifyRelativeUrl(`SuspiciousCustomerDetails/GetTop5Bettypes?custid=${custid}&from=${from}&to=${to}`)
        },
        columnDefs: [
            { targets: [1, 2, 3], className: "text-right" }
        ],
        columns: [
            { data: obj => obj.BetTypeName },
            { data: obj => common.formatNumber(obj.Turnover) },
            { data: obj => obj.WinLoss > 0 ? common.formatNumber(obj.WinLoss) : `<span class="text-red">${common.formatNumber(obj.WinLoss)}</span>` },
            { data: obj => obj.Margin > 0 ? common.formatNumber(obj.Margin) : `<span class="text-red">${common.formatNumber(obj.Margin)}</span>` }
        ]
    });

    violatedTickets.DataTable({
        ordering: false,
        searching: false,
        info: false,
        paging: false,
        processing: true,
        dom: "t",
        language: {
            lengthMenu: "Page size _MENU_",
            paginate: {
                previous: "Back"
            },
            info: "Displaying _START_ to _END_ of _TOTAL_ item(s)",
            infoEmpty: "",
            zeroRecords: "There's no data"
        }
    });
}

$(suspiciousCustomerDetails);