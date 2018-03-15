function UICommon() {
    $("#progress_bar").progressbar({ value: false });

    var reg = /[<>]/;

    var loading = $("#loading").dialog({
        autoOpen: false,
        modal: true,
        open: function (e, ui) {
            $(".ui-dialog-titlebar-close", ui.dialog | ui).hide();
        },
        draggable: false,
        resizable: false,
        height: 100
    });

    var confirmation = $("#confirmation").dialog({
        autoOpen: false,
        modal: true,
        draggable: false,
        resizable: false,
    });

    var success = $("#success").dialog({
        autoOpen: false,
        draggable: false,
        resizable: false,
        height: 100,
        position: { my: "top", at: "top", of: window }
    });

    var error = $("#error").dialog({
        autoOpen: false,
        draggable: false,
        resizable: false,
        height: 100,
        position: { my: "top", at: "top", of: window }
    });

    var defaultTableOptions = {
        ordering: false,
        searching: false,
        dom: "t<'pagination'<'page-size'lp>i>",
        language: {
            lengthMenu: "Page size _MENU_",
            paginate: {
                previous: "Back"
            },
            info: "Displaying _START_ to _END_ of _TOTAL_ item(s)",
            infoEmpty: ""
        }
    }

    $(document).ajaxError(() => showDialog(error, "Internal Error", 3000));

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

    this.showConfirmation = function (html, yesCallback, noCallback) {
        confirmation.empty().html(html).dialog("open");

        var yesBtn =
            {
                text: "Yes",
                click: function () {
                    confirmation.dialog("close");

                    if (yesCallback != null) {
                        yesCallback()
                    }
                }
            };

        var noBtn =
            {
                text: "No",
                click: function () {
                    confirmation.dialog("close");

                    if (noCallback != null) {
                        noCallback()
                    }
                }
            }

        confirmation.dialog("option", "buttons", [yesBtn, noBtn])
    }

    this.showSuccess = (str = "Success", timeout = 3000) => showDialog(success, str, timeout);

    this.showError = (str = "Internal Error", timeout = 3000) => showDialog(error, str, timeout);

    this.getTableDefaultOptions = () => Object.assign({}, defaultTableOptions);

    this.modifyRelativeUrl = url => window.location.host.includes("starific") ? "V2" + url : url;

    function showDialog(target, str, timeout) {
        target.text(str).dialog("open");

        setTimeout(() => target.dialog("close"), timeout);
    }
}

var common = new UICommon();