function UICommon() {
    $("#progress_bar").progressbar({ value: false });

    var loading = $("#loading").dialog({
        autoOpen: false,
        modal: true,
        open: function (e, ui) {
            $(".ui-dialog-titlebar-close", ui.dialog | ui).hide();
        },
        draggable: false,
        resizable: false,
        height: 80
    });

    this.showLoading = function () {
        loading.dialog("open");
    };

    this.closeLoading = function () {
        loading.dialog("close");
    }
}

var common = new UICommon();