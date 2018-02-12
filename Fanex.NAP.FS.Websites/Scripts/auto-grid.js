(function ($) {
    function renderHeader(columns, callback) {
        var obj = {}
        var arr = []

        for (var prop in columns) {
            var row =
                {
                    HeaderText: columns[prop],
                    Name: prop,
                    HeaderAlign: "center",
                    Width: 100,
                    CellAlign: "center"
                }

            obj[prop] = row;
            arr.push(row);
        }

        if (callback != null) callback(obj, arr);

        return arr;
    }

    function renderRow(columns, datum, callback) {
        var arr = [];
        var obj = {};

        for (var prop in columns) {
            var value = datum[prop];

            var row =
                {
                    Name: prop,
                    Value: value == null ? "-" : value
                }

            obj[prop] = row;
            arr.push(row);
        }

        if (callback != null) callback(obj, arr);

        return { Columns: arr };
    }

    function renderRows(columns, data, callback) {
        var row_arrays = []

        for (var i = 0; i < data.length; i++) {
            var array = renderRow(columns, data[i], callback);

            row_arrays.push(array);
        }    

        return row_arrays;
    }

    $.fn.autoGrid = function (columns, gridOptions, data) {
        var options = Object.assign({}, gridOptions);

        options.columns = renderHeader(columns, options.headerCallback == null ? null : options.headerCallback);
        options.bodyRows = renderRows(columns, data, options.rowCallback == null ? null : options.rowCallback);

        this.fanexGrid(options);

        return this;
    }
})(jQuery);