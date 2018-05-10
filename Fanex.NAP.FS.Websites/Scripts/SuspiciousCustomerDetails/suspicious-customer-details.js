function suspiciousCustomerDetails() {
    var summary = $("#summary"),
        choice = $("#choice"),
        topSport = $("#top-5-sport"),
        topBettype = $("#top-5-bettype"),
        violatedTickets = $("#violated-tickets"),
        saveBtn = $("#save-btn"),
        logNote = $("#log-note"),
        fraudCase = $("#fraudcase");

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

    function initSelect() {
        var url = common.modifyRelativeUrl("/SuspiciousCustomer/GetUsers");

        $.get(url, {}, function (data) {
            data.unshift({ Username: "Unassign", Id: "null" });

            $("select").select2({
                data: data.map(d => ({ id: d.Id, text: d.Username, selected: d.Username == assignee }))
            }).on("select2:select", function (e) {
                var d = e.params.data;

                if (d.text != assignee) {
                    saveBtn.prop("disabled", false);
                }
                else {
                    saveBtn.prop("disabled", true);
                }
            });
        });
    }

    function deleteNote(obj) {
        common.showConfirmation("Do you want to delete this log", function () {
            var url = common.modifyRelativeUrl("/SuspiciousCustomerDetails/RemoveLogNote");

            $.get(url, { custId: generalParam.custId, customerLogId: obj.CustomerLogId }, common.handleUpdateAjax(null, () => logNote.DataTable().ajax.reload()));
        })
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
            { targets: "_all", defaultContent: "-", className: "text-right" }
        ],
        columns: [
            { data: obj => common.formatNumber(obj.SumTO) },
            { data: obj => obj.SumWL >= 0 ? common.formatNumber(obj.SumWL) : `<span class="text-red">${common.formatNumber(obj.SumWL)}</span>` },
            { data: obj => obj.Margin >= 0 ? common.formatNumber(obj.Margin) : `<span class="text-red">${common.formatNumber(obj.Margin)}</span>` },
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
            { data: obj => obj.SumWL >= 0 ? common.formatNumber(obj.SumWL) : `<span class="text-red">${common.formatNumber(obj.SumWL)}</span>` },
            { data: obj => obj.Margin >= 0 ? common.formatNumber(obj.Margin) : `<span class="text-red">${common.formatNumber(obj.Margin)}</span>` }
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
            { data: obj => obj.SumWL >= 0 ? common.formatNumber(obj.SumWL) : `<span class="text-red">${common.formatNumber(obj.SumWL)}</span>` },
            { data: obj => obj.Margin >= 0 ? common.formatNumber(obj.Margin) : `<span class="text-red">${common.formatNumber(obj.Margin)}</span>` }
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
            { data: obj => obj.SumWL >= 0 ? common.formatNumber(obj.SumWL) : `<span class="text-red">${common.formatNumber(obj.SumWL)}</span>` },
            { data: obj => obj.Margin >= 0 ? common.formatNumber(obj.Margin) : `<span class="text-red">${common.formatNumber(obj.Margin)}</span>` }
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
            zeroRecords: "There's no data"
        },
        scrollY: "30vh",
        scrollCollapse: true,
        ajax: common.modifyRelativeUrl(`/SuspiciousCustomerDetails/GetViolatedTickets?custid=${generalParam.custId}&from=${generalParam.from}&to=${generalParam.to}`),
        columnDefs: [
            { targets: "_all", defaultContent: "-" },
            { targets: [-3, -4, -5, 3], className: "text-right" },
            { targets: [0, 1, 2, 4, -2], className: "text-center" }
        ],
        columns: [
            { data: (obj, typet, set, meta) => obj.value == null ? null : ++ticketIdx, width: "40px" },
            { data: obj => obj.value != null ? obj.value.TransDate : null },
            { data: obj => obj.value != null ? obj.value.BetID : null },
            { data: obj => obj.value != null ? obj.value.Choice : null },
            { data: obj => obj.value != null ? `<div class='row-ht'>HT: ${obj.value.Result.HTHomeScore} - ${obj.value.Result.HTAwayScore}</div><div class='row-ft' style="font-weight: bold">FT: ${obj.value.Result.FinalHomeScore} - ${obj.value.Result.FinalAwayScore}</div>` : null },
            { data: obj => obj.value != null ? obj.value.Odds : null },
            { data: obj => obj.value != null ? common.formatGridNumber(obj.value.Stake) : null },
            { data: obj => obj.value != null ? common.formatGridNumber(obj.value.WinLoss) : null },
            { data: obj => obj.value != null ? obj.value.Status : null },
            { data: obj => obj.value != null ? obj.value.IP : null }
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

            if (n >= 0 && !stopRender) {
                $(".group").click();
            }
        }
    });

    logNote.DataTable({
        ordering: false,
        info: false,
        paging: false,
        processing: true,
        searching: false,
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
        scrollY: "30vh",
        scrollCollapse: true,
        ajax: common.modifyRelativeUrl(`/SuspiciousCustomerDetails/GetCustomerLogNotes?custid=${generalParam.custId}`),
        columnDefs: [
            { targets: "_all", defaultContent: "-" },
            { targets: [0, -2], className: "text-center" },
            { targets: [0], width: "200px" },
            { targets: [-1, -2], width: "40px" }
        ],
        columns: [
            { data: obj => obj.LogDateString },
            { data: obj => obj.Description },
            { data: obj => obj.Author, width: "100px" },
            { data: obj => obj.Type },
            { data: obj => "<span class='icon-delete text-red' style='cursor: pointer'></span>", className: "text-center has-icon" }
        ],
        drawCallback: function () {
            var table = logNote.DataTable();

            $(".icon-delete").click(e => deleteNote(table.row($(e.target).parents("tr")).data()));
        }
    });

    fraudCase.DataTable({
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
            zeroRecords: "There's no data"
        },
        scrollCollapse: true,
        ajax: common.modifyRelativeUrl(`/SuspiciousCustomerDetails/GetFraudCases?custid=${generalParam.custId}`),
        columnDefs: [
            { targets: "_all", defaultContent: "-" },
            { targets: [-1, 1, 2, 3, 4], className: "text-center" },
            { targets: [1, 2, 3], width: "70px" }
        ],
        columns: [
            { data: obj => obj.FraudCaseName, width: "150px" },
            { data: obj => obj.Violations },
            { data: obj => obj.Verified },
            { data: obj => obj.Unverified },
            { data: obj => obj.LastVerifiedDate == null ? null : moment(new Date(parseInt(obj.LastVerifiedDate.substr(6)))).format("MMM DD, YYYY hh:mm:ss A"), width: "170px" },
            { data: obj => obj.LastVerifiedBy, width: "100px" },
            { data: obj => `<a class='${obj.Unverified != 0 ? "link" : ""} verify' data-fraudcase-id="${obj.FraudCaseId}" style="${obj.Unverified != 0 ? "color: #2097fd; cursor: pointer" : "color: inherit"}">Verify</span>`, width: "70px" }
        ],
        drawCallback: function () {
            var table = logNote.DataTable();

            $(".verify").click(function (e) {
                if ($(e.target).hasClass("link")) {
                    var p =
                        {
                            custId: generalParam.custId,
                            fraudCaseId: $(e.target).attr("data-fraudcase-id")
                        }

                    var url = common.modifyRelativeUrl("/SuspiciousCustomerDetails/UpdateCustomerFraudCaseVerify");

                    $.get(url, p, function (result) {
                        if (result.ErrorCode == 0) {
                            common.showSuccess();
                            logNote.DataTable().ajax.reload();
                            fraudCase.DataTable().ajax.reload();
                            $(e.target).removeClass("link").css("color", "inherit");
                        }
                        else {
                            common.showError(result.ErrorMsg);
                        }
                    })
                }
            });
        }
    })

    getStatistics();

    initSelect();

    $(".area").accordion({
        collapsible: true,
        heightStyle: "content",
        activate: function (e, ui) {
            var target = $(e.target);

            if (target.hasClass(".-1")) {
                fraudCase.DataTable().columns.adjust();
            }
            else {
                violatedTickets.DataTable().columns.adjust();
            }
        }
    });

    saveBtn.click(function (e) {
        var btn = $(e.target);

        var url = common.modifyRelativeUrl("/SuspiciousCustomerDetails/UpdateAssignee");

        $.get(url, { custId: generalParam.custId, assigneeId: $("select").val() }, function (result) {
            if (result.ErrorCode == 0) {
                btn.prop("disabled", true);
                assignee = $("select").select2("data")[0].text;
                common.showSuccess();
                logNote.DataTable().ajax.reload();
            }
            else {
                common.showError(result.ErrorMsg);
            }
        })
    });

    $("#add-btn").click(function () {
        var val = $("#note").val();

        if (val == null || val.length == 0 || common.hasSpecialCharacter(val)) {
            common.showError("Invalid input!");
        }
        else {
            var url = common.modifyRelativeUrl("/SuspiciousCustomerDetails/InsertLogNote");

            var p =
                {
                    custId: generalParam.custId,
                    logDesc: val
                };

            $.get(url, p, common.handleUpdateAjax(null, function () {
                logNote.DataTable().ajax.reload();
                $("#note").val("");
            }));
        }
    })

    $(".account-type input").change(function (e) {
        var target = $(e.target);

        var url = common.modifyRelativeUrl("/SuspiciousCustomerDetails/UpdateCustomerInfo");

        var p =
            {
                custId: generalParam.custId,
                isDelinquent: target.is("#delinquency") ? target.is(":checked") : null,
                isDirty: target.is("#dirty") ? target.is(":checked") : null,
                isHedge: target.is("#hedge") ? target.is(":checked") : null,
            }

        $.get(url, p, common.handleUpdateAjax(null, () => logNote.DataTable().ajax.reload()));
    })
}

$(suspiciousCustomerDetails);