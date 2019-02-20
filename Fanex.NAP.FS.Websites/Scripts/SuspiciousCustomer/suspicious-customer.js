function suspicious_customer() {
    var from = $("#event_date"),
        to = $("#event_date2"),
        fraud_case = $("#fraud_case"),
        is_verified = $("#is_verified"),
        classification = $("#classification"),
        assignee = $("#assignee"),
        customer_table = $("#customer_grid table"),
        date_type = $("#date_type"),
        note_popup = $("#note_popup");

    var first_time = true;

    var saved_dto = null;

    var table = null;

    $("table", note_popup).DataTable({
        ordering: false,
        searching: false,
        lengthMenu: [10, 20, 50, 100, 500],
        pageLength: 50,
        scrollCollapse: true,
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
        columnDefs: [
            { defaultContent: "-", targets: "_all" }
        ],
        columns: [
            { data: (obj, typet, set, meta) => meta.row + 1, className: "text-center", width: "40px" },
            { data: obj => obj.Date, className: "text-center", width: "150px" },
            { data: obj => `<span title="${obj.Description}">${common.makeWrapString(obj.Description, 50)}</span>`, width: "500px" },
            { data: obj => obj.Author }
        ]
    })

    note_popup.draggable({
        containment: "parent"
    });

    from.datepicker({
        dateFormat: "M dd, yy",
        changeMonth: true,
        changeYear: true,
        firstDay: 1,
        minDate: "-1y +1d"
    });

    to.datepicker({
        dateFormat: "M dd, yy",
        changeMonth: true,
        changeYear: true,
        firstDay: 1
    });

    get_fraud_case();

    get_assignee();

    assignee.select2();

    $([date_type, fraud_case, is_verified, classification]).select2({
        minimumResultsForSearch: -1
    });        

    $(".-primary").click(get_customers);

    date_type.change(restrict_date_range);

    restrict_date_range();

    from.change(restrict_to_date);

    to.change(restrict_from_date);

    $(".-export").click(export_excel);

    $(".icon-calendar-text").click(e => $(e.target).prev().focus());

    $(document).click(function (e) {
        var target = $(e.target);

        var inside_note_click = target.is(note_popup) || target.parents("#note_popup").length != 0;

        if (!inside_note_click) {
            note_popup.hide();
        }
    })

    function get_customers() {
        var dto = {
            Username: $("#member").val(),
            FromDate: from.val(),
            ToDate: to.val(),
            FraudCaseId: fraud_case.val(),
            IsVerified: is_verified.val(),
            AssigneeId: assignee.val(),
            Classification: classification.val()
        }

        saved_dto = dto;

        var urlAction = common.modifyRelativeUrl("/suspiciouscustomer/Generate");

        //if (table != null) {
        //    table.destroy();
        //}

        table = customer_table.DataTable({
            destroy: true,
            serverSide: true, 
            processing: false,
            stateSave: true,
                ajax: {
                    url: urlAction,
                    data: dto,
                    type: "POST",
                    beforeSend: function () {
                        common.showLoading();
                    },
                    complete: function () {
                        common.closeLoading(); 
                        
                    }
                },                
                ordering: false,
                searching: false,
                lengthMenu: [10, 20, 50, 100, 500],
                pageLength: 50,
                scrollY: "70vh",
                scrollCollapse: true,
                dom: "t<'pagination'<'page-size'lp>i>",
                language: {
                    lengthMenu: "Page size _MENU_",
                    paginate: {
                        previous: "Back"
                    },
                    info: "Displaying _START_ to _END_ of _TOTAL_ item(s)",
                    infoEmpty: "",
                    zeroRecords: "There's no data",
                    processing: 'Loading...'
                },
                columns: [                    
                    { data: obj => obj.Counter, className: "text-center", width: "40px" },
                    { data: build_classification, className: "has-icon text-center list-icon", width: "120px" },
                    { data: build_note, className: "has-icon text-center", width: "90px" },
                    { data: obj => obj.IsVerified ? '<span class="icon-approval"></span>' : null, className: "has-icon text-center", width: "90px" },
                    { data: obj => `<a href="${common.modifyRelativeUrl(`/SuspiciousCustomerDetails?custId=${obj.CustId}&from=${from.val()}&to=${to.val()}&custname=${obj.Username}`)}" target='_blank'>${obj.Username}</a>`, width: "300px" },
                    { data: obj => obj.TotalViolation },
                    { data: obj => obj.ViolatedCases },
                    { data: obj => obj.BetCount },
                    { data: obj => obj.Assignee }
                ],
                columnDefs: [
                    { defaultContent: "-", targets: "_all" },
                    { targets: [-2, -3, -4], className: "text-right", width: "120px" }
                    ]
                }
        );
               
        table.on('click','span.icon-note-text', function(event) {
            create_popup(event, this);            
        });

        $("#customer_grid").show();
                    
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
            assignee.append("<option>All</option>").append("<option value='-1'>None</option>");

            for (var i = 0; i < json.length; i++) {
                assignee.append(`<option value='${json[i].Id}'>${json[i].Username}</option>`)
            }
        })
    }

    function build_note(obj) {
        return obj.HasNote ? '<span class="icon-note-text icon-note" custid="' + obj.CustId + '" username="' + obj.Username + '"></span>' : null;
    }

    function build_classification(obj) {
        var result = "";

        if (obj.IsDirty) result += '<span class="icon-dirty"></span>';

        if (obj.IsDelinquent) result += '<span class="icon-delinquency"></span>';

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

        from.datepicker("option", "minDate", moment(to_val).add(-1, "y").toDate());

        date_type.val(7).change();
    }

    function restrict_to_date() {
        var from_val = from.datepicker("getDate");

        to.datepicker("option", "minDate", from_val);

        date_type.val(7).change();
    }

    function export_excel() {
        var url = common.modifyRelativeUrl("/suspiciouscustomer/ExportExcel");

        var dto = {};

        if (saved_dto == null) {
            dto = {
                Username: $("#member").val(),
                FromDate: from.val(),
                ToDate: to.val(),
                FraudCaseId: fraud_case.val(),
                IsVerified: parseInt(is_verified.val()),
                AssigneeId: assignee.val(),
                Classification: classification.val()
            }
        } else {
            dto = Object.assign({}, saved_dto);
        }

        dto.fraudCaseName = fraud_case.find(":selected").text();

        dto.assignee = assignee.find(":selected").text();

        common.downloadFile(url, dto);
    }

    function create_popup(e, data) {
        var url = common.modifyRelativeUrl("/SuspiciousCustomer/GetSuspiciousCustomerNote");
        var custid = $(data).attr("custid");
        var username = $(data).attr("username");       

        $.get(url, { custId: custid }, function (json) {
            $("table", note_popup).DataTable().clear().rows.add(json).draw();

            $("table", note_popup).DataTable().columns.adjust();

            note_popup.css({ top: e.clientY, left: e.clientX, position: "absolute" });

            $("h2", note_popup).text(username);

            note_popup.show();
        });
    }
}

$(suspicious_customer)