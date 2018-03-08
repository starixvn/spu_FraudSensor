function UICommon() {
    $("#progress_bar").progressbar({ value: false });

    var reg = /[!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]/;

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

    this.downloadFile = function (url, parameters) {
        if (url && parameters) {
            var downloadUrl = url + '?';
            var iframe = $('body iframe');

            parameters = typeof parameters === 'string' ? parameters : $.param(parameters);
            downloadUrl += parameters;

            if (iframe.length === 0) {
                iframe = $('<iframe width="0" height="0" class="display-none"></iframe>');
                iframe.appendTo('body');
            }

            iframe.attr('src', downloadUrl);
        }
    };

    this.hasSpecialCharacter = function (str = "") {
        if (str == null) {
            return false;
        }

        return reg.test(str);
    }
}

var common = new UICommon();