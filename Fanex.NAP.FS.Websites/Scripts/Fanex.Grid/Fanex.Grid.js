// --------------------------------------------------------------------------
// <copyright file="Fanex.Grid.js" company="">
// Copyright (c) . All rights reserved.
// Revision:
//          + 01/10/2014: Auto re-size column.
//          + 01/21/2014: Supported Sort with SortType for each column.
//          + 01/22/2014: Fixed Auto reset scroller when resize grid.
//          + 02/06/2014: Customize Render footer when auto paging + Update initialize footer.
//          + 02/06/2014: Customize Render footer when auto paging + Update initialize footer.
//          + 02/27/2014: Update show "..." when auto truncate.
//          + 03/19/2014: Support Time Zone Config
//          + 04/08/2014: Support auto resize to full-screen width
//          + 04/11/2014: Remove class "Container"
//          + 04/16/2014: Auto fix header height, fix collapse/expand
//          + 06/20/2014: Fix auto fix row height.
//          + 09/25/2014: Update auto parseInt for paging option.
//          + 10/08/2014: Fix dont resize grid hidden.
//          + 10/09/2014: Update default page index for pager
//          + 10/14/2014: Remove use strict
//          + 11/02/2014: Merge Grid Scroller in to Fanex.Grid
//          + 11/20/2014: Update virtual Rendering
//          + 01/23/2015: Check pager undefined before add in show/hide columns
//          + 03/03/2015: Update fix height for both table when auto fix height
//          + 03/03/2015: Update fix show format number for number column, add defaultCollapseAllGroups option for collapsing all group by default
//          + 07/28/2016: Support collapse/expand on footer
//          + 08/04/2016: Use bottomBorderWidth to calculate top for footer
//          + 08/01/2018: Support Smartphone Friendly
// </copyright>
// --------------------------------------------------------------------------
/*jshint maxdepth:false, maxstatements:false, strict:false*/
/*global fanexGridLanguage, moment,numeral*/

var fanexGridCurrentLanguage = fanexGridCurrentLanguage || {};

