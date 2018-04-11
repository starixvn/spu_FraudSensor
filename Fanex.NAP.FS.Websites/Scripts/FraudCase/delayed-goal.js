function delayedGoal() {
    var customer_list = $("#customer-list");
    var view_link = $(".view-list");
    var table = $("#delayed-goal-customer-list");

    customer_list.dialog({
        height: "auto",
        width: 900,
        modal: true,
        autoOpen: false,
        resizable: false,
        draggable: false
    });

    view_link.click(function () {
        customer_list.dialog("open");
        table.DataTable().ajax.reload();
    });

    table.DataTable({
        ordering: false,
        lengthMenu: [10, 20, 50, 100, 500],
        pageLength: 50,
        processing: true,
        ajax: common.modifyRelativeUrl("/FraudCase/GetDelayedGoalCustomerList"),
        columns: [
            { data: "rowNum", className: "text-center" },
            { data: "data.Name" },
            { data: d => `<span title='${d.data.Creator}'>${common.makeWrapString(d.data.Creator, 40)}</span>`},
            { data: "data.AddedDate", className: "text-center" },
            { defaultContent: "<span class='icon-delete'></span>", className: "has-icon text-center", createdCell: c => $("span", c).click(deleteFlow) }
        ],
        scrollY: '40vh',
        scrollCollapse: true,
        dom: "t<'pagination'<'page-size'lp>i>",
        language: {
            lengthMenu: "Page size _MENU_",
            paginate: {
                previous: "Back"
            },
            info: "Displaying _START_ to _END_ of _TOTAL_ item(s)",
            infoEmpty: ""
        }
    });

    $("#add-user-btn").click(addNewUser);

    $("#export-excel").click(exportData);

    table.DataTable().columns([1]).every(function () {
        var column = this;

        $("#username-search").keyup(function () {
            if (column.search() !== this.value) {
                column.search(this.value).draw();
            }
        })
    })

    function deleteFlow() {
        var $table = table.DataTable();

        console.log($table.row($(this).parents("tr")).data());

        var $row = $table.row($(this).parents("tr"));

        var row = $row.data();

        common.showConfirmation("Do you want to delete customer " + row.data.Name + "?", () => deleteCustomer(row.data, $row));
    }

    function deleteCustomer(data) {
        var url = common.modifyRelativeUrl("/FraudCase/DeleteDelayedGoalCustomer");

        var $table = table.DataTable();

        $.post({
            url: url,
            data: { custId: data.Id },
            success: showResponse
        })
    }

    function addNewUser() {
        var input = $("#username");
        var username = input.val();

        if (common.hasSpecialCharacter(username)) {
            common.showError("Invalid input");
            return;
        }

        if (username != null && username.length != 0) {
            var url = common.modifyRelativeUrl("/FraudCase/AddDelayedGoalCustomer");

            $.post({
                url: url,
                data: { username: username },
                success: showResponse
            })
        }
    }

    function showResponse(data) {
        if (data.ErrorCode != 0) {
            common.showError(data.ErrorMsg);
        }
        else {
            common.showSuccess();
            table.DataTable().ajax.reload();
        }
    }

    function exportData() {
        var url = "/FraudCase/ExportDelayedGoalCustomer";

        common.downloadFile(url, { searchString: $("#username-search").val()})
    }
}

$(delayedGoal);