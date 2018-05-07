function suspiciousCustomerDetails () {
    var summary = $("#summary"),
        choice = $("#choice"),
        topSport = $("#top-5-sport"),
        topBettype = $("#top-5-bettype");

    summary.DataTable({
        ordering: false,
        searching: false,
        info: false,
        paging: false,
        dom: "t<'pagination>",
        language: {
            lengthMenu: "Page size _MENU_",
            paginate: {
                previous: "Back"
            },
            info: "Displaying _START_ to _END_ of _TOTAL_ item(s)",
            infoEmpty: "",
            zeroRecords: "There's no data"
        },
    });
}

$(suspiciousCustomerDetails);