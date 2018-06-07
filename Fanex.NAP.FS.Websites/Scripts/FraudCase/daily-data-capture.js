function daily_data_capture() {
    var 
        dialog = $("#user-list"),
        user_table = $("#user-table", dialog);

    function add_recipient() {
        var data = $("#user").select2("data");

        if (data == null || data.length == 0) {
            common.showError("Recipient list can't be empty!");
            return;
        }

        var url = common.modifyRelativeUrl("/Fraudcase/AddRecipients");

        $.post(url, JSON.stringify({ recipients: data.map(obj => obj.id).join(",") }), common.handleUpdateAjax(), "json");
    }

    function remove_recipient_event(target) {
        $("span", target).click(function () {
            var obj = user_table.DataTable().row(target).data();

            common.showConfirmation(`Do you want to remove ${obj.Recipient}?`, function () {
                var url = common.modifyRelativeUrl("/Fraudcase/RemoveRecipient");

                $.get(url, { id: obj.RecipientId }, common.handleUpdateAjax());
            });
        })
    }

    dialog.dialog({
        height: "auto",
        width: 650,
        modal: true,
        autoOpen: false,
        resizable: false,
        draggable: false
    });

    $("#user-link").click(function () {
        dialog.dialog("open");
        user_table.DataTable().ajax.reload();
        user_table.DataTable().columns.adjust();
        $("#user").val(null).change();
    });

    user_table.DataTable({
        ordering: false,
        lengthMenu: [10, 20, 50, 100, 500],
        pageLength: 50,
        processing: true,
        ajax: common.modifyRelativeUrl("/FraudCase/GetRecipients"),
        scrollY: '40vh',
        scrollCollapse: true,
        dom: "t<'pagination'<'page-size'lp>i>",
        language: common.language(),
        columnDefs: [
            { defaultContent: "-" },
            { targets: [1], width: "100px" }
        ],
        columns: [
            { data: (row, type, set, meta) => meta.row + 1, className: "text-center", width: "40px" },
            { data: obj => obj.Recipient },
            { defaultContent: "<span class='icon-delete'></span>", className: "has-icon text-center", createdCell: remove_recipient_event, width: "40px" }
        ]
    });

    $("#add-recipient-btn").click(add_recipient);

    $("select", dialog).select2({
        ajax: {
            url: common.modifyRelativeUrl("/SuspiciousCustomer/GetUsers"),
            processResults: function (json) {
                return {
                    results: json.map(d => ({ id: d.Id, text: d.Username }))
                };
            },
        }
    })
}

$(daily_data_capture);