(function ($) {
    //'use strict';

    //// Method expose for user use
    var methods =
    {
        createFanexGrid: function (opts) {
            if (opts.showColumnsShowHide === false || opts.enableHiddenColumns === false) {
                opts.enableHiddenColumns = false;
                opts.hiddenColumns = opts.hiddenColumns || [];
                opts.hiddenColumns = opts.hiddenColumns.concat(opts.columnsNeedHide || []);
                opts.columns = Fanex.Grid.Helper.SetColumnDisplay(opts.columns, opts.hiddenColumns);
            }
            opts = $.extend({}, $.fn.fanexGrid.defaults, opts);
            return this.each(function () {
                var fanexGrid = new Fanex.Grid(this, opts);
                fanexGrid.RenderGrid();
                return fanexGrid.GetjQueryInstance();
            });
        },
        reRender: function (opts) {
            if (opts.showColumnsShowHide === false || opts.enableHiddenColumns === false) {
                opts.enableHiddenColumns = false;
                opts.hiddenColumns = opts.hiddenColumns || [];
                opts.hiddenColumns = opts.hiddenColumns.concat(opts.columnsNeedHide);
                opts.columns = Fanex.Grid.Helper.SetColumnDisplay(opts.columns, opts.hiddenColumns);
            }
            opts = $.extend({}, $.fn.fanexGrid.defaults, opts);
            return this.each(function () {
                var fanexGrid = new Fanex.Grid(this, opts);
                fanexGrid.RenderGrid();
                return fanexGrid.GetjQueryInstance();
            });
        },
        reRenderBodyAfterSort: function (opts, sortColumnName, sortDirection) {
            opts = $.extend({}, $.fn.fanexGrid.defaults, opts);
            return this.each(function () {
                var fanexGrid = new Fanex.Grid(this, opts);
                fanexGrid.ReRenderBodyAfterSort(sortColumnName, sortDirection);
                if (opts.showPager === true) {
                    fanexGrid.RenderPager();
                }
                return fanexGrid.GetjQueryInstance();
            });
        },
        showColumns: function (opts, columnsNeedShow) {
            opts.enableHiddenColumns = true;
            opts.columns = Fanex.Grid.Helper.SetColumnDisplay(opts.columns, columnsNeedShow);
            opts = $.extend({}, $.fn.fanexGrid.defaults, opts);
            return this.each(function () {
                var fanexGrid = new Fanex.Grid(this, opts);
                if (Fanex.Grid.List[fanexGrid.tableContentId].pagerOption) {
                    opts.pagerOption = Fanex.Grid.List[fanexGrid.tableContentId].pagerOption;
                }
                Fanex.Grid.Helper.UpdateSortInforAfterShowHideColumns(opts, Fanex.Grid.List[fanexGrid.tableContentId].sortOption);
                fanexGrid.RenderGrid();
                return fanexGrid.GetjQueryInstance();
            });
        },
        hideColumns: function (opts, hiddenColumns) {
            opts.enableHiddenColumns = false;
            opts.columns = Fanex.Grid.Helper.SetColumnDisplay(opts.columns, hiddenColumns);
            opts.hiddenColumns = hiddenColumns;
            opts = $.extend({}, $.fn.fanexGrid.defaults, opts);
            return this.each(function () {
                var fanexGrid = new Fanex.Grid(this, opts);
                if (Fanex.Grid.List[fanexGrid.tableContentId].pagerOption) {
                    opts.pagerOption = Fanex.Grid.List[fanexGrid.tableContentId].pagerOption;
                }
                Fanex.Grid.Helper.UpdateSortInforAfterShowHideColumns(opts, Fanex.Grid.List[fanexGrid.tableContentId].sortOption);
                fanexGrid.RenderShowHideColumns(false);
                return fanexGrid.GetjQueryInstance();
            });
        },
        resizeGrid: function () {
            return this.each(function () {
                var fanexGrid = new Fanex.Grid(this, { reCalculateGrid: true });
                var gridOptions = Fanex.Grid.List[fanexGrid.tableContentId].gridOptions;
                gridOptions.isResetScroller = false;

                if (gridOptions.autoCalculateGridExpandHeight
                    && gridOptions.fullHeight) {
                    gridOptions.gridHeight = fanexGrid.CalculateGridExpandHeight(gridOptions);
                }

                fanexGrid.RenderGridScroller(gridOptions);
                return fanexGrid.GetjQueryInstance();
            });
        },
        resizeHeight: function (gridHeight) {
            return this.each(function () {
                var fanexGrid = new Fanex.Grid(this, { reCalculateGrid: true });

                if (gridHeight) {
                    var gridOptions = Fanex.Grid.List[fanexGrid.tableContentId].gridOptions;
                    gridOptions.gridHeight = gridHeight;
                    fanexGrid.RenderGridScroller(gridOptions);
                }
                return fanexGrid.GetjQueryInstance();
            });
        },
        appendBody: function (opts, rows) {
            opts = $.extend({}, $.fn.fanexGrid.defaults, opts);
            return this.each(function () {
                var fanexGrid = new Fanex.Grid(this, opts);
                fanexGrid.AppendBodyRows(rows);
                $(this).fanexGrid('resizeGrid');
            });
        },
        getCurrentPage: function () {
        },
        getColumnsIdentify: function (opts) {
            return Fanex.Grid.Helper.GetColumnsHastable(opts.columns);
        }
    };

    //// Plugin
    $.fn.fanexGrid = function (options) {
        if (methods[options]) {
            return methods[options].apply(this, Array.prototype.slice.call(arguments, 1));
        }
        else if (typeof options === 'object' || !options) {
            return methods.createFanexGrid.apply(this, arguments);
        }
        else {
            throw 'Options cannot be null';
        }
    };

    //// Default Options
    $.fn.fanexGrid.defaults =
    {
        customizeHeader: false,
        headerRows: [],
        columns: [],
        bodyRows: [],
        numberOfFrozenColumn: 0,
        showFooter: false,
        footerRows: [],
        showTitle: false,
        headerColumnHeight: 25,
        bodyRowHeight: 25,
        footerRowHeight: 25,
        titleInfo:
        {
            text: "",
            hideWhenNoData: true,
            showExcelButton: false,
            exportExcelFunction: null,
            showBackButton: false,
            backFunction: null,
            showSendMailButton: false,
            sendMailFunction: null,
            showCollapseExpandButton: false,
            customButtons: '',
            showButtonsWhenNoData: false
        },
        showPager: false,
        pagerOption:
        {
            itemsPerPage: 100,
            maxItemsPerPage: 500,
            currentPage: 0,
            totalItem: 0,
            startPerPage: 100,
            pageStep: 50,
            afterPagedFunction: null,
            showDetail: true
        },
        showTree: false,
        gridExpandHeight: 150,
        showNoDataOnFrozen: false,
        showColumnsShowHide: true,
        enableHiddenColumns: true,
        showDistinctRow: true,
        showHighlightRow: true,
        highlightFrozenWidth: 1,
        showNumberColumn: false,
        marginRight: 5,
        allowTruncate: false,
        columnsNeedHide: [],
        hiddenColumns: [],
        sortOption:
        {
            customGridSort: null,
            serverSortFunction: null,
            afterGridSorted: null,
            beforeGridSorted: null
        },
        hideYBackGroundScroller: false,
        bottomBorderWidth: 1,
        treeParentPaddingLeft: 20,
        isResetScroller: false,
        fullHeight: true,
        gridHeight: 400,
        gridWidth: 0,
        noDataText: "There is no data",
        language: 'en-US',
        autoFixFrozenRowHeight: false,
        autoResizeColumn: false,
        pixelsPerCharacter: 0,
        calTextWidthWithFontSize: true,
        timeZone: '',
        scrollWidth: 24,
        reCalculateGrid: false,
        defaultCollapseAllGroups: false,
        virtualScroller: false,
        numOfVirtualRows: 100,
        thresholdNeedToVirtualRendering: 150,
        virtualThresholdNumber: 30,
        renderVirtualTimeout: 50,
        keepStateOnVirtualRow: false,
        minHeight: 120,
        minWidth: 300,
        fullWidth: true,
        autoCalculateGridExpandHeight: false,
        numOfPaddingForGridExpandHeight: 10
    };

    var Fanex = Fanex || {};

    Fanex.Grid = function (grid, options) {
        this.$grid = $(grid);
        this.gridObject = grid;
        this.gridId = this.$grid.attr('id');
        this.tableContentId = 'tblContent_' + this.gridId;
        this.frozenContentId = 'frozenContent_' + this.gridId;
        //// Calculate fontsize for hide cell value if cell value exceed
        if (options.reCalculateGrid) {
            return this;
        }

        try {
            this.currentFontSize = parseInt(this.$grid.css('font-size').replace('px', ''));
        }
        catch (e) {
            this.currentFontSize = 12;
        }
        this.currentFontSize -= 4;
        if (options.pixelsPerCharacter === 0) {
            options.pixelsPerCharacter = this.currentFontSize;
        }
        this.Options = options.autoResizeColumn ? Fanex.Grid.Helper.ComputeResizeColumn(this.$grid, options) : options;
        this.headerColumnsCloneFromOptionColumns = Fanex.Grid.Helper.CreateHeaderColumnsFromOptionsColumn(options.columns.slice(0));

        if (fanexGridLanguage[options.language] == null) {
            alert('Cannot load language: ' + options.language);
        }
        else {
            fanexGridCurrentLanguage = fanexGridLanguage[options.language];
        }

        //// Create a hashtable contain columns information for esay retrieve data
        this.columnsHastable = Fanex.Grid.Helper.GetColumnsHastable(options.columns);
        //// Create columnswidth array
        this.columnsWidth = Fanex.Grid.Helper.GetArrayWidth(options.columns, options.enableHiddenColumns, false);
        this.columnsForComputingWidth = Fanex.Grid.Helper.GetArrayWidth(options.columns, options.enableHiddenColumns, true);
        if (this.Options.customPager !== true) {
            this.Options.pagerOption.totalItem = this.Options.bodyRows.length;
        }
        this.gridOptions = null;
        this.frozenColsName = [];
        //// If frozen, then create an array contain frozen columns name
        if (options.numberOfFrozenColumn > 0) {
            //var frozenColumnNumber = isValid(options.numberOfFrozenColumnSaved) ? options.numberOfFrozenColumnSaved : options.numberOfFrozenColumn;
            for (var colIndex = 0; colIndex < options.numberOfFrozenColumn; colIndex++) {
                this.frozenColsName.push(options.columns[colIndex].Name);
            }
            //// If highlight frozen then set width for hightligth
            if (options.highlightFrozenWidth > 1) {
                var lastFrozenCol = options.columns[options.numberOfFrozenColumn - 1];
                if (isValid(lastFrozenCol.TrClass)) {
                    lastFrozenCol.TdClass += " border-right-highlight ";
                }
                else {
                    lastFrozenCol.TdClass = "border-right-highlight ";
                }
                options.highlightFrozenWidth--;
            }
        }
    };

    Fanex.Grid.prototype =
    {
        RenderGrid: function () {
            if (typeof (fanexGridLanguage) === 'undefined') {
                return;
            }
            var me = this;
            var options = me.Options;
            var footer = '';
            if (options.showFooter && options.bodyRows.length > 0) {
                if (typeof (options.pagerOption.customRenderFooter) === 'function') {
                    var startIndex = (options.pagerOption.currentPage * options.pagerOption.itemsPerPage);
                    var endIndex = ((options.pagerOption.currentPage + 1) * options.pagerOption.itemsPerPage);
                    var currentBodyRows = options.bodyRows.slice(startIndex, endIndex);
                    me.Options.footerRows = options.pagerOption.customRenderFooter(
                                                                                currentBodyRows,
                                                                                options.pagerOption.currentPage,
                                                                                options.pagerOption.itemsPerPage);
                }

                footer = me.RenderFooter();
            }
            //// Render Table Main Frame
            var html = me.RenderBody(me.RenderHeader()) + footer;
            if (me.gridObject) {
                me.gridObject.innerHTML = html;
            }
            //me.$grid.empty().html(html);
            me.$grid.before(me.RenderTitle());
            //me.$grid.parent().css("float","left");
            me.RegisterMouseHoverEvent();
            me.Initialize();
            //// Render Title
            //
        },
        RenderShowHideColumns: function (isShow) {
            var me = this;
            var options = me.Options;
            var currentMainBodyHtml = me.$grid.find('#' + me.tableContentId + '> tbody').html();
            var currentFrozenBodyHeader = me.$grid.find('#' + me.frozenContentId + '> tbody').html();
            var bodyHtml = me.RenderHeader() + me.RenderCommonHeader('mainbody');
            var footer = (options.showFooter && options.bodyRows.length > 0 ? me.RenderFooter() : "");
            var bodyHtmlTemp = '',
                currentCol;
            // Fix for replace $' string
            if (options.numberOfFrozenColumn > 0) {
                bodyHtmlTemp = bodyHtml.split('{$}frozenBody');
                if (bodyHtmlTemp.length === 2) {
                    bodyHtml = bodyHtmlTemp[0] + currentFrozenBodyHeader + bodyHtmlTemp[1];
                }
            }
            bodyHtmlTemp = bodyHtml.split('{$}_');
            if (bodyHtmlTemp.length === 2) {
                bodyHtml = bodyHtmlTemp[0] + currentMainBodyHtml + bodyHtmlTemp[1];
            }

            bodyHtml += footer;

            me.$grid.empty(); //.html(bodyHtml);
            if (me.gridObject) {
                me.gridObject.innerHTML = bodyHtml;
            }

            var selector = [];
            var columnsShowLength = options.columns.length;
            for (var colIndex = 0; colIndex < columnsShowLength; colIndex++) {
                //selector.push('.free-grid-td-' + colIndex);
                me.$grid.find('.free-grid-td-' + colIndex).removeClass("grid-show-hide-column");
            }
            //me.$grid.find(selector.join(',')).show();

            if (!isShow) {
                var columnsHideLength = options.hiddenColumns.length,
                    nhIndex;
                selector = [];
                try {
                    for (var colIndex1 = 0; colIndex1 < columnsHideLength; colIndex1++) {
                        currentCol = options.hiddenColumns[colIndex1];
                        //selector.push('.free-grid-td-' + currentCol);
                        //me.$grid.find('.free-grid-td-' + currentCol).addClass("grid-show-hide-column");
                        me.$grid.find('.free-cell, .row_header_wrapper, .col_header, .grid-container')
                        .find('.free-grid-td-' + currentCol)
                        .addClass("grid-show-hide-column");
                    }

                    if (options.showFooter) {
                        var needHideCols = options.hiddenColumns.slice(0);
                        for (nhIndex = 0; nhIndex < needHideCols.length; nhIndex++) {
                            currentCol = needHideCols[nhIndex];
                            me.$grid.find('.row_footer, .col_footer').find('.free-grid-td-' + currentCol).addClass("grid-show-hide-column");
                        }
                    }
                }
                catch (e) {
                    window.console.log(e);
                }
                //me.$grid.find(selector.join(',')).hide();
            }

            me.RegisterMouseHoverEvent();
            me.Initialize();
        },
        ReRenderBodyAfterSort: function (sortColumnName, sortDirection) {
            var me = this;
            me.RenderBody('', true);
            me.gridOptions = Fanex.Grid.List[me.tableContentId].gridOptions;
            me.gridOptions.isResetScroller = false;
            me.RegisterMouseHoverEvent();
            me.AutoResizeRowHeight();
            me.RenderGridScroller(me.gridOptions);
            //// Save Current Sor
            Fanex.Grid.List[me.tableContentId].sortOption =
            {
                SortColumnName: sortColumnName,
                SortDirection: sortDirection
            };
        },
        AppendBodyRows: function (rows) {
            var me = this;
            var options = me.Options;
            var rowLength = rows.length,
                currentRow;
            if (rowLength > 0) {
                var currentFrozenRows = (options.numberOfFrozenColumn > 0) ?
                                        Fanex.Grid.Helper.CreateRowsForFrozenColumn(rows, options.numberOfFrozenColumn, me.frozenColsName) :
                                        [];
                var frozenBodyLength = currentFrozenRows.length;
                var bodyRowsHtml = [], bodyFrozenHtml = [];
                var currentPageStartIndex = options.bodyRows.length;

                //// Render Frozen Row and Main Row at the same time
                for (var rowIndex = 0; rowIndex < frozenBodyLength; rowIndex++) {
                    currentRow = rows[rowIndex];
                    var currentFrozenRow = currentFrozenRows[rowIndex];
                    bodyRowsHtml.push(me.RenderNormalRow(currentRow, 'td', true, rowIndex + currentPageStartIndex, '', false, rowIndex, 0));
                    bodyFrozenHtml.push(me.RenderFrozenRow(currentFrozenRow, 'td', true, rowIndex + currentPageStartIndex, '', false, rowIndex, 0));
                }

                //// If MainRow still has item, then continue render
                for (rowIndex = frozenBodyLength; rowIndex < rowLength; rowIndex++) {
                    currentRow = rows[rowIndex];
                    bodyRowsHtml.push(me.RenderNormalRow(currentRow, 'td', true, rowIndex + currentPageStartIndex, '', false, rowIndex, 0));
                }

                //// Update Data
                me.$grid.find('#' + me.tableContentId + '> tbody').append(bodyRowsHtml.join(''));
                me.$grid.find('#' + me.frozenContentId + '> tbody').append(bodyFrozenHtml.join(''));
            }
        },
        GetjQueryInstance: function () {
            return this.$grid;
        },
        Initialize: function () {
            var me = this;
            var options = this.Options;
            var numberOfFrozenColumnNew = (options.enableHiddenColumns === false ?
                                            Fanex.Grid.Helper.ComputeNewFrozenColumn(options.hiddenColumns, options.numberOfFrozenColumn) :
                                            options.numberOfFrozenColumn);

            //// Render Grid Scroller
            me.gridOptions =
            {
                tblContentId: me.tableContentId,
                column: me.columnsWidth,
                gridHeight: options.fullHeight ? me.CalculateGridExpandHeight(options) : options.gridHeight,
                gridWidth: options.gridWidth,
                minHeight: options.minHeight,
                minWidth: options.minWidth,
                numberFreezeCol: numberOfFrozenColumnNew,
                highlightFrozenWidth: options.highlightFrozenWidth,
                heightScrollYAdd: 0,
                marginRight: options.marginRight,
                hideYBackGroundScroller: options.hideYBackGroundScroller,
                bottomBorderWidth: options.bottomBorderWidth,
                isResetScroller: options.isResetScroller,
                fullHeight: options.fullHeight,
                fullWidth: options.fullWidth,
                autoCalculateGridExpandHeight: options.autoCalculateGridExpandHeight,
                numOfPaddingForGridExpandHeight: options.numOfPaddingForGridExpandHeight
            };
            me.InitGridScroller(options.showPager && (options.bodyRows.length > 0));
            me.RenderGridScroller(me.gridOptions);

            //// Render Pager
            if (options.showPager === true) {
                me.RenderPager();
                Fanex.Grid.List[me.tableContentId].pagerOption = options.pagerOption;
            }
            me.RegisterEvents();
            me.AutoResizeRowHeight();
        },
        CalculateGridExpandHeight: function (options) {
            if (!options.autoCalculateGridExpandHeight) {
                return options.gridExpandHeight;
            }

            var expandHeight = this.$grid.offset().top + options.numOfPaddingForGridExpandHeight;

            return expandHeight;
        },
        RenderTitle: function () {
            var me = this;
            var options = me.Options;
            var titleId = me.gridId + '-grid-title';
            if (options.showTitle === false) {
                return '';
            }
            else {
                me.$grid.parent().find('#' + titleId).remove();
                // style="width:' + me.$grid.width() + 'px"
                var titleHtml = [];
                titleHtml.push('<div id="' + me.gridId + '-grid-title' + '" class="grid-title display-none"><div class="grid-title-label" id="reporttile">');
                titleHtml.push(options.titleInfo.text);
                titleHtml.push('</div>');
                titleHtml.push('<div class="grid-toolbox">');
                if (options.titleInfo.showBackButton === true) {
                    titleHtml.push('<input id="btnBack' + me.gridId + '" class="btnBack" type="button" title="Back" value="" name="btnBack" />');
                }
                if (options.bodyRows.length > 0 || options.titleInfo.showButtonsWhenNoData === true) {
                    if (options.titleInfo.customButtons !== '') {
                        titleHtml.push(options.titleInfo.customButtons);
                    }
                    if (options.titleInfo.showCollapseExpandButton === true) {
                        titleHtml.push('<input class="btExpand expanded" id="btnCollapseExpand' +
                                        me.gridId +
                                        '" type="button" value="" title="Collapse/Expand" />');
                    }
                    if (options.titleInfo.showExcelButton === true) {
                        titleHtml.push('<input type="button" id="btnExport' + me.gridId + '" class="btnExcel" title="Export to Excel" />');
                    }
                    if (options.titleInfo.showSendMailButton === true) {
                        titleHtml.push('<input id="btnSendMail' + me.gridId + '" class="btnSendMail" type="button" title="Send Mail" />');
                    }
                }
                titleHtml.push('</div></div>');

                return titleHtml.join('');
            }
        },
        RenderHeader: function () {
            var me = this;
            var options = this.Options;
            var headerHtml = [];

            //// Clone and Add last row of Group Header
            if (options.customizeHeader) {
                this.Options.cloneGroupHeaderRows = options.groupHeaderRows.slice(0);
                this.Options.cloneGroupHeaderRows.push({
                    Columns: me.headerColumnsCloneFromOptionColumns.slice(0)
                });
            }

            //// If there are any item of header column
            var columnLength = options.columns.length;
            if (columnLength > 0) {
                if (options.numberOfFrozenColumn > 0) {
                    headerHtml.push(me.RenderCommonHeader('frozenheader').replace('{$}_', ''));
                    headerHtml.push(me.RenderCommonHeader('frozenbody'));
                }
                headerHtml.push(me.RenderCommonHeader('mainheader').replace('{$}_', ''));
            }

            return headerHtml.join('');
        },
        RenderCommonHeader: function (type) {
            var me = this;
            var options = this.Options;
            var divHeaderClass = '';
            var divContainerClass = '';
            var tableClass = '';
            var hasId = false;
            var tableContainId = me.tableContentId;
            var columnLength = me.headerColumnsCloneFromOptionColumns.length;
            var replaceMark = '_';

            // Grid Frame
            //////////////////////////////////
            // frozen Header //  main header//
            //////////////////////////////////
            // frozen body   // main body   //
            //////////////////////////////////
            switch (type) {
                case 'frozenheader':
                    divContainerClass = 'free-cell free-top-left';
                    tableClass = 'grid-fixed-table free-cell-top-left';
                    columnLength = options.numberOfFrozenColumn;
                    break;
                case 'frozenbody':
                    divHeaderClass = 'row_header_wrapper';
                    divContainerClass = 'row_header row-headers-background';
                    tableClass = 'grid-fixed-table row_header_content';
                    columnLength = options.numberOfFrozenColumn;
                    hasId = true;
                    tableContainId = me.frozenContentId;
                    replaceMark = 'frozenBody';
                    break;
                case 'mainheader':
                    divHeaderClass = 'col_header row-headers-background scroll-y';
                    divContainerClass = 'header-container';
                    tableClass = 'grid-fixed-table col_header_content';
                    break;
                case 'mainbody':
                    divHeaderClass = 'grid-container';
                    divContainerClass = 'content-container';
                    tableClass = 'grid-fixed-table col_body_content';
                    hasId = true;
                    break;
            }

            var headerHtml = [],
                trsHeader = [],
                cellInfo,
                colIndex,
                currentCol,
                customSpan;
            //// Build Header
            if (divHeaderClass !== '') {
                headerHtml.push('<div class="' + divHeaderClass + '">');
            }
            headerHtml.push('<div class="' + divContainerClass + '">');
            headerHtml.push('<table class="' + tableClass + '" ' + (hasId ? ('id="' + tableContainId + '"') : '') + ' ><thead>');

            if (options.customizeHeader) {
                var currentGroupHeader = options.cloneGroupHeaderRows;
                if (type === 'frozenheader' || type === 'frozenbody') {
                    //// Create Header Rows for Frozen Table
                    currentGroupHeader = Fanex.Grid.Helper.CreateRowsForFrozenColumn(
                                                    options.cloneGroupHeaderRows.slice(0),
                                                    options.numberOfFrozenColumn,
                                                    me.frozenColsName);
                }
                for (var rowIndex = 0; rowIndex < currentGroupHeader.length; rowIndex++) {
                    var currentRow = currentGroupHeader[rowIndex];

                    if (currentRow.Columns.length > 0) {
                        headerHtml.push('<tr>');

                        for (colIndex = 0; colIndex < currentRow.Columns.length && colIndex < columnLength; colIndex++) {
                            currentCol = currentRow.Columns[colIndex];
                            if (isValid(currentCol)) {
                                customSpan = '';
                                var currentHeight = options.headerColumnHeight;
                                var showHeaderMergeCol = true;

                                if (isValid(currentCol) && isValid(currentCol.MergeRowNumber)) {
                                    //// If It has merge row and current row has only one columns and column has merge row, then remove next row
                                    Fanex.Grid.Helper.RemoveItemForRowSpan(currentGroupHeader, rowIndex, currentCol, me.columnsHastable);
                                    var currentMergeRowNumber = ' rowspan="' + currentCol.MergeRowNumber + '"';
                                    //if (type === 'frozenheader' || type === 'frozenbody') {
                                    //// Calculate height for merger row
                                    currentHeight = ((options.headerColumnHeight * currentCol.MergeRowNumber) + (currentCol.MergeRowNumber - 1) * 5);
                                    //}
                                    customSpan += currentMergeRowNumber;
                                }

                                //// If Current cell has merge Col
                                if (isValid(currentCol) && isValid(currentCol.MergeColNumber)) {
                                    //// Compute for With for merge
                                    currentCol.Width = Fanex.Grid.Helper.GetColMergeWidth(me.columnsForComputingWidth,
                                                                                            me.columnsHastable[currentCol.Name].ColumnIndex,
                                                                                            currentCol.MergeColNumber,
                                                                                            options.enableHiddenColumns,
                                                                                            options.columns);
                                    if (options.enableHiddenColumns === false) {
                                        showHeaderMergeCol = Fanex.Grid.Helper.HideHeaderGroupWhenShowHide(options.hiddenColumns,
                                                                                                            me.columnsHastable[currentCol.Name].ColumnIndex,
                                                                                                            currentCol.MergeColNumber);
                                        customSpan += ' colspan="' + Fanex.Grid.Helper.ReComputeMergeColumAfterShowHide(options.hiddenColumns,
                                                                                                            me.columnsHastable[currentCol.Name].ColumnIndex,
                                                                                                            currentCol.MergeColNumber) + '"';
                                    }
                                    else {
                                        customSpan += ' colspan="' + currentCol.MergeColNumber + '"';
                                    }
                                }

                                if (isValid(currentCol) && isValid(currentCol.MergeHeaderColNumber)) {
                                    customSpan += ' colspan="' + currentCol.MergeHeaderColNumber + '"';
                                }

                                cellInfo =
                                {
                                    CellTag: 'th',
                                    CustomSpan: customSpan,
                                    CssClass: currentCol.CssClass,
                                    SortClass: (currentRow.Columns[colIndex].Sort === true ? ' colSorter' : ''),
                                    CellWidth: (currentCol.Width || me.columnsHastable[currentCol.Name].Width),
                                    CellHeight: currentHeight,
                                    CellId: (me.columnsHastable[currentCol.Name].Sort === true ? (me.gridId + '_header_sort_' + currentCol.Name) : ''),
                                    Sort: currentRow.Columns[colIndex].Sort,
                                    CellColumnName: currentCol.Name,
                                    HeaderText: currentCol.HeaderText,
                                    CustomAttr: (currentGroupHeader.length > 1 && rowIndex === (currentGroupHeader.length - 1)) ?
                                                    me.columnsHastable[currentCol.Name].CustomAttr :
                                                    undefined,
                                    CustomHeaderRender: currentCol.CustomHeaderRender || me.columnsHastable[currentCol.Name].CustomHeaderRender,
                                    SortDirection: currentRow.Columns[colIndex].SortDirection || '',
                                    CanShowHide: (me.columnsHastable[currentCol.Name].CanShowHide && (!options.enableHiddenColumns) && showHeaderMergeCol),
                                    TdClass: me.columnsHastable[currentCol.Name].TdClass || '',
                                    ThClass: currentRow.Columns[colIndex].ThClass || '',
                                    CellAlign: me.columnsHastable[currentCol.Name].HeaderAlign || (me.columnsHastable[currentCol.Name].CellAlign || 'center')
                                };
                                trsHeader.push(me.RenderHeaderCell(cellInfo, currentCol));
                            }
                        }
                        headerHtml.push(trsHeader.join(''));
                        headerHtml.push('</tr>');
                        trsHeader = [];
                    }
                }
            }
            else {
                //// Has no frozen column
                headerHtml.push('<tr>');
                for (colIndex = 0; colIndex < columnLength; colIndex++) {
                    currentCol = me.headerColumnsCloneFromOptionColumns[colIndex];
                    customSpan = '';
                    if (isValid(currentCol) && isValid(currentCol.MergeHeaderColNumber)) {
                        customSpan += ' colspan="' + currentCol.MergeHeaderColNumber + '"';
                    }
                    cellInfo =
                    {
                        CellTag: 'th',
                        CustomSpan: customSpan,
                        CssClass: currentCol.CssClass,
                        SortClass: (currentCol.Sort === true ? ' colSorter' : ''),
                        CellWidth: (currentCol.Width || me.columnsHastable[currentCol.Name].Width),
                        CellHeight: options.headerColumnHeight,
                        CellId: (me.columnsHastable[currentCol.Name].Sort === true ? (me.gridId + '_header_sort_' + currentCol.Name) : ''),
                        Sort: currentCol.Sort,
                        CellColumnName: currentCol.Name,
                        HeaderText: currentCol.HeaderText,
                        CustomHeaderRender: currentCol.CustomHeaderRender || me.columnsHastable[currentCol.Name].CustomHeaderRender,
                        SortDirection: currentCol.SortDirection || '',
                        CustomAttr: me.columnsHastable[currentCol.Name].CustomAttr,
                        CanShowHide: me.columnsHastable[currentCol.Name].CanShowHide && (!options.enableHiddenColumns),
                        TdClass: me.columnsHastable[currentCol.Name].TdClass || '',
                        ThClass: me.columnsHastable[currentCol.Name].ThClass || '',
                        CellAlign: me.columnsHastable[currentCol.Name].HeaderAlign || (me.columnsHastable[currentCol.Name].CellAlign || 'center')
                    };
                    trsHeader.push(me.RenderHeaderCell(cellInfo, currentCol));
                }
                headerHtml.push(trsHeader.join(''));
                headerHtml.push('</tr>');
            }
            headerHtml.push('</thead><tbody>{$}' + replaceMark + '</tbody></table>');
            headerHtml.push('</div>');
            if (divHeaderClass !== '') {
                headerHtml.push('</div>');
            }
            return headerHtml.join('');
        },
        RenderFrozenRow: function (currentRow, colHtmlTag, isBody, rowIndex, treeCollapseExpandClass, isForNoData, parentHighlight, level) {
            var me = this;
            var options = me.Options;
            var frozenRowHtml = [];
            var colsLength = currentRow.Columns.length;
            var currentCollapseExpandId = '';
            var parentRowClass = '';
            var currentMergeColsIndex = 0;
            var trHighlightAtt = (options.showHighlightRow ? (' highlightatt="highlight-att-frozen-' + parentHighlight + '-' + rowIndex + (isBody ? "-body" : "") + '"') : '');
            var trAutoFixRowAtt = (options.autoFixFrozenRowHeight ? (' autofixatt="autofix-att-frozen-' + parentHighlight + '-' + rowIndex + (isBody ? "-body" : "") + '"') : '');
            var trRowIndexAttr = ' data-row-index="' + rowIndex + '" ';
            var trClass = (options.showHighlightRow ? 'highlight-row ' : '') +
                            (options.autoFixFrozenRowHeight ? 'auto-fix-row ' : '') +
                            (treeCollapseExpandClass || '') + ' ' +
                            (currentRow.CssClass || '') + ' ' +
                            (options.showDistinctRow ? ((rowIndex + 1) % 2 === 0 ? ' freegrid-even' : '') : '');
            trClass = trClass.trim();
            if (options.showTree === true && currentRow.SubRows) {
                parentRowClass = 'parent-row parent-level-' + level;
            }

            frozenRowHtml.push('<tr class="' + trClass + ' ' + parentRowClass + '" ' + (currentRow.CustomAttr || '')
                + trRowIndexAttr + trHighlightAtt + trAutoFixRowAtt + '>');
            for (var columnIndex = 0; columnIndex < colsLength; columnIndex++) {
                var currentCol = currentRow.Columns[columnIndex];
                var customSpan = '';
                var currentHeight = 'height:' + (currentRow.RowHeight || options.bodyRowHeight) + 'px;';
                var currentWidth = me.columnsHastable[currentCol.Name].Width;
                var currentHeightForComputeCenter = options.bodyRowHeight;

                //// If Merge Row, then add rowspan, If in the same row and there are no other columns, then cal height,
                //// or there are some columns but they also have colspan, then cal height
                if (isValid(currentCol.MergeRowNumber)) {
                    customSpan += ' rowspan="' + currentCol.MergeRowNumber + '" ';
                    if (Fanex.Grid.Helper.CheckHideColSpan(currentRow.Columns)) {
                        currentHeightForComputeCenter = ((options.bodyRowHeight * currentCol.MergeRowNumber) + (currentCol.MergeRowNumber - 1) * 5);
                        currentHeight = 'height:' + currentHeightForComputeCenter + 'px;';
                        customSpan = '';
                    }
                }

                //// If Merge Column and current row is footer, then calculate height
                if (isValid(currentCol.MergeColNumber)) {
                    customSpan += ' colspan="' + currentCol.MergeColNumber + '" ';
                    currentWidth = Fanex.Grid.Helper.GetColMergeWidth(me.columnsForComputingWidth,
                                                                        me.columnsHastable[currentCol.Name].ColumnIndex,
                                                                        currentCol.MergeColNumber,
                                                                        options.enableHiddenColumns,
                                                                        options.columns);
                }

                if (currentCol.PrevRowsColMergeNumber) {
                    currentMergeColsIndex += currentCol.PrevRowsColMergeNumber;
                }

                var renderObject = me.RenderBodyCell(
                        {
                            HtmlTag: colHtmlTag,
                            CustomSpan: customSpan,
                            Height: currentHeight
                        },
                        {
                            IsNoData: isForNoData,
                            ColIndex: columnIndex,
                            RowIndex: rowIndex,
                            CssClass: (currentCol.CssClass || (me.columnsHastable[currentCol.Name].CellCssClass || '')),
                            Width: currentWidth,
                            Align: (currentCol.CellAlign || (me.columnsHastable[currentCol.Name].CellAlign || 'left')),
                            CustomAttr: (currentCol.CustomAttr || ''),
                            ParentHighlight: parentHighlight,
                            MergeColumnIndex: currentMergeColsIndex
                        },
                        currentCol,
                        currentRow,
                        isBody,
                        level
                    );
                frozenRowHtml.push(renderObject.CellHtml);
                if (renderObject.CurrentColumnShowTree === true) {
                    currentCollapseExpandId = renderObject.CurrentCollapseExpandId;
                }
                if (isValid(currentCol.MergeColNumber)) {
                    currentMergeColsIndex += currentCol.MergeColNumber - 1;
                }
            }
            frozenRowHtml.push('</tr>');

            if (options.showTree === true && currentRow.SubRows) {
                frozenRowHtml.push(me.RenderSubRow(currentRow, false, isBody, 0, currentCollapseExpandId, parentHighlight, level));
            }

            return frozenRowHtml.join('');
        },
        RenderNormalRow: function (currentRow, colHtmlTag, isBody, rowIndex, treeCollapseExpandClass, isForNoData, parentHighlight, level) {
            var me = this;
            var options = me.Options;
            var rowHtml = [];
            var colsLength = currentRow.Columns.length;
            var currentCollapseExpandId = '';
            var parentRowClass = '';
            var currentMergeColsIndex = 0;
            var trHighlightAtt = (options.showHighlightRow
                ? (' highlightatt="highlight-att-main-' + parentHighlight + '-' + rowIndex + (isBody ? "-body" : "") + '"')
                : '');
            var trAutoFixRow = (options.autoFixFrozenRowHeight ? (' autofixatt="autofix-att-main-' + parentHighlight + '-' + rowIndex + (isBody ? "-body" : "") + '"') : '');
            var trRowIndexAttr = ' data-row-index="' + rowIndex + '" ';
            var trClass = (options.showHighlightRow ? 'highlight-row ' : '') +
                            (options.autoFixFrozenRowHeight ? 'auto-fix-row ' : '') +
                            (currentRow.CssClass || '') + ' ' +
                            (treeCollapseExpandClass || '') + ' ' +
                            (options.showDistinctRow ? ((rowIndex + 1) % 2 === 0 ? ' freegrid-even' : '') : '');
            trClass = trClass.trim();
            if (colsLength > 0) {
                //// If show grid with tree, then add class to parent row
                if (options.showTree === true && currentRow.SubRows) {
                    parentRowClass = 'parent-row parent-level-' + level;
                }
                rowHtml.push('<tr class="' + trClass + ' ' + parentRowClass + '" ' + (currentRow.CustomAttr || '')
                    + trRowIndexAttr + trHighlightAtt + trAutoFixRow + '>');

                for (var colIndex = 0; colIndex < colsLength; colIndex++) {
                    var currentCol = currentRow.Columns[colIndex];
                    var currentWidth = me.columnsHastable[currentCol.Name].Width;
                    var customSpan = '';
                    var currentHeight = 'height:' + (currentRow.RowHeight || options.bodyRowHeight) + 'px;';
                    var currentHeightForComputeCenter = options.bodyRowHeight;
                    if (isValid(currentCol.MergeRowNumber)) {
                        customSpan += ' rowspan="' + currentCol.MergeRowNumber + '" ';
                        currentHeightForComputeCenter *= currentCol.MergeRowNumber;
                    }

                    //// If merge Column, then calculate width for merging
                    if (isValid(currentCol.MergeColNumber)) {
                        customSpan += ' colspan="' + currentCol.MergeColNumber + '" ';
                        currentWidth = Fanex.Grid.Helper.GetColMergeWidth(me.columnsForComputingWidth,
                                                                            me.columnsHastable[currentCol.Name].ColumnIndex,
                                                                            currentCol.MergeColNumber,
                                                                            options.enableHiddenColumns,
                                                                            options.columns);
                    }
                    if (currentCol.PrevRowsColMergeNumber) {
                        currentMergeColsIndex += currentCol.PrevRowsColMergeNumber;
                    }
                    var renderObject = me.RenderBodyCell({
                        HtmlTag: colHtmlTag,
                        CustomSpan: customSpan,
                        Height: currentHeight
                    }, {
                        IsNoData: isForNoData,
                        ColIndex: colIndex,
                        RowIndex: rowIndex,
                        CssClass: (currentCol.CssClass || (me.columnsHastable[currentCol.Name].CellCssClass || '')),
                        Width: currentWidth,
                        Align: (currentCol.CellAlign || (me.columnsHastable[currentCol.Name].CellAlign || 'left')),
                        CustomAttr: (currentCol.CustomAttr || ''),
                        ParentHighlight: parentHighlight,
                        MergeColumnIndex: currentMergeColsIndex
                    },
                        currentCol,
                        currentRow,
                        isBody,
                        level
                    );
                    rowHtml.push(renderObject.CellHtml);
                    if (renderObject.CurrentColumnShowTree === true) {
                        currentCollapseExpandId = renderObject.CurrentCollapseExpandId;
                    }
                    if (isValid(currentCol.MergeColNumber)) {
                        currentMergeColsIndex += currentCol.MergeColNumber - 1;
                    }
                }
                rowHtml.push('</tr>');

                if (options.showTree === true && currentRow.SubRows) {
                    rowHtml.push(me.RenderSubRow(currentRow, true, isBody, 0, currentCollapseExpandId, parentHighlight, level));
                }
            }

            return rowHtml.join('');
        },
        RenderSubRow: function (currentRow, isMain, isBody, rowIndex, currentCollapseExpandId, parentHighlight, level) {
            var me = this;
            var options = me.Options;
            var subRowHtml = [];
            var currentSubRows = (isMain === true ? currentRow.SubRows :
                                    Fanex.Grid.Helper.CreateRowsForFrozenColumn(currentRow.SubRows, options.numberOfFrozenColumn, me.frozenColsName));
            var subRowsLength = currentSubRows.length;
            level++;
            for (var subRowIndex = 0; subRowIndex < subRowsLength; subRowIndex++) {
                var currentSubRow = currentSubRows[subRowIndex];
                if (isMain === true) {
                    subRowHtml.push(me.RenderNormalRow(currentSubRow,
                                                        'td',
                                                        isBody,
                                                        subRowIndex,
                                                        currentCollapseExpandId,
                                                        false,
                                                        parentHighlight + '-' + rowIndex + '-' + subRowIndex, level));
                }
                else {
                    subRowHtml.push(me.RenderFrozenRow(
                                                    currentSubRow,
                                                    'td',
                                                    isBody,
                                                    subRowIndex,
                                                    currentCollapseExpandId,
                                                    false,
                                                    parentHighlight + '-' + rowIndex + '-' + subRowIndex, level));
                }
            }
            return subRowHtml.join('');
        },
        RenderBodyCell: function (container, cell, currentCol, currentRow, isBody, level) {
            var me = this;
            var options = me.Options;
            var cellHtml = [];
            var currentCollapseExpandId = '';

            var currentValue = currentCol.Value;
            if (me.columnsHastable[currentCol.Name].IsAbs && currentValue < 0) {
                currentValue = Math.abs(currentValue);
            }

            if (me.columnsHastable[currentCol.Name].FormatNumber && !isNaN(currentValue) && currentValue !== '') {
                currentValue = numeral(currentValue).format(me.columnsHastable[currentCol.Name].FormatNumber);
            }

            if (me.columnsHastable[currentCol.Name].FormatDateTime &&
                isValid(currentValue) &&
                currentValue !== '' &&
                currentValue != null) {
                var momentObject = moment(currentValue);
                if (momentObject.isValid()) {
                    if (options.timeZone !== '') {
                        momentObject = momentObject.zone(options.timeZone);
                    }

                    currentValue = momentObject.format(me.columnsHastable[currentCol.Name].FormatDateTime);
                }
            }

            if (me.columnsHastable[currentCol.Name].FormatNegative) {
                if (currentCol.Value < 0) {
                    currentValue = '<span class="format-negative">' + me.columnsHastable[currentCol.Name].FormatNegative.replace('$', currentValue) + '</span>';
                }
            }

            if (me.columnsHastable[currentCol.Name].FormatPositive) {
                if (currentCol.Value > 0) {
                    currentValue = '<span class="format-positive">' + me.columnsHastable[currentCol.Name].FormatPositive.replace('$', currentValue) + '</span>';
                }
            }

            var cellContent = '';
            if (cell.IsNoData === true) {
                cellContent = currentCol.Value;
            }
            else {
                cellContent = typeof currentCol.CustomCellRender === 'function' ?
                                                currentCol.CustomCellRender(currentCol, currentValue) :
                                                ((typeof (me.columnsHastable[currentCol.Name].CustomCellRender) === 'function' && isBody) ?
                                                            me.columnsHastable[currentCol.Name].CustomCellRender(currentCol, currentValue) :
                                                            currentValue);
            }

            //// If show tree, then add icon expand
            var tdClass = 'free-grid-td-' + (cell.ColIndex + cell.MergeColumnIndex) + ' '
                            + (currentCol.TdClass || (me.columnsHastable[currentCol.Name].TdClass || ''));
            if (options.showTree === true && currentRow.SubRows && me.columnsHastable[currentCol.Name].ShowCollapseExpand === true) {
                tdClass = 'tr-grid-collapse-expand';
            }

            //// For show/hide column
            if (me.columnsHastable[currentCol.Name].CanShowHide && me.columnsHastable[currentCol.Name].CanShowHide === true && (!options.enableHiddenColumns)) {
                tdClass += ' grid-show-hide-column';
            }

            var truncateClass = '';
            var allowTruncate = me.columnsHastable[currentCol.Name].AllowTruncate === true ? true : options.allowTruncate;
            if (allowTruncate === true) {
                truncateClass = ' column-truncate';
            }

            var titleTooltip = '';
            if (typeof (currentCol.Value) === 'string' &&
                (currentCol.Value.length * me.currentFontSize > me.columnsHastable[currentCol.Name].Width) &&
                allowTruncate === true) {
                titleTooltip = ' title="' + currentCol.Value + '" ';
            }

            var isCurrentColShowTree = (options.showTree === true &&
            currentRow.SubRows &&
            currentRow.SubRows.length > 0 && me.columnsHastable[currentCol.Name].ShowCollapseExpand === true);

            var paddingLeft = '';
            if ((isCurrentColShowTree && level !== 0) ||
            (level > 0 &&
                !(currentRow.SubRows && currentRow.SubRows.length > 0) &&
                me.columnsHastable[currentCol.Name].ShowCollapseExpand === true) ||
            (level === 0 && isCurrentColShowTree &&
                (!isValid(currentRow.SubRows) || currentRow.SubRows.length === 0))) {
                var totalPaddingLeft = level * options.treeParentPaddingLeft;
                paddingLeft = 'padding-left:' + totalPaddingLeft + 'px;';
                cell.Width -= totalPaddingLeft;
            }

            cellHtml.push('<' + container.HtmlTag + ' ' + container.CustomSpan + ' class="' + tdClass + '"' + ' style="' + container.Height + '"' + ' >');
            cellHtml.push('<div class="table-cell ' +
                            cell.CssClass + ' ' +
                            truncateClass +
                            '" style="width:' + cell.Width + 'px;' + paddingLeft + '" ' +
                            'align="' + cell.Align + '" ' + titleTooltip + cell.CustomAttr + '>');

            if (isCurrentColShowTree) {
                currentCollapseExpandId = me.gridId + '_tree_' + cell.ParentHighlight + (isBody ? "-body" : "");
                cellHtml.push('<span class="grid-collapse-expand grid-expand ' +
                                currentCollapseExpandId + '-class' +
                                '" id="' + currentCollapseExpandId + '"></span>');
                currentCollapseExpandId += ' child-row';
            }

            var showRowIndexNumber = (isBody && options.showNumberColumn && cell.ColIndex === 0 && !cell.IsNoData);
            var currentRowIndexForCustomPager = 0;
            if (showRowIndexNumber) {
                if (options.customPager) {
                    currentRowIndexForCustomPager = cell.RowIndex + options.currentStartRowIndex + 1;
                }
                else {
                    currentRowIndexForCustomPager = (cell.RowIndex + 1);
                }

                if (me.columnsHastable[currentCol.Name].FormatNumber) {
                    currentRowIndexForCustomPager = numeral(currentRowIndexForCustomPager).format(me.columnsHastable[currentCol.Name].FormatNumber);
                }
            }

            cellHtml.push(showRowIndexNumber ? currentRowIndexForCustomPager : cellContent);
            cellHtml.push('</div></' + container.HtmlTag + '>');

            return {
                CellHtml: cellHtml.join(''),
                CurrentCollapseExpandId: currentCollapseExpandId,
                CurrentColumnShowTree: isCurrentColShowTree
            };
        },
        RenderEmptyRow: function (columnName, columnValue, mergeColumnNumber, cssClass, showOnMainBody) {
            var me = this;
            var options = me.Options;
            var minusColumnNumber = 0;
            var hiddenColumns = options.hiddenColumns,
                i;
            if (options.numberOfFrozenColumn > 0) {
                for (i = 0; i < hiddenColumns.length; i++) {
                    if (hiddenColumns[i] < mergeColumnNumber) {
                        minusColumnNumber++;
                    }
                }
            }
            var columns = [{
                Name: columnName,
                Value: columnValue,
                MergeColNumber: mergeColumnNumber - minusColumnNumber,
                CssClass: cssClass
            }];
            if (showOnMainBody) {
                var minusMainColumnNumber = mergeColumnNumber - minusColumnNumber;
                if (options.numberOfFrozenColumn > 0) {
                    for (i = 0; i < hiddenColumns.length; i++) {
                        if (hiddenColumns[i] >= mergeColumnNumber) {
                            minusMainColumnNumber++;
                        }
                    }
                }
                columns.push({
                    Name: options.columns[minusMainColumnNumber].Name,
                    Value: fanexGridCurrentLanguage.noData,
                    MergeColNumber: (options.columns.length - minusMainColumnNumber),
                    CssClass: cssClass
                });
            }

            var emptyRowHtml = this.RenderNormalRow({
                Columns: columns
            }, 'td', true, 0, '', true, '');
            return emptyRowHtml;
        },
        RenderHeaderCell: function (cellInfo, currentColumn) {
            var me = this;
            var options = me.Options;
            var cellHtml = [];
            var cellTag = cellInfo.CellTag || 'td';
            var customSpan = cellInfo.CustomSpan || ' ';
            var cssClass = cellInfo.CssClass || ' ';
            var sortClass = cellInfo.SortClass || ' ';
            var customAttr = cellInfo.CustomAttr || ' ';
            var thClass = cellInfo.ThClass || ' ';
            var cellWidth = 'width:' + (cellInfo.CellWidth || '200') + 'px;';
            var cellHeight = isValid(cellInfo.CellHeight) ? ('height:' + (cellInfo.CellHeight || options.headerColumnHeight) + 'px;') : ' ';
            var cellId = isValid(cellInfo.CellId) ? ' id="' + cellInfo.CellId + '"' : " ";
            var sort = cellInfo.Sort || false;
            var cellColumnName = cellInfo.CellColumnName || '';
            var showClass = (isValid(cellInfo.CanShowHide) && cellInfo.CanShowHide) ? ('grid-show-hide-column') : ' ' + thClass;

            cellHtml.push('<' + cellTag + ' ' + customSpan + ' class="' + showClass + '" >');

            cellHtml.push('<div class="table-cell ' + cssClass + ' ' + sortClass +
                            '" style="' + cellWidth + cellHeight + '" ' + cellId +
                            ' align="' + cellInfo.CellAlign + '" ' + customAttr + '>');
            if (sort === true) {
                cellHtml.push('<div class="display-inline-block">');
            }

            if (typeof cellInfo.CustomHeaderRender === 'function') {
                cellHtml.push(cellInfo.CustomHeaderRender(currentColumn));
            }
            else {
                cellHtml.push(cellInfo.HeaderText);
            }

            if (sort === true) {
                var sortDirection = cellInfo.SortDirection || '';
                cellHtml.push('</div><div class="divSorter ' + sortDirection.toLowerCase() + '" data-columnname="' + cellColumnName + '"> </div>');
            }

            cellHtml.push('</div>');
            cellHtml.push('</' + cellTag + '>');
            return cellHtml.join('');
        },
        RenderBody: function (headerHtml, reRenderBody) {
            var me = this;
            var options = this.Options;
            var bodyHtml = (reRenderBody === true ? '' : (headerHtml + me.RenderCommonHeader('mainbody')));
            var bodyRowsHtml = [];
            var bodyFrozenHtml = [];
            var currentBodyRows = [];
            var currentPageStartIndex = 0,
                currentRow;

            if (options.sortOption.serverSortFunction == null &&
                options.sortOption.customGridSort == null) {
                var defaultSort = false,
                customSortFunction = null,
                sortType = null,
                columnName = '',
                currentDirection = '';
                for (var colIndex = 0; colIndex < options.columns.length; colIndex++) {
                    if (options.columns[colIndex].SortDirection != null
                    && options.columns[colIndex].SortDirection !== '') {
                        defaultSort = true;
                        currentDirection = options.columns[colIndex].SortDirection;
                        customSortFunction = options.columns[colIndex].CustomSortFunction;
                        columnName = options.columns[colIndex].Name;
                        sortType = options.columns[colIndex].SortType;
                        break;
                    }
                }

                if (defaultSort) {
                    options.bodyRows = Fanex.Grid.Helper.GetSortedArray(options.bodyRows,
                                                                    columnName,
                                                                    currentDirection,
                                                                    customSortFunction,
                                                                    sortType);
                }
            }

            //// If show pager, then take approriate data range
            if (options.showPager === true && options.customPager !== true) {
                var startIndex = (options.pagerOption.currentPage * options.pagerOption.itemsPerPage);
                var endIndex = ((options.pagerOption.currentPage + 1) * options.pagerOption.itemsPerPage);
                currentBodyRows = options.bodyRows.slice(startIndex, endIndex);
                currentPageStartIndex = startIndex;
            }
            else {
                this.Options.currentStartRowIndex = options.pagerOption.currentPage * options.pagerOption.itemsPerPage;
                currentBodyRows = options.bodyRows;
            }

            if (options.virtualScroller
                && currentBodyRows.length > options.thresholdNeedToVirtualRendering) {
                options.shouldVirtualScroller = true;
            }
            else {
                options.shouldVirtualScroller = false;
            }

            var bodyLength = currentBodyRows.length;
            var currentFrozenRows = (options.numberOfFrozenColumn > 0) ?
                                    Fanex.Grid.Helper.CreateRowsForFrozenColumn(currentBodyRows, options.numberOfFrozenColumn, me.frozenColsName) :
                                    [];
            var frozenBodyLength = currentFrozenRows.length;

            if (bodyLength > 0) {
                var realLength = bodyLength;
                var realFrozenLength = frozenBodyLength;
                if (options.shouldVirtualScroller) {
                    if (frozenBodyLength > 0
                        && frozenBodyLength > options.numOfVirtualRows) {
                        //virtualFrozenLength =
                        frozenBodyLength = options.numOfVirtualRows;
                    }

                    if (bodyLength > options.numOfVirtualRows) {
                        bodyLength = options.numOfVirtualRows;
                    }

                    options.totalHeight = Fanex.Grid.Helper.ComputeBodyHeight(currentBodyRows, options.bodyRowHeight);
                    options.realLength = realLength;
                    options.allBodyRowsHtml = [];
                    options.allFrozenRowsHtml = [];
                    options.rowStartIndex = 0;
                    options.rowEndIndex = bodyLength;
                }

                //// Render Frozen Row and Main Row at the same time
                var bodyRowHtml = '';
                var frozenRowHtml = '';
                for (var rowIndex = 0; rowIndex < realFrozenLength; rowIndex++) {
                    currentRow = currentBodyRows[rowIndex];
                    var currentFrozenRow = currentFrozenRows[rowIndex];
                    bodyRowHtml = me.RenderNormalRow(currentRow, 'td', true, rowIndex + currentPageStartIndex, '', false, rowIndex, 0);
                    frozenRowHtml = me.RenderFrozenRow(currentFrozenRow, 'td', true, rowIndex + currentPageStartIndex, '', false, rowIndex, 0);

                    if (rowIndex < frozenBodyLength) {
                        bodyRowsHtml.push(bodyRowHtml);
                        bodyFrozenHtml.push(frozenRowHtml);
                    }

                    if (options.shouldVirtualScroller) {
                        options.allBodyRowsHtml.push(bodyRowHtml);
                        options.allFrozenRowsHtml.push(frozenRowHtml);
                    }
                }

                //// If MainRow still has item, then continue render
                for (rowIndex = realFrozenLength; rowIndex < realLength; rowIndex++) {
                    currentRow = currentBodyRows[rowIndex];

                    bodyRowHtml = me.RenderNormalRow(currentRow, 'td', true, rowIndex + currentPageStartIndex, '', false, rowIndex, 0);

                    if (rowIndex < bodyLength) {
                        bodyRowsHtml.push(bodyRowHtml);
                    }

                    if (options.shouldVirtualScroller) {
                        options.allBodyRowsHtml.push(bodyRowHtml);
                    }
                }
            }
            else {
                //// Render There is no data, showNoDataOnFrozen
                var emptyData = fanexGridCurrentLanguage.noData;
                if (options.numberOfFrozenColumn > 0) {
                    if (options.showNoDataOnFrozen) {
                        //// Show No Info on the frozen table
                        bodyRowsHtml.push(me.RenderEmptyRow(options.columns[0].Name, emptyData, options.columns.length, 'no-data-frozen', false));
                        bodyFrozenHtml.push(me.RenderEmptyRow(options.columns[0].Name, emptyData, options.numberOfFrozenColumn, 'no-data-frozen', false));
                    }
                    else {
                        //// Show No Info on the main table
                        bodyRowsHtml.push(me.RenderEmptyRow(options.columns[0].Name, '', options.numberOfFrozenColumn, 'no-data-frozen', true));
                        bodyFrozenHtml.push(me.RenderEmptyRow(options.columns[0].Name, '', options.numberOfFrozenColumn, '', false));
                    }
                }
                else {
                    //// Show No Info on the main table
                    bodyRowsHtml.push(me.RenderEmptyRow(options.columns[0].Name, emptyData, options.columns.length, 'no-data-not-frozen', false));
                }
            }

            if (reRenderBody === true) {
                me.$grid.find('#' + me.frozenContentId + '> tbody').remove();
                me.$grid.find('#' + me.tableContentId + '> tbody').remove();
                var div = document.createElement("div");
                div.innerHTML = "<table><tbody>" + bodyFrozenHtml.join('') + "</tbody></table>";
                var frozenContainer = document.getElementById(me.frozenContentId);
                if (frozenContainer) {
                    frozenContainer.appendChild(div.firstChild.tBodies[0]);
                }
                div.innerHTML = "<table><tbody>" + bodyRowsHtml.join('') + "</tbody></table>";
                var mainContainer = document.getElementById(me.tableContentId);
                if (mainContainer) {
                    mainContainer.appendChild(div.firstChild.tBodies[0]);
                }
            }
            else {
                // Fix for replace $' string
                var bodyHtmlTemp = bodyHtml.split('{$}frozenBody');
                if (bodyHtmlTemp.length === 2) {
                    bodyHtml = bodyHtmlTemp[0] + bodyFrozenHtml.join('') + bodyHtmlTemp[1];
                }
                bodyHtmlTemp = bodyHtml.split('{$}_');
                if (bodyHtmlTemp.length === 2) {
                    bodyHtml = bodyHtmlTemp[0] + bodyRowsHtml.join('') + bodyHtmlTemp[1];
                }
            }

            return Fanex.Grid.Helper.RemoveSpaceIE9(bodyHtml);
        },
        RenderFooter: function (reRender) {
            var me = this,
            //options = me.Options,
                footerHtml = [],
                frozenFooterHtml = [],
                currentFooterHtml = me.RenderFooterBody();
            reRender = reRender || false;
            if (reRender) {
                //currentFooterHtml
                me.$grid.find('.row_footer_content > thead').empty().append(currentFooterHtml.frozenFooterHtml);
                me.$grid.find('.col_footer_content > thead').empty().append(currentFooterHtml.footerHtml);
            }
            else {
                footerHtml.push('<div class="col_footer row-footer-background scroll-y">' +
                                '<div class="footer-container">' +
                                    '<table class="col_footer_content grid-fixed-table">' +
                                        '<thead>');

                frozenFooterHtml.push(' <div class="row_footer row-footer-background z999">' +
                                    ' <table class="grid-fixed-table row_footer_content">' +
                                    '    <thead>');

                currentFooterHtml = me.RenderFooterBody();

                footerHtml.push(currentFooterHtml.footerHtml);

                frozenFooterHtml.push(currentFooterHtml.frozenFooterHtml);

                frozenFooterHtml.push('   </thead>' + '</table>' + '</div>');

                footerHtml.push(' </thead>' + '</table>' + '</div>' + '</div>');

                return frozenFooterHtml.join('') + footerHtml.join('');
            }
        },
        RenderFooterBody: function () {
            var me = this;
            var options = me.Options;
            var footerHtml = [];
            var frozenFooterHtml = [];
            var footerRowsLength = options.footerRows.length,
                currentRow;

            var currentFrozenRows = (options.numberOfFrozenColumn > 0) ?
                                    Fanex.Grid.Helper.CreateRowsForFrozenColumn(options.footerRows, options.numberOfFrozenColumn, me.frozenColsName) :
                                    [];
            var frozenFooterLength = currentFrozenRows.length;

            //// Render Frozen fotter and main Footer
            for (var rowIndex = 0; rowIndex < frozenFooterLength; rowIndex++) {
                currentRow = options.footerRows[rowIndex];
                var currentFrozenRow = currentFrozenRows[rowIndex];
                footerHtml.push(me.RenderNormalRow(currentRow, 'th', false, rowIndex, '', false, rowIndex, 0));
                frozenFooterHtml.push(me.RenderFrozenRow(currentFrozenRow, 'th', false, rowIndex, '', false, rowIndex, 0));
            }

            //// Render only main Footer
            for (rowIndex = frozenFooterLength; rowIndex < footerRowsLength; rowIndex++) {
                currentRow = options.footerRows[rowIndex];
                footerHtml.push(me.RenderNormalRow(currentRow, 'th', false, rowIndex, '', false, rowIndex, 0));
            }

            return {
                footerHtml: footerHtml.join(''),
                frozenFooterHtml: frozenFooterHtml.join('')
            };
        },
        RenderPager: function () {
            var me = this;
            var options = this.Options;
            var pagerOption = {};
            if (options.bodyRows.length > 0) {
                if ($.fn.fanexPagination === undefined) {
                    alert('Please include Fanex.Pagination.cs');
                    return;
                }

                if (options.customPager && options.customPager === true) {
                    pagerOption =
                    {
                        callback: function (pageIndex, itemsPerPage, jq) {
                            if (me.Options.pagerOption.itemsPerPage !== itemsPerPage) {
                                pageIndex = 0;
                            }
                            options.pagerOption.pagerSelectCallBack(pageIndex, itemsPerPage, jq);
                        }
                    };
                }
                else {
                    pagerOption =
                    {
                        callback: function (pageIndex, itemsPerPage) {
                            if (me.Options.pagerOption.itemsPerPage !== itemsPerPage) {
                                pageIndex = 0;
                            }
                            me.Options.pagerOption.currentPage = pageIndex;
                            me.Options.pagerOption.itemsPerPage = itemsPerPage;
                            me.RenderBody('', true);
                            ////Re-render footer
                            ////options.showFooter && options.bodyRows.length > 0 ? me.RenderFooter() : ""
                            if (options.showFooter && options.bodyRows.length > 0 && typeof (options.pagerOption.customRenderFooter) === 'function') {
                                var startIndex = (options.pagerOption.currentPage * options.pagerOption.itemsPerPage);
                                var endIndex = ((options.pagerOption.currentPage + 1) * options.pagerOption.itemsPerPage);
                                var currentBodyRows = options.bodyRows.slice(startIndex, endIndex);
                                me.Options.footerRows = options.pagerOption.customRenderFooter(
                                                                                                currentBodyRows,
                                                                                                options.pagerOption.currentPage,
                                                                                                options.pagerOption.itemsPerPage);
                                me.RenderFooter(true);
                            }

                            me.gridOptions.isResetScroller = true;
                            me.RenderGridScroller(me.gridOptions);
                            me.RenderPager();
                            me.RegisterMouseHoverEvent();
                            me.AutoResizeRowHeight();
                            //// Save for rerender
                            Fanex.Grid.List[me.tableContentId].pagerOption = me.Options.pagerOption;
                            if (typeof (options.pagerOption.afterPagedFunction) === 'function') {
                                options.pagerOption.afterPagedFunction(options.pagerOption);
                            }
                            return false;
                        }
                    };
                }

                pagerOption.startPerPage = options.pagerOption.startPerPage;
                pagerOption.stepPerPage = options.pagerOption.pageStep;
                pagerOption.maxTtemsPerPage = parseInt(options.pagerOption.maxItemsPerPage || 0);
                pagerOption.itemsPerPage = parseInt(options.pagerOption.itemsPerPage);
                pagerOption.currentPage = parseInt(options.pagerOption.currentPage);
                pagerOption.totalItem = options.pagerOption.totalItem;
                pagerOption.itemsPerPageArray = options.pagerOption.itemsPerPageArray;
                pagerOption.showDetail = options.pagerOption.showDetail;

                this.$grid.find('#pagination').fanexPagination(pagerOption.totalItem, pagerOption);
            }
        },
        RegisterEvents: function () {
            var me = this;
            var options = me.Options;
            //// Sorting
            me.RegisterSortEvent();

            //// For Onclick Title Button
            if (options.showTitle === true) {
                if (typeof options.titleInfo.exportExcelFunction === 'function') {
                    me.$grid.parent().off('click', '#btnExport' + me.gridId, null).on('click', '#btnExport' + me.gridId, options.titleInfo.exportExcelFunction);
                }
                if (typeof options.titleInfo.backFunction === 'function') {
                    me.$grid.parent().off('click', '#btnBack' + me.gridId, null).on('click', '#btnBack' + me.gridId, options.titleInfo.backFunction);
                }
                if (typeof options.titleInfo.sendMailFunction === 'function') {
                    me.$grid.parent()
                            .off('click', '#btnSendMail' + me.gridId, null)
                            .on('click', '#btnSendMail' + me.gridId, options.titleInfo.sendMailFunction);
                }
                if (options.titleInfo.showCollapseExpandButton === true) {
                    me.$grid.parent().off('click', '#btnCollapseExpand' + me.gridId, null).on('click', '#btnCollapseExpand' + me.gridId, function () {
                        var $this = $(this);
                        if ($this.hasClass('expanded')) {
                            me.$grid.find('.child-row').hide();
                            me.$grid.find('.grid-collapse-expand').removeClass('grid-expand').addClass('grid-collapse');
                            $this.removeClass('expanded').addClass('collapsed');
                        }
                        else {
                            $this.removeClass('collapsed').addClass('expanded');
                            me.$grid.find('.grid-collapse-expand').removeClass('grid-collapse').addClass('grid-expand');
                            me.$grid.find('.child-row').show();
                        }
                        me.gridOptions.isResetScroller = false;
                        var gridOptions = Fanex.Grid.List[me.tableContentId].gridOptions;
                        if (gridOptions) {
                            me.gridOptions.gridHeight = gridOptions.gridHeight;
                        }
                        me.RenderGridScroller(me.gridOptions);
                    });

                    if (options.defaultCollapseAllGroups) {
                        me.$grid.parent().find('#btnCollapseExpand' + me.gridId).trigger('click');
                    }
                }
            }

            //// Bind collapse expand event
            if (options.showTree === true) {
                me.$grid
                    .off('click', '.parent-row > td.tr-grid-collapse-expand, .parent-row > th.tr-grid-collapse-expand', null)
                    .on('click', '.parent-row > td.tr-grid-collapse-expand, .parent-row > th.tr-grid-collapse-expand', function () {
                        var $this = $(this);
                        var collapseexpandspan = $this.find('.grid-collapse-expand');
                        var isExpand = false;
                        if (collapseexpandspan.hasClass('grid-expand')) {
                            me.$grid.find('.' + collapseexpandspan.attr('id') + '-class').removeClass('grid-expand').addClass('grid-collapse');
                            isExpand = false;
                        }
                        else {
                            me.$grid.find('.' + collapseexpandspan.attr('id') + '-class').removeClass('grid-collapse').addClass('grid-expand');
                            isExpand = true;
                        }
                        var $child = me.$grid.find('.' + collapseexpandspan.attr('id'));
                        if ($child.length > 0) {
                            $child.toggle();
                            for (var index = 0; index < $child.length; index++) {
                                var $currentChild = $($child[index]);
                                me.RecursiveCollapseExpandChild($currentChild, isExpand);
                            }
                            me.gridOptions.isResetScroller = false;
                            me.RenderGridScroller(me.gridOptions);
                        }
                    });
            }
        },
        RecursiveCollapseExpandChild: function ($current, isExpand) {
            var me = this;
            var collapseexpandspan = $current.find('.grid-collapse-expand');
            var $child = me.$grid.find('.' + collapseexpandspan.attr('id'));

            if (isExpand) {
                if (collapseexpandspan.hasClass('grid-expand')) {
                    $child.show();
                }
                else {
                    $child.hide();
                }
            }
            else {
                $child.hide();
            }

            for (var index = 0; index < $child.length; index++) {
                var $currentChild = $($child[index]);
                me.RecursiveCollapseExpandChild($currentChild, isExpand);
            }
        },
        RegisterSortEvent: function () {
            var me = this;
            var options = me.Options;
            //// For Sorting
            var colsLength = options.columns.length;
            for (var colIndex = 0; colIndex < colsLength; colIndex++) {
                var currentCol = options.columns[colIndex];

                if (currentCol.Sort === true) {
                    /*jshint ignore:start*/
                    me.$grid.off('click', '#' + me.gridId + '_header_sort_' + currentCol.Name, null)
                            .on('click', '#' + me.gridId + '_header_sort_' + currentCol.Name, function () {
                                var $this = $(this).find('.divSorter');
                                var currentDirection = '';
                                if ($this.hasClass('desc')) {
                                    me.$grid.find('.divSorter').removeClass('asc').removeClass('desc');
                                    $this.addClass('asc');
                                    currentDirection = 'asc';
                                }
                                else if ($this.hasClass('asc')) {
                                    me.$grid.find('.divSorter').removeClass('asc').removeClass('desc');
                                    $this.addClass('desc');
                                    currentDirection = 'desc';
                                }
                                else {
                                    me.$grid.find('.divSorter').removeClass('asc').removeClass('desc');
                                    $this.addClass('desc');
                                    currentDirection = 'desc';
                                }
                                var customSortFunction = null;
                                var sortType = null;
                                for (var colIndex = 0; colIndex < options.columns.length; colIndex++) {
                                    if (options.columns[colIndex].Name === $this.data('columnname')) {
                                        options.columns[colIndex].SortDirection = currentDirection.toLowerCase();
                                        customSortFunction = options.columns[colIndex].CustomSortFunction;
                                        sortType = options.columns[colIndex].SortType;
                                    }
                                    else {
                                        options.columns[colIndex].SortDirection = '';
                                    }
                                }
                                //customGridSort: null
                                if (typeof (options.sortOption.serverSortFunction) === 'function') {
                                    options.sortOption.serverSortFunction($this.data('columnname'), currentDirection);
                                }
                                else {
                                    if (typeof (options.sortOption.beforeGridSorted) === 'function') {
                                        options.bodyRows = options.sortOption.beforeGridSorted(options.bodyRows, $this.data('columnname'), currentDirection);
                                    }
                                    if (typeof (options.sortOption.customGridSort) === 'function') {
                                        options.bodyRows = options.sortOption.customGridSort(
                                                            options.bodyRows,
                                                            $this.data('columnname'),
                                                            currentDirection,
                                                            customSortFunction,
                                                            sortType);
                                    }
                                    else {
                                        options.bodyRows = Fanex.Grid.Helper.GetSortedArray(options.bodyRows,
                                                                                        $this.data('columnname'),
                                                                                        currentDirection,
                                                                                        customSortFunction,
                                                                                        sortType);
                                    }
                                    me.ReRenderBodyAfterSort($this.data('columnname'), currentDirection);
                                    if (typeof (options.sortOption.afterGridSorted) === 'function') {
                                        options.bodyRows = options.sortOption.afterGridSorted(options.bodyRows, $this.data('columnname'), currentDirection);
                                    }
                                }
                            });
                    /*jshint ignore:end*/
                    continue;
                }
            }
        },
        RegisterMouseHoverEvent: function () {
            var me = this;
            var options = me.Options;
            //// For Mouse hover event
            if (options.showHighlightRow === true && options.numberOfFrozenColumn > 0) {
                me.$grid
                    .off('mouseenter.grid', '.highlight-row')
                    .on('mouseenter.grid', '.highlight-row', function () {
                        var $this = $(this);
                        var hlattr = $this.attr('highlightatt');
                        if (isValid(hlattr)) {
                            if (hlattr.indexOf('frozen') > 0) {
                                me.$grid.find('tr[highlightatt="' + hlattr.replace('frozen', 'main') + '"]').addClass('mousehoverbeyond');
                            }
                            else {
                                me.$grid.find('tr[highlightatt="' + hlattr.replace('main', 'frozen') + '"]').addClass('mousehoverbeyond');
                            }
                        }
                    })
                    .off('mouseleave.grid', '.highlight-row')
                    .on('mouseleave.grid', '.highlight-row', function () {
                        var $this = $(this);
                        var hlattr = $this.attr('highlightatt');
                        if (isValid(hlattr)) {
                            if (hlattr.indexOf('frozen') > 0) {
                                me.$grid.find('tr[highlightatt="' + hlattr.replace('frozen', 'main') + '"]').removeClass('mousehoverbeyond');
                            }
                            else {
                                me.$grid.find('tr[highlightatt="' + hlattr.replace('main', 'frozen') + '"]').removeClass('mousehoverbeyond');
                            }
                        }
                    });
            }
        },
        AutoResizeRowHeight: function () {
            var me = this,
                options = me.Options;
            if (options.autoFixFrozenRowHeight) {
                var extraHeight = 3;
                me.$grid.find('.content-container .auto-fix-row').each(function () {
                    var $this = $(this),
                        currentHeight = $this.height() + extraHeight,
                        atfattr = $this.attr('autofixatt');

                    if (isValid(atfattr)) {
                        me.$grid.find('.row_header tr[autofixatt="' + atfattr.replace('main', 'frozen') + '"]')
                            .attr('frozen-row-computed-height', currentHeight + 'px');
                        $this.attr('frozen-row-computed-height', currentHeight + 'px');
                    }
                });

                me.$grid.hide().find('tr.auto-fix-row').each(function () {
                    var $this = $(this);
                    $this.css('height', $this.attr('frozen-row-computed-height'));
                });
                me.$grid.show();
                // Header
                var currentFrozenHeader = me.$grid.find('.grid-fixed-table.free-cell-top-left thead tr'),
                    currentFrozenBody = me.$grid.find('.grid-fixed-table.row_header_content thead tr');
                if (currentFrozenHeader.length > 0 && currentFrozenBody.length) {
                    me.$grid.find('.grid-fixed-table.col_header_content thead tr').each(function (index) {
                        var $trTable = $(this),
                            currentHeight = $trTable.height();

                        $(currentFrozenHeader[index]).css('height', currentHeight + 'px');
                        $(currentFrozenBody[index]).css('height', currentHeight + 'px');
                        //$trTable.css('height', currentHeight + 'px');
                    });
                }
                // Footer
                var currentFrozenFooter = me.$grid.find('.grid-fixed-table.row_footer_content thead tr');
                if (currentFrozenFooter.length > 0) {
                    me.$grid.find('.col_footer_content.grid-fixed-table thead tr').each(function (index) {
                        var $trTable = $(this),
                            currentHeight = $trTable.height();
                        $(currentFrozenFooter[index]).css('height', currentHeight + 'px');
                        //$trTable.css('height', currentHeight + 'px');
                    });
                }

                //$(me.$grid).gridScroller(me.gridOptions);
                me.RenderGridScroller(me.gridOptions);
            }
        },
        InitGridScroller: function (isShowPager) {
            //$.fn.initGrid
            var me = this;
            //Render Free Cell
            var freeCellTpl = '<div class="free-cell free-bottom-left">\
                    <table class="grid-fixed-table">\
                        <thead>\
                            <tr>\
                                <th class="free-cell-bottom-left">\
                                    <div>\
                                    </div>\
                                </th>\
                            </tr>\
                        </thead>\
                    </table>\
                </div>\
                <div class="free-cell free-top-right">\
                    <table class="grid-fixed-table">\
                        <thead>\
                            <tr>\
                                <th class="free-cell-top-right">\
                                    <div>\
                                    </div>\
                                </th>\
                            </tr>\
                        </thead>\
                    </table>\
                </div>\
                <div class="free-cell free-bottom-right">\
                    <table class="grid-fixed-table">\
                        <thead>\
                            <tr>\
                                <th class="free-cell-bottom-right">\
                                    <div>\
                                    </div>\
                                </th>\
                            </tr>\
                        </thead>\
                    </table>\
                </div>';
            me.$grid.append(freeCellTpl);
            //Render Scrollbar
            var scrollbarTpl = '<div class="y-scrollbar">\
                    <div class="scrollbar-y-content">\
                        &nbsp;</div>\
                </div>\
                <div class="x-scrollbar">\
                    <div class="scrollbar-x-content">\
                        &nbsp;</div>\
                </div>';
            me.$grid.append(scrollbarTpl);
            if (isShowPager) {
                var pagerTpl = '<div class="grid-pager">\
                                <div class="grid-pager-content">\
                                    <div id="pagination">&nbsp;</div>\
                                </div>\
                            </div>';
                me.$grid.append(pagerTpl);
            }
        },
        ResetGrid: function () {
            var me = this;
            var o = me.$grid;
            //$.fn.resetGrid
            var xscrollbar = o.find(".x-scrollbar");
            var container = o.find(".grid-container");
            var colHeaderContainer = o.find(".col_header");
            var colFooterContainer = o.find(".col_footer");

            o.css("width", "");
            container.css("width", "");
            o.find(".content-container").css("width", "");
            xscrollbar.css("width", "");
            colHeaderContainer.css("width", "");
            colFooterContainer.css("width", "");
            colHeaderContainer.find(".header-container").css("width", "");
            o.find(".footer-container").css("width", "");
            o.find(".scrollbar-x-content").css("width", "");
            o.find(".free-cell-bottom-left").css("width", "");
        },
        ResetGridScroller: function (o, container, rowheader, xscrollbar, yscrollbar) {
            yscrollbar.prop("scrollTop", 0);
            container.prop("scrollTop", 0);
            rowheader.prop("scrollTop", 0);

            container.prop("scrollLeft", 0);
            xscrollbar.prop("scrollLeft", 0);
            o.find(".col_header").prop("scrollLeft", 0);
            o.find(".col_footer").prop("scrollLeft", 0);
        },
        RenderGridScroller: function (scrollerOptions) {
            var scrollerDefaults = {
                tblContentId: 'grid1',
                isUseLeftTable: false,
                isShowPager: false,
                column: [],
                numberFreezeCol: 2,
                fullWidth: true,
                fullHeight: true,
                gridWidth: 0,
                //=totalColumnWidth + totalColumn*4 +totalColumn
                minWidth: 600,
                minHeight: 400,
                gridHeight: 300,
                freezeWidth: 241,
                //=totalColumnWidth + (totalColumn-1)*2 + (totalColumn-1)
                padding: 2,
                marginRight: 20,
                isResetScroller: true,
                highlightFrozenWidth: 0,
                heightScrollYAdd: 0,
                hideYBackGroundScroller: true,
                bottomBorderWidth: 3
            };
            //$.fn.gridScroller
            var me = this;
            var o = me.$grid;
            var xscrollbar = o.find(".x-scrollbar");
            var yscrollbar = o.find(".y-scrollbar");
            var container = o.find(".grid-container");
            var rowheader = o.find(".row_header");
            o.parent().removeClass('display-none')
                .show().find('.grid-title').removeClass('display-none');
            o.show();
            var options = $.extend({}, scrollerDefaults, scrollerOptions);

            if (!isValid(Fanex.Grid.List[options.tblContentId])) {
                $(window).resize(function () {
                    if (o.is(':hidden') || o.parent().is(':hidden')) {
                        return null;
                    }

                    var opt = $.extend({}, {}, Fanex.Grid.List[options.tblContentId].gridOptions);
                    opt.manualCopyHeader = true;
                    me.RenderGridFrames(opt);
                });
                Fanex.Grid.List[options.tblContentId] =
                {
                    gridOptions: options,
                    pagerOption: null,
                    sortOption: null
                };
            }
            else {
                Fanex.Grid.List[options.tblContentId].gridOptions = options;
            }

            me.RenderGridFrames(options);
            //o.find('.grid-fixed-table').append(' ');

            if (options.isResetScroller) {
                me.ResetGridScroller(o, container, rowheader, xscrollbar, yscrollbar);
            } else {
                var top = yscrollbar.scrollTop();
                container.prop("scrollTop", top);
                rowheader.prop("scrollTop", top);
            }

            me.RegisterScrollerEvents(o, container, rowheader, xscrollbar, yscrollbar);
        },
        RenderGridFrames: function (options) {
            var bar = 17;
            var me = this;
            var o = me.$grid;

            var isParentShow = o.parent().is(':visible');
            if (!isParentShow) {
                o.parent().css({ position: "absolute", visibility: "hidden", display: "block" });
            }

            var title = o.find(".grid-title");
            var titleHeight = (!title.hasClass('display-none') && title.length > 0) ? title.height() : 0;
            var pager = o.find(".grid-pager");
            var pagerHeight = pager.length > 0 ? pager.height() : 0;
            var xscrollbar = o.find(".x-scrollbar");
            var yscrollbar = o.find(".y-scrollbar");
            var container = o.find(".grid-container");
            var tblContent = o.find("#" + options.tblContentId);

            var freeCellTop = o.find(".free-top-right");
            var freeCellBottom = o.find(".free-bottom-right");

            var colHeaderContainer = o.find(".col_header");
            var rowHeaderContainer = o.find(".row_header");
            var rowHeaderWarpper = o.find(".row_header_wrapper");
            var colFooterContainer = o.find(".col_footer");
            var rowFooterContainer = o.find(".row_footer");
            var isShowFooter = !colFooterContainer.hasClass('display-none') && colFooterContainer.find("thead tr").length > 0;

            var numOfFreezeCol = options.numberFreezeCol;
            var col = options.column.length,
                i;
            if (col > 0) {
                options.gridWidth = 0;
                options.freezeWidth = 0;
                for (i = 0; i < col; i++) {
                    var colw = options.column[i];
                    options.gridWidth += colw;
                    if (i < numOfFreezeCol) {
                        options.freezeWidth += colw;
                    }
                }
                options.gridWidth += (options.padding + 1 + options.padding) * col;
                options.freezeWidth += (numOfFreezeCol - 1) * options.padding * 2 + numOfFreezeCol - 1;
            }

            if (tblContent.height() === 0 && options.fullHeight === false) {
                tblContent.height(options.gridHeight);
            }

            var tblWidth = Math.max(tblContent.width(), options.gridWidth);
            var freezeWidth = options.freezeWidth;

            var parentWidth = o.parent().width();
            var browserWindowWidth = $(window).width();
            if (parentWidth > browserWindowWidth) {
                parentWidth = browserWindowWidth;
            }

            var gridWidth = options.fullWidth ? ((parentWidth < options.minWidth ? options.minWidth : parentWidth) - options.marginRight) : options.gridWidth;
            var isScrollX = gridWidth < tblWidth + 17;
            gridWidth = isScrollX ? gridWidth : (tblWidth + bar);

            o.css("width", gridWidth + "px");
            o.find(".content-container").css("width", tblWidth + "px");
            o.find(".col_header_content").css("width", tblWidth + "px");
            //container.css("width", gridWidth - 100 + "px");
            container.css("width", gridWidth + "px");
            xscrollbar.css("width", (gridWidth - freezeWidth - bar - (options.padding) * 2 - 1) + "px");
            colHeaderContainer.css("width", gridWidth + "px");
            colHeaderContainer.find(".header-container").css("width", tblWidth + "px");
            o.find(".footer-container").css("width", tblWidth + "px");
            o.find(".scrollbar-x-content").css("width", (tblWidth - freezeWidth - (isOnChromeBrowser ? 5 : 0)) + "px");
            o.find(".free-cell-bottom-left").css("width", freezeWidth + "px");
            pager.css("width", gridWidth + "px");
            rowHeaderContainer.css({
                "width": (freezeWidth + 5 + 17 + options.highlightFrozenWidth) + "px"
            });
            //rowHeaderContainer.css({
            //    "width": (freezeWidth + 5 + 17 + options.highlightFrozenWidth - 70) + "px"
            //});
            rowHeaderWarpper.css({
                "width": (freezeWidth + 5 + options.highlightFrozenWidth) + "px"
            });

            var tblHeight = tblContent.height();
            var freezeHeight = tblContent.find("thead").height();
            //var freezeFooterHeight = isShowFooter ? colFooterContainer.find("thead").height() + 3 : 0;
            var freezeFooterHeight = isShowFooter ? colFooterContainer.find("thead").height() + options.bottomBorderWidth : 0;
            var gridHeight = options.fullHeight ? ($(window).height() - options.gridHeight) : options.gridHeight;

            var isScrollY = gridHeight - titleHeight - pagerHeight - bar - freezeFooterHeight < tblHeight;
            if (isScrollY && gridHeight < options.minHeight) {
                gridHeight = options.minHeight;
            }
            gridHeight = isScrollY ? gridHeight : tblHeight + titleHeight + pagerHeight + bar + freezeFooterHeight;

            if (options.hideYBackGroundScroller) {
                o.css("width", (gridWidth - (!isScrollY && !isScrollX ? 18 : 0)) + "px");
                pager.css("width", (gridWidth - ((!isScrollY && !isScrollX ? 18 : 0) ? 18 : 0)) + "px");
            }

            var containerHeight = gridHeight - titleHeight - freezeFooterHeight - pagerHeight - bar + 1 - extraPosition;
            o.css("height", (gridHeight - 1 - (isScrollX ? -1 : bar)) + "px");
            rowHeaderContainer.css("height", containerHeight + "px");
            container.css("height", containerHeight + "px");
            rowHeaderWarpper.css("height", containerHeight + "px");
            yscrollbar.css("height", (containerHeight - freezeHeight - (ub.chrome || ub.msie ? 1 : 0) - options.heightScrollYAdd) + "px");
            var freeCellTopRightHeight = freezeHeight - ((ub.msie || ub.chrome || ff16) ? (options.padding * 2 + 1) : 0) + (ub.chrome ? 1 : 0);
            o.find(".free-cell-top-right").css("height", freeCellTopRightHeight + "px");

            if (isShowFooter) {
                o.find(".free-cell-bottom-right").css("height", (freezeFooterHeight + (isScrollX ? bar : 0) + 1) + "px");
            }
            freeCellBottom.css("top", (gridHeight - pagerHeight - freezeFooterHeight - bar) + "px");
            if (me.Options && me.Options.shouldVirtualScroller) {
                o.find(".scrollbar-y-content").css("height", me.Options.totalHeight + "px");
            }
            else {
                o.find(".scrollbar-y-content").css("height", (tblHeight - freezeHeight) + "px");
            }

            //// Fixed bug for IE10, wrong body Height
            var newTblHeight = tblContent.height();
            if (ub.msie && tblHeight !== newTblHeight) {
                tblHeight = newTblHeight;
                if (me.Options && me.Options.shouldVirtualScroller) {
                    o.find(".scrollbar-y-content").css("height", me.Options.totalHeight + "px");
                }
                else {
                    o.find(".scrollbar-y-content").css("height", (tblHeight - freezeHeight) + "px");
                }
            }

            yscrollbar.css("top", (freezeHeight + titleHeight -
                                    (ub.chrome ? (colHeaderContainer.find("thead tr").length - 2) : 0) +
                                    options.heightScrollYAdd) + "px");
            xscrollbar.css("top", (gridHeight - pagerHeight - bar - 1 + extraPosition) + "px");
            if (numOfFreezeCol > 0) {
                o.find(".free-bottom-left").css("top", (gridHeight - pagerHeight - bar - 1 + extraPosition) + "px");
            }
            else {
                o.find(".free-bottom-left").remove();
            }

            rowFooterContainer.css({
                "top": (gridHeight - pagerHeight - freezeFooterHeight - bar - options.bottomBorderWidth - extraPosition) + "px",
                "border-top-width": options.bottomBorderWidth + "px"
            });
            colFooterContainer.css({
                "width": gridWidth + "px",
                "top": (gridHeight - pagerHeight - freezeFooterHeight - bar - options.bottomBorderWidth - extraPosition) + "px",
                "border-top-width": options.bottomBorderWidth + "px"
            });
            //pager.css("top", (gridHeight - pagerHeight - (isScrollX ? -(isShowFooter ? 0 : 1) : bar) - (isShowFooter ? 1 : 1)) + "px");
            pager.css("top", (gridHeight - pagerHeight - (isScrollX ? -(isShowFooter ? 0 : 1) : bar) - 2 + extraPosition * 2) + "px");

            xscrollbar.css("left", (freezeWidth + (options.padding) * 2 + 1 - extraPosition) + "px");
            yscrollbar.css("left", (gridWidth - bar - 1 + extraPosition) + "px");
            freeCellTop.css("left", (gridWidth - bar - 1 + extraPosition) + "px");
            freeCellBottom.css("left", (gridWidth - bar - 1) + "px");

            if (isScrollY) {
                yscrollbar.css("overflow-y", "scroll");
            }
            else {
                yscrollbar.css("overflow-y", "hidden");
            }

            if (isScrollX) {
                xscrollbar.css("overflow-x", "scroll");
            }
            else {
                xscrollbar.css("overflow-x", "hidden");
            }

            ////Recalculate Title Width
            var gridTitle = o.parent().find(".grid-title");
            if (gridTitle.length > 0) {
                gridTitle.width(o.width());
            }
            //// Fixbug show scroller on row header on chrome
            if (isOnChromeBrowser) {
                rowHeaderContainer.css("overflow", "hidden !important");
            }

            ////fix bug scoll in IE
            if (ub.msie) {
                yscrollbar.addClass("y-scrollbar-fix-width");
                xscrollbar.addClass("x-scrollbar-fix-height");
            }

            if (!isParentShow) {
                o.parent().css({ position: "", visibility: "", display: "" });
            }
        },
        RegisterScrollerEvents: function (obj, container, rowheader, xscrollbar, yscrollbar) {
            var me = this;
            me.preLeft = xscrollbar.scrollLeft();
            me.preTop = yscrollbar.scrollTop();
            me.preTouchX = 0;
            me.preTouchY = 0;
            me.colHeaderContainer = obj.find(".col_header");
            me.rowHeaderContainer = obj.find(".row_header");
            me.colFooterContainer = obj.find(".col_footer");

            var fanexGridOption = me.Options ? me.Options : {};
            var yscrollbarHeight = yscrollbar.height();
            var yscrollbarHalfHeight = parseInt(yscrollbarHeight / 2);
            var containerHalfHeight = parseInt(container.height() / 2);
            var bodyRowHeight = fanexGridOption.bodyRowHeight;
            var maxLength = fanexGridOption.realLength;
            var isShowFrozenCol = fanexGridOption.numberOfFrozenColumn > 0;
            var tbodyContainer = container.find('tbody');
            var tbodyRowHeader = rowheader.find('tbody');
            me.containerPreTop = 0;
            me.rowHeaderPreTop = 0;
            me.yScrollPreTop = 0;

            yscrollbar
                .unbind('scroll.' + me.gridId)
                .bind('scroll.' + me.gridId, function () {
                me.alreadyYScroll = true;
                var top = yscrollbar.scrollTop();
                if (fanexGridOption.shouldVirtualScroller) {
                    if (top !== me.yScrollPreTop) {
                        if (me.renderVirtualTimeoutId) {
                            clearTimeout(me.renderVirtualTimeoutId);
                        }

                        me.renderVirtualTimeoutId = setTimeout(function () {
                            var currentScrollIndex = parseInt((top + yscrollbarHalfHeight) / bodyRowHeight);
                            var rowNeedRenderStartIndex = Math.max(0, currentScrollIndex - fanexGridOption.virtualThresholdNumber);
                            var rowNeedRenderEndIndex = Math.min(maxLength, currentScrollIndex + fanexGridOption.virtualThresholdNumber);
                            var addRowsLength = fanexGridOption.virtualThresholdNumber;
                            var numOfVirtualRows = fanexGridOption.numOfVirtualRows;

                            var addRowsHtml = '';
                            var addFrozenRowsHtml = '';
                            var fromIndex = 0;
                            var toIndex = 0;
                            var isRemoveAllRows = false;
                            var gapMaxLength = 0;
                            var negativeRows = 0;
                            var i = 0;
                            var removeAllContainerScrollTop = false;
                            var removeRowsLength = 0;

                            var removeRows = false;
                            var removeHeight = false;

                            //var virtualTop = 0;
                            var scrollForwardWhenScrollTopIsMax = 0;
                            var newContainerScrollTop = 0;
                            var currentTop = 0;
                            if (fanexGridOption.rowStartIndex > 0
                                && rowNeedRenderStartIndex < fanexGridOption.rowStartIndex) {
                                //console.log('%c New Start Rows', 'background: #222; color: #bada55');
                                //// Add more start rows
                                toIndex = fanexGridOption.rowStartIndex;
                                var newStartIndex = Math.max(fanexGridOption.rowStartIndex - addRowsLength, 0);

                                if (rowNeedRenderStartIndex < newStartIndex) {
                                    newStartIndex = rowNeedRenderStartIndex;

                                    if (newStartIndex + numOfVirtualRows < fanexGridOption.rowStartIndex) {
                                        isRemoveAllRows = true;
                                        newStartIndex = currentScrollIndex - parseInt(numOfVirtualRows / 2);
                                        if (newStartIndex < 0) {
                                            negativeRows = newStartIndex;
                                        }
                                        newStartIndex = Math.max(newStartIndex, 0);
                                        fanexGridOption.rowEndIndex = toIndex = newStartIndex + numOfVirtualRows;
                                    }
                                }

                                //console.log('%cMain: newStartIndex: ' + newStartIndex + ',toIndex: ' + toIndex, 'background: #95BAC6');

                                for (i = newStartIndex; i < toIndex; i++) {
                                    addRowsHtml += fanexGridOption.allBodyRowsHtml[i];
                                    if (isShowFrozenCol) {
                                        addFrozenRowsHtml += fanexGridOption.allFrozenRowsHtml[i];
                                    }
                                }

                                if (isRemoveAllRows) {
                                    Fanex.Grid.Helper.KeepStateOfRows(fanexGridOption.allBodyRowsHtml, tbodyContainer.find('tr'));
                                    tbodyContainer.empty();
                                    if (isShowFrozenCol) {
                                        Fanex.Grid.Helper.KeepStateOfRows(fanexGridOption.allFrozenRowsHtml, tbodyRowHeader.find('tr'));
                                        tbodyRowHeader.empty();
                                    }
                                }

                                tbodyContainer.prepend(addRowsHtml);
                                if (isShowFrozenCol) {
                                    tbodyRowHeader.prepend(addFrozenRowsHtml);
                                }
                                fanexGridOption.rowStartIndex = newStartIndex;

                                currentTop = container.scrollTop();

                                if (isRemoveAllRows) {
                                    removeAllContainerScrollTop = parseInt((toIndex - newStartIndex) * bodyRowHeight / 2) - yscrollbarHalfHeight;
                                    if (negativeRows < 0) {
                                        removeAllContainerScrollTop += negativeRows * bodyRowHeight;
                                    }
                                    container.prop("scrollTop", removeAllContainerScrollTop);
                                    me.cPreTop = removeAllContainerScrollTop;
                                    if (isShowFrozenCol) {
                                        rowheader.prop("scrollTop", removeAllContainerScrollTop);
                                        me.rPreTop = removeAllContainerScrollTop;
                                    }
                                }
                                else {
                                    removeRowsLength = toIndex - newStartIndex;
                                    fanexGridOption.rowEndIndex = Math.max(fanexGridOption.rowEndIndex - removeRowsLength, 0);

                                    //console.log('%cMain: fanexGridOption.rowEndIndex: ' + fanexGridOption.rowEndIndex, 'background: #95BAC6');
                                    //// Remove end rows

                                    removeRows = tbodyContainer.find('tr:gt(' + Math.max(tbodyContainer.children().length - removeRowsLength - 1, 0) + ')');
                                    removeHeight = removeRowsLength * bodyRowHeight;
                                    Fanex.Grid.Helper.KeepStateOfRows(fanexGridOption.allBodyRowsHtml, removeRows);
                                    removeRows.remove();
                                    newContainerScrollTop = currentTop + removeHeight;
                                    me.containerPreTop = newContainerScrollTop;
                                    container.prop("scrollTop", newContainerScrollTop);
                                    if (isShowFrozenCol) {
                                        var removeFrozenRows = tbodyRowHeader.find('tr:gt('
                                            + Math.max(tbodyRowHeader.children().length - removeRowsLength - 1, 0)
                                            + ')');
                                        Fanex.Grid.Helper.KeepStateOfRows(fanexGridOption.allFrozenRowsHtml, removeFrozenRows);
                                        removeFrozenRows.remove();
                                        me.rowHeaderPreTop = newContainerScrollTop;
                                        rowheader.prop("scrollTop", newContainerScrollTop);
                                    }

                                    var containerContentHeight = container.find('.col_body_content').height();
                                    var containerHeight = container.height();
                                    if (newContainerScrollTop > (containerContentHeight - containerHeight)) {
                                        scrollForwardWhenScrollTopIsMax = newContainerScrollTop - (containerContentHeight - containerHeight);
                                    }
                                }
                            }

                            var scrollBackWhenTopZero = 0;
                            if (!isRemoveAllRows
                                && rowNeedRenderEndIndex > fanexGridOption.rowEndIndex) {
                                //console.log('%c New End Rows', 'background: #D9C5C1');
                                //http://stackoverflow.com/questions/15690541/meaning-of-window-scrolltop-document-height-window-height

                                //// Add more end rows
                                fromIndex = fanexGridOption.rowEndIndex;
                                var newEndIndex = Math.min(fanexGridOption.rowEndIndex + addRowsLength, maxLength);
                                isRemoveAllRows = false;

                                if (rowNeedRenderEndIndex > newEndIndex) {
                                    newEndIndex = rowNeedRenderEndIndex;

                                    if (newEndIndex - numOfVirtualRows > fanexGridOption.rowEndIndex) {
                                        isRemoveAllRows = true;
                                        newEndIndex = currentScrollIndex + parseInt(numOfVirtualRows / 2);
                                        if (maxLength < newEndIndex) {
                                            gapMaxLength = newEndIndex - maxLength;
                                        }
                                        newEndIndex = Math.min(newEndIndex, maxLength);
                                        fanexGridOption.rowStartIndex = fromIndex = newEndIndex - numOfVirtualRows;
                                    }
                                }

                                //console.log('%cMain: fromIndex: ' + fromIndex + ',newEndIndex: ' + newEndIndex, 'background: #D9C5C1');

                                for (i = fromIndex; i < newEndIndex; i++) {
                                    addRowsHtml += fanexGridOption.allBodyRowsHtml[i];
                                    if (isShowFrozenCol) {
                                        addFrozenRowsHtml += fanexGridOption.allFrozenRowsHtml[i];
                                    }
                                }

                                if (isRemoveAllRows) {
                                    Fanex.Grid.Helper.KeepStateOfRows(fanexGridOption.allBodyRowsHtml, tbodyContainer.find('tr'));
                                    tbodyContainer.empty();
                                    if (isShowFrozenCol) {
                                        Fanex.Grid.Helper.KeepStateOfRows(fanexGridOption.allFrozenRowsHtml, tbodyRowHeader.find('tr'));
                                        tbodyRowHeader.empty();
                                    }
                                }

                                tbodyContainer.append(addRowsHtml);
                                if (isShowFrozenCol) {
                                    tbodyRowHeader.append(addFrozenRowsHtml);
                                }
                                fanexGridOption.rowEndIndex = newEndIndex;

                                currentTop = container.scrollTop();
                                //// Remove start rows
                                if (isRemoveAllRows) {
                                    removeAllContainerScrollTop = parseInt((newEndIndex - fromIndex + 1) * bodyRowHeight / 2) - yscrollbarHalfHeight;
                                    if (gapMaxLength > 0) {
                                        //newEndIndex === maxLength
                                        //removeAllContainerScrollTop = parseInt((newEndIndex - fromIndex) * bodyRowHeight) - yscrollbarHeight;
                                        //console.log(gapMaxLength);
                                        removeAllContainerScrollTop += (gapMaxLength + 1) * bodyRowHeight;
                                    }

                                    container.prop("scrollTop", removeAllContainerScrollTop);
                                    me.cPreTop = removeAllContainerScrollTop;
                                    if (isShowFrozenCol) {
                                        rowheader.prop("scrollTop", removeAllContainerScrollTop);
                                        me.rPreTop = removeAllContainerScrollTop;
                                    }
                                }
                                else {
                                    removeRowsLength = newEndIndex - fromIndex;

                                    fanexGridOption.rowStartIndex += removeRowsLength;

                                    //console.log('%cMain: fanexGridOption.rowStartIndex: ' + fanexGridOption.rowStartIndex, 'background: #D9C5C1');

                                    removeRows = tbodyContainer.find('tr:lt(' + removeRowsLength + ')');
                                    removeHeight = removeRows.length * bodyRowHeight;
                                    Fanex.Grid.Helper.KeepStateOfRows(fanexGridOption.allBodyRowsHtml, removeRows);
                                    removeRows.remove();
                                    newContainerScrollTop = currentTop - removeHeight;
                                    me.containerPreTop = newContainerScrollTop;
                                    container.prop("scrollTop", newContainerScrollTop);

                                    if (isShowFrozenCol) {
                                        var removeRowsHeader = tbodyRowHeader.find('tr:lt(' + removeRowsLength + ')');
                                        Fanex.Grid.Helper.KeepStateOfRows(fanexGridOption.allFrozenRowsHtml, removeRowsHeader);
                                        removeRowsHeader.remove();
                                        me.rowHeaderPreTop = newContainerScrollTop;
                                        rowheader.prop("scrollTop", newContainerScrollTop);
                                    }

                                    if (newContainerScrollTop <= 0) {
                                        scrollBackWhenTopZero = Math.abs(newContainerScrollTop);
                                    }
                                }
                                //console.log('%cMain: fanexGridOption.rowEndIndex: ' + newEndIndex, 'background: #D9C5C1');
                            }

                            if (!isRemoveAllRows
                                && me.yScrollPreTop !== top) {
                                var containerOldTop = container.scrollTop();
                                if (typeof (me.yScrollPreTop) === 'undefined') {
                                    me.yScrollPreTop = 0;
                                }

                                var containerNewTop = containerOldTop + (top - me.yScrollPreTop) - scrollBackWhenTopZero + scrollForwardWhenScrollTopIsMax;
                                if (top === 0) {
                                    containerNewTop = 0;
                                }

                                if (yscrollbarHeight + top + 50 >= fanexGridOption.totalHeight) {
                                    //window.console.log('containerNewTop += 100');
                                    containerNewTop += bodyRowHeight * 4;
                                }
                                me.containerPreTop = containerNewTop;
                                if (isShowFrozenCol) {
                                    me.rowHeaderPreTop = containerNewTop;
                                }
                                container.prop("scrollTop", containerNewTop);
                                if (isShowFrozenCol) {
                                    me.rowHeaderContainer.prop("scrollTop", containerNewTop);
                                }
                            }
                            //me.preTop = top;
                            me.yScrollPreTop = top;
                            me.renderVirtualTimeoutId = null;
                        }, fanexGridOption.renderVirtualTimeout);
                    }
                }
                else {
                    if (me.preTop !== top) {
                        container.prop("scrollTop", top);
                        me.rowHeaderContainer.prop("scrollTop", top);
                        me.preTop = top;
                    }
                }
            });

            var touchMoveEventHandler = function (e) {
                var touch = e.originalEvent.touches[0];
                var pageX = touch.pageX;
                if (me.preTouchX == 0) {
                    me.preTouchX = touch.pageX;
                } else {
                    var touchLength = (pageX - me.preTouchX);
                    var left = me.preLeft - touchLength;
                    xscrollbar.prop("scrollLeft", left);
                    me.preTouchX = pageX;
                }
            };

            var touchEndEventHandler = function (e) {
                me.preTouchX = 0;
                me.preTouchY = 0;
            };

            container
                .unbind('scroll.' + me.gridId)
                .bind('scroll.' + me.gridId, function () {
                   
                var top = container.scrollTop();
                var yScrollTop = yscrollbar.prop("scrollTop") + (top - (me.cPreTop || 0));
                var isAddTop = false, isSubTop = false;
                if (fanexGridOption.shouldVirtualScroller) {
                    if (me.containerPreTop !== top) {
                        var maxLength = fanexGridOption.realLength;
                        var currentScrollIndex = parseInt((yScrollTop + containerHalfHeight) / bodyRowHeight);
                        var rowNeedRenderStartIndex = Math.max(0, currentScrollIndex - fanexGridOption.virtualThresholdNumber);
                        var rowNeedRenderEndIndex = Math.min(maxLength, currentScrollIndex + fanexGridOption.virtualThresholdNumber);
                        var addRowsLength = fanexGridOption.virtualThresholdNumber;
                        var addRowsHtml = '';
                        var addFrozenRowsHtml = '';
                        var fromIndex = 0;
                        var toIndex = 0;
                        var newContainerScrollTop = 0;
                        var currentTop = 0;
                        var i = 0;
                        var isRemoveAllRows = false;
                        var removeRows = false;
                        var removeHeight = false;
                        var removeRowsHeader = 0;
                        var removeRowsLength = 0;

                        if (fanexGridOption.rowStartIndex > 0
                            && rowNeedRenderStartIndex < fanexGridOption.rowStartIndex) {
                            toIndex = fanexGridOption.rowStartIndex;
                            var newStartIndex = Math.max(fanexGridOption.rowStartIndex - addRowsLength, 0);

                            if (rowNeedRenderStartIndex < newStartIndex) {
                                newStartIndex = rowNeedRenderStartIndex;
                            }

                            for (i = newStartIndex; i < toIndex; i++) {
                                addRowsHtml += fanexGridOption.allBodyRowsHtml[i];
                                if (isShowFrozenCol) {
                                    addFrozenRowsHtml += fanexGridOption.allFrozenRowsHtml[i];
                                }
                            }

                            tbodyContainer.prepend(addRowsHtml);
                            if (isShowFrozenCol) {
                                tbodyRowHeader.prepend(addFrozenRowsHtml);
                            }
                            fanexGridOption.rowStartIndex = newStartIndex;

                            currentTop = container.scrollTop();

                            removeRowsLength = toIndex - newStartIndex;
                            fanexGridOption.rowEndIndex = Math.max(fanexGridOption.rowEndIndex - removeRowsLength, 0);

                            removeRows = tbodyContainer.find('tr:gt(' + Math.max(tbodyContainer.children().length - removeRowsLength - 1, 0) + ')');
                            removeHeight = removeRowsLength * bodyRowHeight;
                            Fanex.Grid.Helper.KeepStateOfRows(fanexGridOption.allBodyRowsHtml, removeRows);
                            removeRows.remove();
                            newContainerScrollTop = currentTop + removeHeight;

                            me.cPreTop += removeHeight;
                            top += removeHeight;
                            me.containerPreTop = newContainerScrollTop;
                            container.prop("scrollTop", newContainerScrollTop);
                            isAddTop = true;

                            if (isShowFrozenCol) {
                                removeRowsHeader = tbodyRowHeader.find('tr:gt(' + Math.max(tbodyRowHeader.children().length - removeRowsLength - 1, 0)
                                    + ')');
                                Fanex.Grid.Helper.KeepStateOfRows(fanexGridOption.allFrozenRowsHtml, removeRowsHeader);
                                removeRowsHeader.remove();
                                me.rowHeaderPreTop = newContainerScrollTop;
                                rowheader.prop("scrollTop", newContainerScrollTop);
                                me.rPreTop = me.cPreTop;
                            }
                        }

                        if (rowNeedRenderEndIndex > fanexGridOption.rowEndIndex) {
                            //// Add more end rows
                            fromIndex = fanexGridOption.rowEndIndex;
                            var newEndIndex = Math.min(fanexGridOption.rowEndIndex + addRowsLength, maxLength);
                            isRemoveAllRows = false;

                            if (rowNeedRenderEndIndex > newEndIndex) {
                                newEndIndex = rowNeedRenderEndIndex;
                            }
                            if (newEndIndex > fanexGridOption.allBodyRowsHtml.length) {
                                return;
                            }
                            for (i = fromIndex; i < newEndIndex; i++) {
                                addRowsHtml += fanexGridOption.allBodyRowsHtml[i];
                                if (isShowFrozenCol) {
                                    addFrozenRowsHtml += fanexGridOption.allFrozenRowsHtml[i];
                                }
                            }

                            tbodyContainer.append(addRowsHtml);
                            if (isShowFrozenCol) {
                                tbodyRowHeader.append(addFrozenRowsHtml);
                            }
                            fanexGridOption.rowEndIndex = newEndIndex;

                            currentTop = container.scrollTop();

                            removeRowsLength = newEndIndex - fromIndex;

                            fanexGridOption.rowStartIndex += removeRowsLength;

                            removeRows = tbodyContainer.find('tr:lt(' + removeRowsLength + ')');
                            removeHeight = removeRows.length * bodyRowHeight;
                            Fanex.Grid.Helper.KeepStateOfRows(fanexGridOption.allBodyRowsHtml, removeRows);
                            removeRows.remove();

                            newContainerScrollTop = Math.max(0, currentTop - removeHeight);
                            me.cPreTop = Math.max(0, me.cPreTop - removeHeight);
                            top = Math.max(0, top - removeHeight);
                            me.containerPreTop = newContainerScrollTop;
                            container.prop("scrollTop", newContainerScrollTop);
                            isSubTop = true;

                            if (isShowFrozenCol) {
                                removeRowsHeader = tbodyRowHeader.find('tr:lt(' + removeRowsLength + ')');
                                Fanex.Grid.Helper.KeepStateOfRows(fanexGridOption.allFrozenRowsHtml, removeRowsHeader);
                                removeRowsHeader.remove();
                                me.rowHeaderPreTop = newContainerScrollTop;
                                rowheader.prop("scrollTop", newContainerScrollTop);
                                me.rPreTop = me.cPreTop;
                            }
                        }

                        if (me.cPreTop !== top) {
                            var yscrollbarOldTop = yscrollbar.scrollTop();
                            if (typeof (me.cPreTop) === 'undefined') {
                                me.cPreTop = 0;
                            }

                            var yscrollbarNewTop = yscrollbarOldTop + (top - me.cPreTop);
                            me.yScrollPreTop = yscrollbarNewTop;

                            yscrollbar.prop("scrollTop", yscrollbarNewTop);
                            //rowheader.prop("scrollTop", top);
                            //
                            if (!isSubTop && !isAddTop) {
                                me.rowHeaderPreTop = top;
                                rowheader.prop("scrollTop", top);
                            }
                        }

                        me.containerPreTop = top;
                    }
                    me.cPreTop = top;
                }
                else {
                    if (me.preTop !== top) {
                        me.rowHeaderContainer.prop("scrollTop", top);
                        yscrollbar.prop("scrollTop", top);
                        me.preTop = top;
                    }
                    //
                    if (isOnSmartPhoneBrowser) {
                        var left = container.scrollLeft();
                        if (me.preLeft !== left) {
                            me.colHeaderContainer.prop("scrollLeft", left);
                            me.colFooterContainer.prop("scrollLeft", left);
                            xscrollbar.prop("scrollLeft", left);
                            me.preLeft = left;
                        }
                    }
                }
                })
                .unbind('touchmove.' + me.gridId)
                .bind('touchmove.' + me.gridId, touchMoveEventHandler)
                .unbind('touchend.' + me.gridId)
                .bind('touchend.' + me.gridId, touchEndEventHandler);

            rowheader
                .unbind('scroll.' + me.gridId)
                .bind('scroll.' + me.gridId, function () {
                var top = rowheader.scrollTop();
                var yScrollTop = yscrollbar.prop("scrollTop") + (top - (me.rPreTop || 0));
                var isAddTop = false, isSubTop = false;
                if (fanexGridOption.shouldVirtualScroller) {
                    if (me.rowHeaderPreTop !== top) {
                        var maxLength = fanexGridOption.realLength;
                        var currentScrollIndex = parseInt((yScrollTop + containerHalfHeight) / bodyRowHeight);
                        var rowNeedRenderStartIndex = Math.max(0, currentScrollIndex - fanexGridOption.virtualThresholdNumber);
                        var rowNeedRenderEndIndex = Math.min(maxLength, currentScrollIndex + fanexGridOption.virtualThresholdNumber);
                        var addRowsLength = fanexGridOption.virtualThresholdNumber;
                        var addRowsHtml = '';
                        var addFrozenRowsHtml = '';
                        var fromIndex = 0;
                        var toIndex = 0;
                        var newContainerScrollTop = 0;
                        var currentTop = 0;
                        var i = 0;
                        var isRemoveAllRows = false;
                        var removeRows = false;
                        var removeHeight = false;
                        var removeRowsLength = 0;

                        if (fanexGridOption.rowStartIndex > 0
                            && rowNeedRenderStartIndex < fanexGridOption.rowStartIndex) {
                            toIndex = fanexGridOption.rowStartIndex;
                            var newStartIndex = Math.max(fanexGridOption.rowStartIndex - addRowsLength, 0);

                            if (rowNeedRenderStartIndex < newStartIndex) {
                                newStartIndex = rowNeedRenderStartIndex;
                            }

                            for (i = newStartIndex; i < toIndex; i++) {
                                addRowsHtml += fanexGridOption.allBodyRowsHtml[i];
                                addFrozenRowsHtml += fanexGridOption.allFrozenRowsHtml[i];
                            }

                            tbodyContainer.prepend(addRowsHtml);
                            tbodyRowHeader.prepend(addFrozenRowsHtml);
                            fanexGridOption.rowStartIndex = newStartIndex;

                            currentTop = rowheader.scrollTop();

                            removeRowsLength = toIndex - newStartIndex;
                            fanexGridOption.rowEndIndex = Math.max(fanexGridOption.rowEndIndex - removeRowsLength, 0);

                            var removeFrozenRows = tbodyRowHeader.find('tr:gt(' + Math.max(tbodyRowHeader.children().length - removeRowsLength - 1, 0) + ')');
                            removeHeight = removeRowsLength * bodyRowHeight;
                            removeFrozenRows.remove();
                            newContainerScrollTop = currentTop + removeHeight;
                            me.rPreTop += removeHeight;
                            top += removeHeight;
                            me.rowHeaderPreTop = newContainerScrollTop;
                            rowheader.prop("scrollTop", newContainerScrollTop);
                            isAddTop = true;

                            removeRows = tbodyContainer.find('tr:gt(' + Math.max(tbodyContainer.children().length - removeRowsLength - 1, 0) + ')');
                            removeRows.remove();
                            me.containerPreTop = newContainerScrollTop;
                            container.prop("scrollTop", newContainerScrollTop);
                            me.cPreTop += removeHeight;
                        }

                        if (rowNeedRenderEndIndex > fanexGridOption.rowEndIndex) {
                            //// Add more end rows
                            fromIndex = fanexGridOption.rowEndIndex;
                            var newEndIndex = Math.min(fanexGridOption.rowEndIndex + addRowsLength, maxLength);
                            isRemoveAllRows = false;

                            if (rowNeedRenderEndIndex > newEndIndex) {
                                newEndIndex = rowNeedRenderEndIndex;
                            }

                            if (newEndIndex > fanexGridOption.allBodyRowsHtml.length) {
                                return;
                            }

                            for (i = fromIndex; i < newEndIndex; i++) {
                                addRowsHtml += fanexGridOption.allBodyRowsHtml[i];
                                addFrozenRowsHtml += fanexGridOption.allFrozenRowsHtml[i];
                            }

                            tbodyContainer.append(addRowsHtml);
                            tbodyRowHeader.append(addFrozenRowsHtml);
                            fanexGridOption.rowEndIndex = newEndIndex;

                            currentTop = rowheader.scrollTop();

                            removeRowsLength = newEndIndex - fromIndex;

                            fanexGridOption.rowStartIndex += removeRowsLength;

                            removeRows = tbodyRowHeader.find('tr:lt(' + removeRowsLength + ')');
                            removeHeight = removeRows.length * bodyRowHeight;
                            removeRows.remove();

                            newContainerScrollTop = Math.max(0, currentTop - removeHeight);
                            me.rPreTop = Math.max(0, me.rPreTop - removeHeight);
                            top = Math.max(0, top - removeHeight);
                            me.rowHeaderPreTop = newContainerScrollTop;
                            rowheader.prop("scrollTop", newContainerScrollTop);
                            isSubTop = true;

                            removeRows = tbodyContainer.find('tr:lt(' + removeRowsLength + ')');
                            removeRows.remove();
                            me.containerPreTop = newContainerScrollTop;
                            container.prop("scrollTop", newContainerScrollTop);
                            me.cPreTop += removeHeight;
                        }

                        if (me.rPreTop !== top) {
                            var yscrollbarOldTop = yscrollbar.scrollTop();
                            if (typeof (me.rPreTop) === 'undefined') {
                                me.rPreTop = 0;
                            }

                            var yscrollbarNewTop = yscrollbarOldTop + (top - me.rPreTop);
                            me.yScrollPreTop = yscrollbarNewTop;

                            yscrollbar.prop("scrollTop", yscrollbarNewTop);

                            if (!isSubTop && !isAddTop) {
                                me.containerPreTop = top;
                                container.prop("scrollTop", top);
                            }
                            //container.prop("scrollTop", yscrollbarNewTop);
                        }

                        me.rowHeaderPreTop = top;
                    }
                    me.rPreTop = top;
                }
                else {
                    if (me.preTop !== top) {
                        container.prop("scrollTop", top);
                        yscrollbar.prop("scrollTop", top);
                        me.preTop = top;
                    }
                }
            });

            xscrollbar
                .unbind('scroll.' + me.gridId)
                .bind('scroll.' + me.gridId, function (e) {
                var left = xscrollbar.scrollLeft();
                if (me.preLeft !== left) {
                    me.colHeaderContainer.prop("scrollLeft", left);
                    me.colFooterContainer.prop("scrollLeft", left);
                    container.prop("scrollLeft", left);
                    me.preLeft = left;
                }
                });

            if (isOnSmartPhoneBrowser) {
                me.colHeaderContainer.scroll(function () {
                    var left = me.colHeaderContainer.scrollLeft();
                    if (me.preLeft !== left) {
                        container.prop("scrollLeft", left);
                        me.colFooterContainer.prop("scrollLeft", left);
                        xscrollbar.prop("scrollLeft", left);
                        me.preLeft = left;
                    }
                }).unbind('touchmove.' + me.gridId)
                    .bind('touchmove.' + me.gridId, touchMoveEventHandler)
                    .unbind('touchend.' + me.gridId)
                    .bind('touchend.' + me.gridId, touchEndEventHandler);

                me.colFooterContainer.scroll(function () {
                    var left = me.colFooterContainer.scrollLeft();
                    if (me.preLeft !== left) {
                        container.prop("scrollLeft", left);
                        me.colHeaderContainer.prop("scrollLeft", left);
                        xscrollbar.prop("scrollLeft", left);
                        me.preLeft = left;
                    }
                }).unbind('touchmove.' + me.gridId)
                    .bind('touchmove.' + me.gridId, touchMoveEventHandler)
                    .unbind('touchend.' + me.gridId)
                    .bind('touchend.' + me.gridId, touchEndEventHandler);
            }
        }
    };

    //// Grid Helper
    Fanex.Grid.Helper =
    {
        DetectBrowser: function () {
            //// Grid Scroller
            var matchedBrowser, browserDectect;

            // Use of jQuery.browser is frowned upon.
            // More details: http://api.jquery.com/jQuery.browser
            // jQuery.uaMatch maintained for back-compat
            function ubMatch(ua) {
                ua = ua.toLowerCase();

                var match = /(chrome)[ \/]([\w.]+)/.exec(ua) ||
                /(webkit)[ \/]([\w.]+)/.exec(ua) ||
                /(opera)(?:.*version|)[ \/]([\w.]+)/.exec(ua) ||
                /(trident)(?:.*rv:([\w.]+))?/.exec(ua) ||
                /(msie) ([\w.]+)/.exec(ua) ||
                ua.indexOf("compatible") < 0 && /(mozilla)(?:.*? rv:([\w.]+)|)/.exec(ua) ||
                [];

                return {
                    browserDectect: match[1] || "",
                    version: match[2] || "0"
                };
            }

            matchedBrowser = ubMatch(navigator.userAgent);
            browserDectect = {};

            if (matchedBrowser.browserDectect) {
                browserDectect[matchedBrowser.browserDectect] = true;
                browserDectect.version = matchedBrowser.version;

                if (matchedBrowser.browserDectect === 'trident') {
                    browserDectect.msie = true;
                }
            }

            // Chrome is Webkit, but Webkit is also Safari.
            if (browserDectect.chrome) {
                browserDectect.webkit = true;
            } else if (browserDectect.webkit) {
                browserDectect.safari = true;
            }

            return browserDectect;
        },
        RemoveItemForRowSpan: function (rows, currentRowIndex, mainColumn) {
            var rowSpanSize = mainColumn.MergeRowNumber;
            var columnName = mainColumn.Name;
            var currentLength = currentRowIndex + rowSpanSize;
            var rowsLength = currentLength < rows.length ? currentLength : rows.length;
            for (var rowIndex = currentRowIndex + 1; rowIndex < rowsLength; rowIndex++) {
                for (var colIndex = 0; colIndex < rows[rowIndex].Columns.length; colIndex++) {
                    var currentCol = rows[rowIndex].Columns[colIndex];
                    if (currentCol.Name === columnName) {
                        if (isValid(currentCol.Sort) && currentCol.Sort === true) {
                            var firstColIndex = Fanex.Grid.Helper.GetColumnIndexFromColumnName(rows[currentRowIndex], currentCol.Name);
                            rows[currentRowIndex].Columns[firstColIndex].Sort = true;
                            rows[currentRowIndex].Columns[firstColIndex].SortDirection = currentCol.SortDirection;
                        }

                        if (isValid(mainColumn.MergeColNumber)) {
                            var mergeColLength = mainColumn.MergeColNumber + colIndex;
                            mergeColLength = (mergeColLength <= rows[rowIndex].Columns.length ? mergeColLength : 0);
                            for (var mergeColIndex = mergeColLength; mergeColIndex >= colIndex; mergeColIndex--) {
                                rows[rowIndex].Columns.splice(mergeColIndex, 1);
                                //rows[rowIndex].Columns.remove(mergeColIndex);
                            }
                        }
                        rows[rowIndex].Columns.splice(colIndex, 1);
                    }
                }
            }
        },
        GetColumnIndexFromColumnName: function (row, columnName) {
            for (var index = 0; index < row.Columns.length; index++) {
                if (row.Columns[index].Name === columnName) {
                    return index;
                }
            }
            return -1;
        },
        CreateRowsForFrozenColumn: function (mainRows, frozenColNumber, frozenColsName) {
            var frozenRows = [];
            var rowLength = mainRows.length;

            for (var rowIndex = 0; rowIndex < rowLength; rowIndex++) {
                var currentRow = mainRows[rowIndex];
                var newRow = clone(currentRow);
                newRow.Columns = [];
                var isset = false;
                var colsLength = currentRow.Columns.length < frozenColNumber ? currentRow.Columns.length : frozenColNumber;

                for (var colIndex = 0; colIndex < colsLength; colIndex++) {
                    var currentCol = currentRow.Columns[colIndex];
                    if (Fanex.Grid.Helper.CheckExistName(frozenColsName, currentCol.Name)) {
                        newRow.Columns.push(clone(currentRow.Columns[colIndex]));
                        isset = true;
                    }
                }
                if (isset) {
                    frozenRows.push(newRow);
                }
            }

            return frozenRows;
        },
        CheckExistName: function (frozenColsName, name) {
            for (var colIndex = 0; colIndex < frozenColsName.length; colIndex++) {
                if (frozenColsName[colIndex] === name) {
                    return true;
                }
            }
            return false;
        },
        CheckHideColSpan: function (currentFrozenColums) {
            var colsLength = currentFrozenColums.length;
            for (var colIndex = 0; colIndex < colsLength; colIndex++) {
                if (!isValid(currentFrozenColums[colIndex].MergeRowNumber)) {
                    return false;
                }
            }
            return true;
        },
        GetArrayWidth: function (columns, enableHiddenColumns, isForComputingWidth) {
            var arrayWidth = [];
            var columnLength = columns.length;
            for (var columnIndex = 0; columnIndex < columnLength; columnIndex++) {
                var currentColumn = columns[columnIndex];
                if (isValid(enableHiddenColumns) &&
                    enableHiddenColumns === false &&
                    currentColumn.CanShowHide &&
                    currentColumn.CanShowHide === true &&
                    isForComputingWidth === false) {
                    continue;
                }
                else {
                    arrayWidth.push(currentColumn.Width);
                }
            }
            return arrayWidth;
        },
        GetColMergeWidth: function (arrayWidth, startIndex, colMergeNumber, enableHiddenColumns, columns) {
            var colWidth = 0;
            var finalColMergeNumber = colMergeNumber;
            var colLength = arrayWidth.length < (startIndex + colMergeNumber) ? arrayWidth.length : (startIndex + colMergeNumber);
            for (var colIndex = startIndex; colIndex < colLength; colIndex++) {
                if (isValid(enableHiddenColumns) && enableHiddenColumns === false && columns[colIndex].CanShowHide && columns[colIndex].CanShowHide === true) {
                    finalColMergeNumber--;
                }
                else {
                    colWidth += arrayWidth[colIndex];
                }
            }
            colWidth += ((finalColMergeNumber - 1) * 5);
            return colWidth;
        },
        GetColumnWidth: function (columns, name) {
            var colLength = columns.length;
            for (var colIndex = 0; colIndex < colLength; colIndex++) {
                if (columns[colIndex].Name === name) {
                    return columns[colIndex].Width;
                }
            }
            return 0;
        },
        GetColumnsHastable: function (columns) {
            var hastable = {};
            var colLength = columns.length;
            for (var colIndex = 0; colIndex < colLength; colIndex++) {
                var currentCol = columns[colIndex];
                hastable[currentCol.Name] = currentCol;
                hastable[currentCol.Name].ColumnIndex = colIndex;
            }
            return hastable;
        },
        SortByNumber: function (col1Value, col2Value, sortDirection) {
            var isNumber1 = !isNaN(col1Value);
            var isNumber2 = !isNaN(col2Value);

            if (isNumber1 && isNumber2) {
                var parseValCol1 = parseFloat(col1Value);
                var parseValCol2 = parseFloat(col2Value);
                if (sortDirection === 'asc') {
                    return parseValCol1 > parseValCol2 ? 1 : -1;
                }
                else {
                    return parseValCol1 < parseValCol2 ? 1 : -1;
                }
            }

            if (sortDirection === 'asc') {
                if (!isNumber1) {
                    return 1;
                }
            }
            else {
                if (!isNumber2) {
                    return 1;
                }
            }

            return -1;
        },
        GetSortedArray: function (rows, sortField, sortDirection, customSortFunction, sortType) {
            var sortColIndex = null;
            if (rows.length > 0) {
                var firstRow = rows[0];
                var colLength = firstRow.Columns.length;

                for (var colIndex = 0; colIndex < colLength; colIndex++) {
                    if (firstRow.Columns[colIndex].Name === sortField) {
                        sortColIndex = colIndex;
                        break;
                    }
                }
            }

            if (sortType != null) {
                sortType = sortType.toLowerCase();
            }

            sortDirection = sortDirection.toLowerCase();

            if (sortDirection === 'asc') {
                return rows.sort(function (row1, row2) {
                    return Fanex.Grid.Helper.CompareRow(sortColIndex, row1, row2, sortField, sortType, function (col1Value, col2Value) {
                        if (typeof (customSortFunction) === 'function') {
                            return customSortFunction(col1Value, col2Value, sortDirection);
                        }

                        if (sortType != null) {
                            switch (sortType) {
                                case 'number':
                                    return Fanex.Grid.Helper.SortByNumber(col1Value, col2Value, sortDirection);
                                case 'string':
                                    break;
                            }
                        }

                        return col1Value > col2Value ? 1 : -1;
                    });
                });
            }
            else {
                return rows.sort(function (row1, row2) {
                    return Fanex.Grid.Helper.CompareRow(sortColIndex, row1, row2, sortField, sortType, function (col1Value, col2Value) {
                        if (typeof (customSortFunction) === 'function') {
                            return customSortFunction(col1Value, col2Value, sortDirection);
                        }

                        if (sortType != null) {
                            switch (sortType) {
                                case 'number':
                                    return Fanex.Grid.Helper.SortByNumber(col1Value, col2Value, sortDirection);
                                case 'string':
                                    break;
                            }
                        }

                        return col1Value < col2Value ? 1 : -1;
                    });
                });
            }
        },
        CompareRow: function (sortColIndex, row1, row2, sortField, sortType, compareFunction) {
            //            var col1Value = row1.Columns[sortColIndex].Value;
            //            var col2Value = row2.Columns[sortColIndex].Value;
            //            if (sortType != null) {
            //                switch (sortType) {
            //                    case 'number':
            //
            //                }
            //            }

            return compareFunction(row1.Columns[sortColIndex].Value, row2.Columns[sortColIndex].Value);
        },
        SetColumnDisplay: function (columns, showHideArray) {
            var columnsLengh = columns.length;
            for (var colIndex = 0; colIndex < columnsLengh; colIndex++) {
                columns[colIndex].CanShowHide = false;
            }
            var showhidelength = showHideArray.length;
            for (var index = 0; index < showhidelength; index++) {
                columns[showHideArray[index]].CanShowHide = true;
            }
            return columns;
        },
        ComputeNewFrozenColumn: function (showhideArray, frozenColumnNumber) {
            var currentFrozenNumber = frozenColumnNumber;
            for (var index = 0; index < showhideArray.length; index++) {
                if (showhideArray[index] < currentFrozenNumber) {
                    frozenColumnNumber--;
                }
            }
            return frozenColumnNumber;
        },
        HideHeaderGroupWhenShowHide: function (hideColumns, startIndex, mergeColNumber) {
            var length = startIndex + mergeColNumber;
            for (var index = startIndex; index < length; index++) {
                if (!Fanex.Grid.Helper.ArrayContains(hideColumns, index)) {
                    return false;
                }
            }
            return true;
        },
        ReComputeMergeColumAfterShowHide: function (hideColumns, startIndex, mergeColNumber) {
            var currentMergeCol = mergeColNumber;
            var length = startIndex + mergeColNumber;
            for (var index = startIndex; index < length; index++) {
                if (Fanex.Grid.Helper.ArrayContains(hideColumns, index)) {
                    currentMergeCol--;
                }
            }
            return currentMergeCol;
        },
        UpdateSortInforAfterShowHideColumns: function (options, sortOption) {
            if (isValid(sortOption)) {
                var colsLength = options.columns.length;
                var customSortFunction = null;
                for (var colIndex = 0; colIndex < colsLength; colIndex++) {
                    if (options.columns[colIndex].Name === sortOption.SortColumnName) {
                        options.columns[colIndex].SortDirection = sortOption.SortDirection.toLowerCase();
                        customSortFunction = options.columns[colIndex].CustomSortFunction;
                    }
                }
                options.bodyRows = Fanex.Grid.Helper.GetSortedArray(options.bodyRows, sortOption.SortColumnName, sortOption.SortDirection, customSortFunction);
            }
        },
        RemoveSpaceIE9: function (html) {
            if (ub.msie && parseInt(ub.version, 10) === 9 && html !== undefined) {
                var expr = new RegExp('>[ \t\r\n\v\f]*<', 'g');
                html = html.replace(expr, '><');
            }

            return html;
        },
        CreateHeaderColumnsFromOptionsColumn: function (columns) {
            var colLength = columns.length;
            var colArray = [];
            for (var colIndex = 0; colIndex < colLength; colIndex++) {
                var currentCol = clone(columns[colIndex]);
                var currentWidth = currentCol.Width;
                if (isValid(currentCol.MergeHeaderColNumber) && currentCol.MergeHeaderColNumber > 0) {
                    var currentMaxLength = currentCol.MergeHeaderColNumber + colIndex;
                    colIndex++;
                    for (; colIndex < currentMaxLength && colIndex < colLength; colIndex++) {
                        currentWidth += columns[colIndex].Width + 5;
                    }
                    colIndex--;
                }
                currentCol.Width = currentWidth;
                colArray.push(currentCol);
            }
            return colArray;
        },
        ArrayContains: function (array, obj) {
            var i = array.length;
            while (i--) {
                if (array[i] === obj) {
                    return true;
                }
            }
            return false;
        },
        ComputeResizeColumn: function (mainContainer, options) {
            var resizeObject = {},
                columnHashtable = Fanex.Grid.Helper.GetColumnsHastable(options.columns),
                pixelsPerCharacter = options.pixelsPerCharacter;

            resizeObject = Fanex.Grid.Helper.ComputeMaxWidthInRows(options.bodyRows, resizeObject, columnHashtable, pixelsPerCharacter);

            if (options.showFooter) {
                resizeObject = Fanex.Grid.Helper.ComputeMaxWidthInRows(options.footerRows, resizeObject, columnHashtable, pixelsPerCharacter);
            }

            var mainColLength = options.columns.length;
            for (var mainColIndex = 0; mainColIndex < mainColLength; mainColIndex++) {
                var currentResizeObject = resizeObject[options.columns[mainColIndex].Name];
                if (currentResizeObject != null) {
                    if (currentResizeObject.calculated || options.calTextWidthWithFontSize) {
                        options.columns[mainColIndex].Width = resizeObject[options.columns[mainColIndex].Name].length;
                    }
                    else {
                        options.columns[mainColIndex].Width = Fanex.Grid.Helper.ComputeTextWidth(mainContainer, currentResizeObject.rawData) + 20;
                    }
                }
            }

            options = Fanex.Grid.Helper.AutoAddWidthForFullScreen(options, mainContainer);

            return options;
        },
        AutoAddWidthForFullScreen: function (options, mainContainer) {
            var currentContainerWidth = mainContainer.parent().width(),
                totalCalulatedWidth = 0,
                colLength = options.columns.length,
                resizeCols = [];

            for (var i = 0; i < colLength; i++) {
                var currentCol = options.columns[i];
                if (currentCol.CanResize === true) {
                    if (typeof (currentCol.MaxResizeWidth) !== 'undefined' && currentCol.Width > currentCol.MaxResizeWidth) {
                        currentCol.Width = currentCol.MaxResizeWidth;
                    }

                    resizeCols.push({
                        colIndex: i,
                        colName: currentCol.Name,
                        colWidth: currentCol.Width
                    });
                }
                totalCalulatedWidth += currentCol.Width;
            }

            //// Add width for others columns
            totalCalulatedWidth += (options.columns.length - 1) * 5 + options.marginRight + (options.hideYBackGroundScroller ? 0 : options.scrollWidth);
            if (resizeCols.length > 0 && currentContainerWidth > totalCalulatedWidth) {
                var totalAddWidth = currentContainerWidth - totalCalulatedWidth,
                    addRowForEachColumn = totalAddWidth / resizeCols.length;

                for (i = 0; i < resizeCols.length; i++) {
                    var currentResizeCol = resizeCols[i];
                    options.columns[currentResizeCol.colIndex].Width = currentResizeCol.colWidth + addRowForEachColumn;
                }
            }

            return options;
        },
        ComputeMaxWidthInRows: function (rows, resizeObject, columnHashtable, pixelsPerCharacter) {
            var rowLength = rows.length;
            //currentPixelsPerChar = 0;

            for (var rowIndex = 0; rowIndex < rowLength; rowIndex++) {
                var currentRow = rows[rowIndex],
                    colLength = currentRow.Columns.length;
                for (var colIndex = 0; colIndex < colLength; colIndex++) {
                    var currentCol = currentRow.Columns[colIndex];
                    if (columnHashtable[currentCol.Name].CanResize === true) {
                        var textLength = 0;
                        var calculated = true;
                        if (typeof (resizeObject[currentCol.Name]) === 'undefined') {
                            resizeObject[currentCol.Name] = {
                                length: columnHashtable[currentCol.Name].Width,
                                calculated: true,
                                rawData: ''
                            };
                        }

                        if (columnHashtable[currentCol.Name].CalResizeFunction != null) {
                            textLength = columnHashtable[currentCol.Name].CalResizeFunction(currentCol.Value);
                        }
                        else {
                            if (columnHashtable[currentCol.Name].PixelsPerCharacter != null) {
                                if (currentCol.Value != null) {
                                    textLength = currentCol.Value.toString().length * columnHashtable[currentCol.Name].PixelsPerCharacter;
                                }
                            }
                            else {
                                //currentpixelsPerChar = Fanex.Grid.;
                                if (currentCol.Value != null) {
                                    textLength = currentCol.Value.toString().length * pixelsPerCharacter;
                                }
                                calculated = false;
                            }

                            ////resizeResult = currentCol.Value.toString().length * currentpixelsPerChar;
                        }

                        if (resizeObject[currentCol.Name].length < textLength) {
                            resizeObject[currentCol.Name].length = textLength;
                            if (calculated === false) {
                                resizeObject[currentCol.Name].calculated = calculated;
                                if (currentCol.Value != null) {
                                    resizeObject[currentCol.Name].rawData = currentCol.Value.toString();
                                }
                            }
                        }
                    }
                }
            }

            return resizeObject;
        },
        ComputeTextWidth: function (mainContainer, text) {
            var tempElement = $('<div>' + text + '</div>')
            .css({
                'position': 'absolute',
                'float': 'left',
                'white-space': 'nowrap',
                'visibility': 'hidden'
            }).appendTo(mainContainer);
            var textWidth = tempElement.width();

            tempElement.remove();

            return textWidth;
        },
        ComputeBodyHeight: function (bodyRows, bodyRowHeight) {
            var totalHeight = 0;
            var bodyLength = bodyRows.length;
            for (var i = 0; i < bodyLength; i++) {
                var currentRow = bodyRows[i];
                if (currentRow.RowHeight) {
                    totalHeight += currentRow.RowHeight;
                }
                else {
                    totalHeight += bodyRowHeight;
                }

                //totalHeight += 5;
            }

            return totalHeight;
        },
        KeepStateOfRows: function (cacheRowsHtml, renderedRows) {
            var mainRowsNeedKeppStateLength = renderedRows.length;
            for (var i = 0; i < mainRowsNeedKeppStateLength; i++) {
                var currentRow = $(renderedRows[i]);
                var rowIndex = parseInt(currentRow.attr('data-row-index'));
                cacheRowsHtml[rowIndex] = currentRow[0].outerHTML;
            }
        }
    };

    Fanex.Grid.List = Fanex.Grid.List || {};

    Fanex.Grid.Constant = Fanex.Grid.Constant ||
    {
        NO_DATA_INFO: $.fn.fanexGrid.defaults.noDataText
    };

    Fanex.Grid.Language = Fanex.Grid.Language || {};

    //----------------------------------------------
    // Search Devives

    var deviceIphone = "iphone";
    var deviceIpod = "ipod";
    var deviceIpad = "ipad";
    var deviceS60 = "series60";
    var deviceSymbian = "symbian";
    var engineWebKit = "webkit";
    var deviceAndroid = "android";
    var deviceBB = "blackberry";
    var devicePalm = "palm";
    var deviceWinMob = "windows ce";

    var uagent = navigator.userAgent.toLowerCase();
    var currentDevice = "";
    function detectSmartPhone() {
        if (uagent.indexOf(deviceIphone) > -1) {
            currentDevice = deviceIphone;
            return true;
        }
        else if (uagent.indexOf(deviceIpod) > -1) {
            currentDevice = deviceIpod;
            return true;
        }
        else if (uagent.indexOf(deviceIpad) > -1) {
            currentDevice = deviceIpad;
            return true;
        }
        else if (uagent.indexOf(deviceS60) > -1) {
            currentDevice = deviceS60;
            return true;
        }
        else if (uagent.indexOf(deviceSymbian) > -1) {
            currentDevice = deviceSymbian;
            return true;
        }
        else if (uagent.indexOf(engineWebKit) > -1) {
            currentDevice = engineWebKit;
            return true;
        }
        else if (uagent.indexOf(deviceAndroid) > -1) {
            currentDevice = deviceAndroid;
            return true;
        }
        else if (uagent.indexOf(deviceBB) > -1) {
            currentDevice = deviceBB;
            return true;
        }
        else if (uagent.indexOf(devicePalm) > -1) {
            currentDevice = devicePalm;
            return true;
        }
        else if (uagent.indexOf(deviceWinMob) > -1) {
            currentDevice = deviceWinMob;
            return true;
        }
        else {
            return false;
        }
    }

    var isOnSmartPhoneBrowser = detectSmartPhone();
    var isOnChromeBrowser = navigator.userAgent.toLowerCase().indexOf('chrome') > -1;

    var ub = Fanex.Grid.Helper.DetectBrowser();
    ub.chrome = /chrome/.test(navigator.userAgent.toLowerCase());
    var ff16 = ub.mozilla && parseInt(ub.version, 10) >= 16;
    var ff18 = ub.mozilla && (parseInt(ub.version, 10) >= 18 && parseInt(ub.version, 10) < 25);
    var extraPosition = ff18 ? 1 : 0;

    function isValid(obj) {
        if (typeof obj !== "undefined" && obj != null) {
            return true;
        }
        return false;
    }

    function clone(obj) {
        var target = {};
        for (var i in obj) {
            if (obj.hasOwnProperty(i)) {
                target[i] = obj[i];
            }
        }
        return target;
    }
})(jQuery);

//// Language
/* jshint ignore:start */
if (typeof (fanexGridLanguage) === 'undefined') {
    /* File Created: December 4, 2013 */
    var fanexGridLanguage = {};
    fanexGridLanguage['en-US'] = {
        noData: 'There is no data',
        pageSize: 'Page size',
        goToPage: 'Go To Page',
        pagingText: 'Displaying <b>{0}</b> to <b>{1}</b> of <b>{2}</b> item(s)'
    };

    var fanexGridCurrentLanguage = fanexGridLanguage['en-US'];
}
/* jshint ignore:end */
// Array Remove - By John Resig (MIT Licensed)
//Array.prototype.removeArrayItem1 = function (from, to) {
//    var rest = this.slice((to || from) + 1 || this.length);
//    this.length = from < 0 ? this.length + from : from;
//    return this.push.apply(this, rest);
//};