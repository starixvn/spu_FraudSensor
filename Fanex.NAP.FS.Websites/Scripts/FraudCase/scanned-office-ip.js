function scanned_office_ip() {
    var ip_dialog = $("#ip-list"),
        ip_table = $("#ip-table", ip_dialog);

    function delete_ip_range(target) {
        $("span", target).click(function () {
            var obj = ip_table.DataTable().row(target).data();

            common.showConfirmation(`Do you want you delete IP range ${obj.FromIp} - ${obj.ToIp}`, function () {
                var url = common.modifyRelativeUrl("FraudCase/DeleteIpRange");

                $.get(url, obj, common.handleUpdateAjax(null, () => ip_table.DataTable().ajax.reload()));
            });
        });
    };

    function validate_ip(ip) {
        var arr = ip.split(".");

        return arr.length == 4 && arr.every(x => !isNaN(parseInt(x)) && parseInt(x).toString() == x && parseInt(x) <= 255);
    }

    function validate_ip_range(fromIp, toIp) {
        var from_arr = fromIp.split(".");
        var to_arr = toIp.split(".");

        var from_number = parseInt(from_arr[3]);
        var to_numer = parseInt(to_arr[3]);

        return from_arr.every((v, idx) => idx == 3 || v == to_arr[idx]) && from_number <= to_numer;
    }

    function add_ip_range() {
        var from = $("#from-ip").val();
        var to = $("#to-ip").val();

        if (!validate_ip(from) || !validate_ip(to)) {
            common.showError("Invalid IP!");
        }
        else if (!validate_ip_range(from, to)) {
            common.showError("Invalid IP range!");
        }
        else {
            var url = common.modifyRelativeUrl("/FraudCase/AddIpRange");

            $.get(url, { fromIP: from, toIp: to }, common.handleUpdateAjax(null, () => ip_table.DataTable().ajax.reload()));
        }
    }

    function export_excel() {
        var url = common.modifyRelativeUrl("/FraudCase/ExportExcelIp");

        common.downloadFile(url, {});
    }

    ip_dialog.dialog({
        height: "auto",
        width: 650,
        modal: true,
        autoOpen: false,
        resizable: false,
        draggable: false
    });

    $("#ip-link").click(function () {
        ip_dialog.dialog("open");
        ip_table.DataTable().columns.adjust();
        ip_table.DataTable().ajax.reload();
    });

    ip_table.DataTable({
        ordering: false,
        lengthMenu: [10, 20, 50, 100, 500],
        pageLength: 50,
        processing: true,
        ajax: common.modifyRelativeUrl("/FraudCase/GetIpList"),
        scrollY: '40vh',
        scrollCollapse: true,
        dom: "t<'pagination'<'page-size'lp>i>",
        language: common.language(),
        columnDefs: [
            { defaultContent: "-" },
            { targets: [1, 2], width: "100px" }
        ],
        columns: [
            { data: (row, type, set, meta) => meta.row + 1, className: "text-center", width: "40px" },
            { data: obj => obj.FromIp },
            { data: obj => obj.ToIp },
            { defaultContent: "<span class='icon-delete'></span>", className: "has-icon text-center", createdCell: delete_ip_range, width: "40px" }
        ]
    })

    $("#add-ip-btn").click(add_ip_range);

    $("#export-excel").click(export_excel);
};

$(scanned_office_ip);