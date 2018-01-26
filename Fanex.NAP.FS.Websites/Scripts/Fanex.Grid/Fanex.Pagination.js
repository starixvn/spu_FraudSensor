/*global fanexGridCurrentLanguage*/
(function ($) {
    $.fn.fanexPagination = function (maxentries, opts) {
        opts = $.extend({
            itemsPerPage: 20,
            startPerPage: 10,
            stepPerPage: 10,
            itemsPerPageArray: [],
            numDisplayEntries: 5,
            currentPage: 0,
            numEdgeEntries: 3,
            linkTo: "javascript:void(0)",
            prevText: "&lt;",
            nextText: "&gt;",
            ellipseText: "...",
            prevShowAlways: true,
            nextShowAlways: true,
            maxTtemsPerPage: 100,
            showDetail: true,

            callback: function () { return false; }
        }, opts || {});

        return this.each(function () {
            /**
            * Calculate the maximum number of pages
            */
            function numPages() {
                return Math.ceil(maxentries / opts.itemsPerPage);
            }

            /**
            * Calculate start and end point of pagination links depending on 
            * currentPage and numDisplayEntries.
            * @return {Array}
            */
            function getInterval() {
                var ne_half = Math.floor(opts.numDisplayEntries / 2);
                var np = numPages();
                var upper_limit = np - opts.numDisplayEntries;
                var start = currentPage > ne_half ? Math.max(Math.min(currentPage - ne_half, upper_limit), 0) : 0;
                var end = currentPage > ne_half ? Math.min(currentPage + (ne_half + 1), np) : Math.min(opts.numDisplayEntries, np);

                return [start, end];
            }

            /**
            * This is the event handling function for the pagination links. 
            * @param {int} page_id The new page number
            */
            function pageSelected(page_id, evt) {
                currentPage = page_id;
                //drawLinks();
                var continuePropagation = opts.callback(parseInt(page_id), parseInt(opts.itemsPerPage), panel);
                if (!continuePropagation) {
                    if (evt.stopPropagation) {
                        evt.stopPropagation();
                    }
                    else {
                        evt.cancelBubble = true;
                    }
                }
                return continuePropagation;
            }

            /**
            * This function inserts the pagination links into the container element
            */
            function drawLinks() {
                panel.empty();
                var interval = getInterval();
                var np = numPages();
                // This helper function returns a handler function that calls pageSelected with the right page_id
                var getClickHandler = function (page_id) {
                    return function (evt) { return pageSelected(page_id, evt); }
                }

                //Generate Items Per Page
                $('<div class="itemperpage">' + fanexGridCurrentLanguage.pageSize + ' </div>').appendTo(panel);
                var optPageSize = $('<select id="itemperpage"></select>').css("width", "50px");
                var totalItemPerPage = opts.itemsPerPageArray.length;

                if (totalItemPerPage > 0) {
                    for (var j = 0; j < totalItemPerPage; j++) {
                        var itemperpage = opts.itemsPerPageArray[j];

                        if (itemperpage == opts.itemsPerPage) {
                            $('<option value="' + itemperpage + '" selected="true">' + itemperpage + '</option>').appendTo(optPageSize);
                        }
                        else {
                            $('<option value="' + itemperpage + '">' + itemperpage + '</option>').appendTo(optPageSize);
                        }
                    }
                }
                else {
                    for (var i = opts.startPerPage; i <= opts.maxTtemsPerPage; i += opts.stepPerPage) {
                        if (i == opts.itemsPerPage) {
                            $('<option value="' + i + '" selected="true">' + i + '</option>').appendTo(optPageSize);
                        }
                        else {
                            $('<option value="' + i + '">' + i + '</option>').appendTo(optPageSize);
                        }
                    }
                }

                optPageSize.bind("change", function () {
                    itemPerPage(this);
                });
                optPageSize.appendTo(panel);
                //            $('<span class="btnseparator"></span>').appendTo(panel);
                // Helper function for generating a single link (or a span tag if it's the current page)
                var numberList = $('<ul ></ul>');

                var appendItem = function (page_id, lastcss, appendopts) {
                    page_id = page_id < 0 ? 0 : (page_id < np ? page_id : np - 1); // Normalize page id to sane value
                    currentPage = currentPage < np ? currentPage : np - 1;
                    appendopts = $.extend({ text: page_id + 1, classes: "" }, appendopts || {});
                    if (page_id == currentPage) {
                        var lnk = $("<span class='current'>" + (appendopts.text) + "</span>");
                    }
                    else {
                        var lnk = $("<a>" + (appendopts.text) + "</a>")
						.bind("click", getClickHandler(page_id))
						.attr('href', opts.linkTo.replace(/__id__/, page_id));
                    }
                    if (appendopts.classes) { lnk.addClass(appendopts.classes); }
                    $(numberList).append($('<li class="' + (appendopts.classes == "" ? 'sep ' : ' ') + lastcss + '"></li>').append(lnk));
                }
                // Generate "Previous"-Link
                if (opts.prevText && (currentPage > 0 || opts.prevShowAlways)) {
                    var tempCurrentPage = (typeof (currentPage) == 'string' ? parseInt(currentPage) : currentPage);
                    appendItem(tempCurrentPage - 1, '', { text: opts.prevText, classes: "prev" });
                }
                // Generate starting points
                if (interval[0] > 0 && opts.numEdgeEntries > 0) {
                    var end = Math.min(opts.numEdgeEntries, interval[0]);
                    for (var i = 0; i < end; i++) {
                        appendItem(i, ((i == (end - 1)) ? ' last' : ''));
                    }
                    if (opts.numEdgeEntries < interval[0] && opts.ellipseText) {
                        $('<li><span class="lnk go-to-page" title="' + fanexGridCurrentLanguage.goToPage + '">' + opts.ellipseText + '</span></li>').appendTo(numberList);
                    }
                }
                // Generate interval links
                for (var i = interval[0]; i < interval[1]; i++) {
                    appendItem(i, (i == (interval[1] - 1) ? ' last' : ''));
                }
                // Generate ending points
                if (interval[1] < np && opts.numEdgeEntries > 0) {
                    if (np - opts.numEdgeEntries > interval[1] && opts.ellipseText) {
                        $('<li><span class="lnk go-to-page" title="' + fanexGridCurrentLanguage.goToPage + '">' + opts.ellipseText + '</span></li>').appendTo(numberList);
                    }
                    var begin = Math.max(np - opts.numEdgeEntries, interval[1]);
                    for (var i = begin; i < np; i++) {
                        appendItem(i, (i == (np - 1) ? ' last' : ''));
                    }

                }
                // Generate "Next"-Link
                if (opts.nextText && (currentPage < np - 1 || opts.nextShowAlways)) {
                    var tempCurrentPage = (typeof (currentPage) == 'string' ? parseInt(currentPage) : currentPage);
                    appendItem(tempCurrentPage + 1, '', { text: opts.nextText, classes: "next" });
                }
                var list = $('<div class="numberlist"></div>');
                $(list).append($(numberList)).appendTo(panel);

                // Generate Status
                if (opts.showDetail) {
                    var pagingText = fanexGridCurrentLanguage.pagingText
                                                            .replace('{0}', (opts.itemsPerPage * opts.currentPage + 1))
                                                            .replace('{1}', Math.min(opts.itemsPerPage * (opts.currentPage + 1), maxentries))
                                                            .replace('{2}', maxentries);
                    $('<div class="detailtext">' + pagingText + '</div>').appendTo(panel);
                }

                //GotoPage Pane
                $(drawGoToPage()).appendTo(panel);
                panel.find("#pagenav_ibtn").bind("click", function () {
                    var txtPage = panel.find("#txtpage");
                    if (isNumberNotNull(txtPage)) {
                        var curr_page = Math.min(parseInt(parseFloat(txtPage.val())) - 1, np - 1);
                        if (curr_page < 0) {
                            txtPage.val(0);
                            curr_page = 0;
                        }

                        pageSelected(curr_page, this);
                    }
                });

                panel.find(".go-to-page").bind("click", function () {
                    var pagelist = panel.find('.numberlist');
                    var offset = pagelist.offset();
                    var goto = panel.find('.gotopage_box');
                    goto.toggle();
                    var w = pagelist.width();
                    var gotow = goto.width();
                    //                goto.css("left", (offset.left + (w - gotow) / 2 - 10) + "px");
                    //                goto.css("top", "-25px");
                    var gotoOffset = goto.offset();
                    goto.offset({
                        top: offset.top - 25,
                        left: (offset.left + (w - gotow) / 2)
                    });
                });

                panel.find("#txtpage").bind("keypress", function (e) {
                    if (!e) e = window.event;
                    var code = (e.keyCode) ? e.keyCode : e.which;

                    if (code == 69 || code == 101 || code == 45 || code == 37 || code == 39 || code == 36 || code == 35) //e or E or "-"
                        return true;
                    if ((code < 48 || code > 57) && code != 8 && code != 13 && code != 0 && code != 46 && code != 9) {
                        return false;
                    }

                    if (code == 13) {
                        try {
                            panel.find("#pagenav_ibtn").click();
                            return true;
                        } catch (error)
                    { return false; }
                    }
                });
            }

            //check Numeric input field by id
            function isNumberNotNull(txtPage) {
                var result = false;
                try {
                    if (!$.trim(txtPage.val()) == "") {
                        result = !isNaN(txtPage.val());
                    }
                } catch (error) {
                }

                return result;
            }

            function drawGoToPage() {
                var goto_page = '<table class="c gotopage_box" id="page_menu" cellpadding="0px" cellspacing="0px"><tr><td class="go-to-page-text">' + fanexGridCurrentLanguage.goToPage + '</td><td><input type="text" name="txtpage" id="txtpage" class="txt" size="4"></td><td><input type="button" id="pagenav_ibtn" class="pagenav_ibtn" ></input></td></tr></table>';
                return goto_page;
            }

            function itemPerPage(obj) {
                opts.itemsPerPage = $(obj).val();
                drawLinks();
                //alert(opts.currentPage);
                opts.callback(parseInt(opts.currentPage), parseInt(opts.itemsPerPage), this);
            }
            // Extract currentPage from options
            var currentPage = opts.currentPage;

            // Create a sane value for maxentries and itemsPerPage
            maxentries = (!maxentries || maxentries < 0) ? 1 : maxentries;
            opts.itemsPerPage = (!opts.itemsPerPage || opts.itemsPerPage < 0) ? 1 : opts.itemsPerPage;
            // Store DOM element for easy access from all inner functions
            var panel = $(this);
            // Attach control functions to the DOM element 
            this.selectPage = function (page_id) { pageSelected(page_id); }
            this.prevPage = function () {
                if (currentPage > 0) {
                    pageSelected(currentPage - 1);
                    return true;
                }
                else {
                    return false;
                }
            }
            this.nextPage = function () {
                if (currentPage < numPages() - 1) {
                    pageSelected(currentPage + 1);
                    return true;
                }
                else {
                    return false;
                }
            }
            // When all initialisation is done, draw the links
            drawLinks();
            // call callback function
            //opts.callback(currentPage, opts.itemsPerPage, this);
        });
    }
})(jQuery);
