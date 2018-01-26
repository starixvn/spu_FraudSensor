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

        if (callbacks !== null) callback(obj, arr);

        return arr;
    }

    function renderRow(columns, datum, callback) {
        var array = [];
        var obj = {};

        for (var prop in columns) {
            var value = data[prop];

            var row =
                {
                    Name: prop,
                    Value: value === null ? "-" : value
                }

            obj[prop] = row;
            arr.push(row);
        }

        if (callback !== null) callback(obj, array);

        return { Columns: array };
    }

    function renderRows(columns, data, callback) {
        var row_arrays = []

        for (var i = 0; i < data.length; i++) {
            var array = renderRow(columns, data[i], callback);

            row_arrays.push(array);
        }    

        return row_arrays;
    }

    $.fn.autoGrid = function (columns, gridOptions, data, callbacks) {
        var options = Object.assign({}, gridOptions);

        gridOptions.columns = renderHeader(columns, callbacks.header);
        gridOptions.bodyRows = renderRows(columns, data, callbacks.row);

        this.fanexGrid(gridOptions);

        return this;
    }
})(jQuery);