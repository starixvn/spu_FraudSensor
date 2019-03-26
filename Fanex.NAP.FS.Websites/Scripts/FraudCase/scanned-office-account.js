function scanned_office_account() {
    var internal_account_dialog = $("#internal-account-list");
    var internal_account_table = $('#internal-account-table', internal_account_dialog);

    $("#internal-account-link").click(function () {
        internal_account_dialog.dialog("open");
        internal_account_table.DataTable().columns.adjust();
        internal_account_table.DataTable().ajax.reload();
        $("#username-search").val("").keyup();
        $("#username").val("");
    });

    $("#add-account-btn").click(add_internal_account);

    $("#export-excel-account").click(export_excel);

    internal_account_dialog.dialog({
        height: "auto",
        width: 900,
        modal: true,
        autoOpen: false,
        resizable: false,
        draggable: false
    });

    internal_account_table.DataTable({
        ordering: true,
        lengthMenu: [10, 20, 50, 100, 500],
        pageLength: 50,
        processing: true,
        ajax: common.modifyRelativeUrl("/FraudCase/GetAllInternalAccount"),
        columnDefs: [
            { targets: [-1], orderable: false }
        ],
        columns: [
            { data: (row, type, set, meta) => meta.row + 1, className: "text-center", width: "40px" },
            { data: obj => obj.UserName, width: "150px" },
            { data: obj => obj.Level, width: "150px" },
            { data: obj => obj.Currency, width: "150px" },
            { defaultContent: "<span class='icon-delete'></span>", className: "has-icon text-center", createdCell: c => $("span", c).click(deleteFlow), width: "40px" }
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
            infoEmpty: "",
            zeroRecords: "There's no data"
        }        
    });
    
    internal_account_table.DataTable().columns([1]).every(function () {
        var column = this;

        $("#username-search").keyup(function () {
            if (column.search() !== this.value) {
                column.search(this.value).draw();
            }
        })         
    })

    function deleteFlow() {
        var $table = internal_account_table.DataTable();
        var $row = $table.row($(this).parents("tr"));
        var row = $row.data();

        common.showConfirmation("Do you want to delete member " + row.UserName + "?", () => deleteInternalAccount(row, $row));
    }

    function deleteInternalAccount(data) {
        var url = common.modifyRelativeUrl("/FraudCase/DeleteInternalAccount");

        $.post({
            url: url,
            data: { custId: data.custid },
            success: showResponse
        })
    }

    function add_internal_account() {
        var username = $("#username").val();

        if (common.hasSpecialCharacter(username)) {
            common.showError("Invalid input");
            return;
        }

        if (username != null && username.length != 0) {
            var url = common.modifyRelativeUrl("/FraudCase/AddInternalAccount");

            common.showLoading();
            $.post({
                url: url,
                data: {username: username},
                success: function (data) {
                    showResponse(data);
                    common.closeLoading();
                }
            })
        }
    }

    function showResponse(data){
        if (data.ErrorCode != 0) {
            var msg = data.ErrorMsg !== null ? data.ErrorMsg : ("Error code: " + data.ErrorCode);
            common.showError(msg);
        } else {
            common.showSuccess();
            internal_account_table.DataTable().ajax.reload();
        }
    }

    function export_excel() {
        var url = common.modifyRelativeUrl("/FraudCase/ExportExcelInternalAccount");

        common.downloadFile(url, {});
    }
};

$(scanned_office_account);