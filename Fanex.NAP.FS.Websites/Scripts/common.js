function UICommon() {
    $("#progress_bar").progressbar({ value: false });

    var reg = /[<>]/;

    var loading = $("#loading").hide();

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

    $(document).ajaxError(() => this.showError());

    this.showLoading = function () {
        loading.show();
    };

    this.closeLoading = function () {
        loading.hide();
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
                'class': 'button -export',
                text: "Yes",
                click: function () {
                    confirmation.dialog("close");

                    if (yesCallback != null) {
                        yesCallback(this)
                    }
                }
            };

        var noBtn =
            {
                'class': 'button -primary',
                text: "No",
                click: function () {
                    confirmation.dialog("close");

                    if (noCallback != null) {
                        noCallback(this)
                    }
                }
            }

        confirmation.dialog("option", {
            dialogClass: "dialog-confirmation",
            buttons: [yesBtn, noBtn],
            width: 460
        })
    }

    this.showSuccess = function (str = "Success", timeout = 3000) {
        $.toast({
            text: `<div class='toast-content'>${str}</div>`, // Text that is to be shown in the toast
            heading: 'Successful!', // Optional heading to be shown on the toast
            icon: 'success', // Type of toast icon
            showHideTransition: 'fade', // fade, slide or plain
            allowToastClose: true, // Boolean value true or false
            hideAfter: timeout, // false to make it sticky or number representing the miliseconds as time after which toast needs to be hidden
            stack: 5, // false if there should be only one toast at a time or a number representing the maximum number of toasts to be shown at a time
            position: 'top-center', // bottom-left or bottom-right or bottom-center or top-left or top-right or top-center or mid-center or an object representing the left, right, top, bottom values
            textAlign: 'left',  // Text alignment i.e. left, right or center
            loader: true,  // Whether to show loader or not. True by default
            loaderBg: '#fff',  // Background color of the toast loader
        });
    }

    this.showError = function (str = "Internal Error", timeout = 3000) {
        $.toast({
            text: `<div class='toast-content'>${str}</div>`, // Text that is to be shown in the toast
            heading: 'Error!', // Optional heading to be shown on the toast
            icon: 'error', // Type of toast icon
            showHideTransition: 'fade', // fade, slide or plain
            allowToastClose: true, // Boolean value true or false
            hideAfter: timeout, // false to make it sticky or number representing the miliseconds as time after which toast needs to be hidden
            stack: 5, // false if there should be only one toast at a time or a number representing the maximum number of toasts to be shown at a time
            position: 'top-center', // bottom-left or bottom-right or bottom-center or top-left or top-right or top-center or mid-center or an object representing the left, right, top, bottom values
            textAlign: 'left',  // Text alignment i.e. left, right or center
            loader: true,  // Whether to show loader or not. True by default
            loaderBg: '#fff',  // Background color of the toast loader
        });
    }

    this.getTableDefaultOptions = () => Object.assign({}, defaultTableOptions);

    this.modifyRelativeUrl = url => window.location.host.includes("starific") ? "/V2" + url : url;

    this.makeNonSearchSelect = select => $(select).select2({ minimumResultsForSearch: -1 })

    this.makeDatePicker = function (input, options) {
        var $input = $();

        if (input instanceof jQuery) {
            $input = input;
        }
        else {
            $input = $(input);
        }

        var next_element = $input.next();

        if (next_element.is("span") && !next_element.hasClass("icon-calendar-text")) {
            next_element.addClass("icon-calendar-text");
        }
        else {
            next_element = $('<span class="icon-calendar-text"></span>');
            $input.after(next_element);
        }

        next_element.click(() => $input.focus());

        $input.addClass("control").datepicker(options);
    }

    this.makeWrapString = function (str = "", maxLength = 0) {
        if (str == null) return str;

        if (str.length > maxLength) return str.substring(0, maxLength - 3) + "...";

        return str;
    }

    this.formatNumber = (n = 0, format = "0,0.00") => numeral(n).format(format);

    this.encodeString = encodeURIComponent;

    this.registerAutoLoading = function () {
        $(document).ajaxStart(() => loading.show());
        $(document).ajaxComplete(() => loading.hide());
    }
}

var common = new UICommon();