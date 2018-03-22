function suspicious_customer() {
    var from = $("#event_date"),
        to = $("#event_date2"),
        fraud_case = $("#fraud_case"),
        is_verified = $("#is_verified"),
        classification = $("#classification"),
        assignee = $("#assignee"),
        customer_table = $("table"),
        date_type = $("#date_type");

    var first_time = true;

    var saved_dto = null;

    from.datepicker({
        dateFormat: "M dd, yy",
        changeMonth: true,
        changeYear: true,
        firstDay: 1,
        minDate: "-1y"
    });

    to.datepicker({
        dateFormat: "M dd, yy",
        changeMonth: true,
        changeYear: true,
        firstDay: 1,
        minDate: "-1y"
    });

    get_fraud_case();

    get_assignee();

    assignee.select2();

    $([date_type, fraud_case, is_verified, classification]).select2({
        minimumResultsForSearch: -1
    });

    customer_table.DataTable({
        ordering: false,
        searching: false,
        lengthMenu: [10, 20, 50, 100, 500],
        scrollY: "70vh",
        scrollCollapse: true,
        autoWidth: false,
        dom: "t<'pagination'<'page-size'lp>i>",
        language: {
            lengthMenu: "Page size _MENU_",
            paginate: {
                previous: "Back"
            },
            info: "Displaying _START_ to _END_ of _TOTAL_ item(s)",
            infoEmpty: ""
        },
        columns: [
            { data: (obj, typet, set, meta) => meta.row + 1, className: "text-center" },
            { data: build_classification, className: "has-icon text-center list-icon" },
            { data: obj => obj.HasNote ? '<span class="icon-note-text icon-note"></span>' : null, className: "has-icon text-center" },
            { data: obj => obj.IsVerified ? '<span class="icon-approval"></span>' : null, className: "has-icon text-center" },
            { data: obj => `<a>${obj.Username}</a>` },
            { data: obj => obj.Evidence },
            { data: obj => obj.TotalViolation },
            { data: obj => obj.ViolatedCases },
            { data: obj => obj.BetCount },
            { data: obj => obj.Assignee }
        ],
        columnDefs: [
            { defaultContent: "-", targets: "_all" }
        ]
    })

    $(".-primary").click(get_customers);

    date_type.change(restrict_date_range);

    restrict_date_range();

    from.change(restrict_to_date);
    to.change(restrict_from_date);

    $(".-export").click(export_excel);

    function get_customers() {
        common.showLoading();

        var dto = {
            Username: $("#member").val(),
            FromDate: from.val(),
            ToDate: to.val(),
            FraudCaseId: fraud_case.val(),
            IsVerified: parseInt(is_verified.val()),
            AssigneedId: assignee.val()
        }

        saved_dto = dto;

        var url = common.modifyRelativeUrl("/suspiciouscustomer/Generate");

        $.get(url, dto, function (json) {
            customer_table.DataTable().clear().rows.add(json).draw();
            $("#customer_grid").show();

            if (first_time) {
                customer_table.DataTable().draw();
                first_time = false;
            }

            common.closeLoading();
        });
    }

    function get_fraud_case() {
        var url = common.modifyRelativeUrl("/suspiciouscustomer/GetFraudCases");

        $.get(url, {}, function (json) {
            fraud_case.append("<option value='0'>All</option>");

            for (var i = 0; i < json.length; i++) {
                fraud_case.append(`<option value="${json[i].FraudCaseId}">${json[i].FraudCaseName}</option>`)
            }
        });
    }

    function get_assignee() {
        var url = common.modifyRelativeUrl("/suspiciouscustomer/GetUsers");

        $.get(url, {}, function (json) {
            assignee.append("<option value='0'>All</option>");

            for (var i = 0; i < json.length; i++) {
                assignee.append(`<option value='${json[i].Id}'>${json[i].Username}</option>`)
            }
        })
    }

    function build_classification(obj) {
        var result = "";

        if (obj.IsDirty) result += '<span class="icon-dirty"></span>';

        if (obj.IsDelinquency) result += '<span class="icon - delinquency"></span>';

        if (obj.IsHedge) result += '<span class="icon-hedge"></span>';

        return result;
    }

    function restrict_date_range() {
        var type = parseInt(date_type.val());

        var from_date, to_date;

        if (type == 0) { 
            from_date = moment().toDate();
            to_date = from_date;
        } //today
        else if (type == 1) {
            var m = moment();

            m.set("date", m.get("date") - 1);

            from_date = m.toDate();
            to_date = from_date;
        } // yesterday
        else if (type == 2) {
            from_date = moment().day(1).toDate();
            to_date = moment().day(7).toDate();
        } // this week
        else if (type == 3) {
            from_date = moment().day(-6).toDate();
            to_date = moment().day(0).toDate();
        } // last week
        else if (type == 4) {
            from_date = moment().date(1).toDate();
            to_date = moment().date(moment().daysInMonth()).toDate();
        } // this month
        else if (type == 5) {
            var m = moment();

            m.set("date", 1);
            m.set("month", m.get("month") - 1);

            from_date = m.toDate();

            to_date = m.date(m.daysInMonth()).toDate();
        } // last month
        else if (type == 6) {
            var m = moment();

            m.set("date", 1);
            m.set("month", m.get("month") - 1);

            from_date = m.toDate();
            to_date = moment().toDate();
        } // since last month

        if (from_date != null && to_date != null) {
            from.datepicker("setDate", from_date);
            to.datepicker("setDate", to_date);
            from.datepicker("option", "maxDate", null);
            to.datepicker("option", "minDate", "-1y");
        }
    }

    function restrict_from_date() {
        var to_val = to.datepicker("getDate");

        from.datepicker("option", "maxDate", to_val);

        date_type.val(7).change();
    }

    function restrict_to_date() {
        var from_val = from.datepicker("getDate");

        to.datepicker("option", "minDate", from_val);

        date_type.val(7).change();
    }

    function export_excel() {
        var url = common.modifyRelativeUrl("insert url here");
        throw "insert url";
        common.downloadFile(url, saved_dto);
    }
}

$(suspicious_customer)