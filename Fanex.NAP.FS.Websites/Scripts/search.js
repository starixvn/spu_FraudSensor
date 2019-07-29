function search() {
    var btnSearch = $('#button-search');
    var input = $('#txt-search');
    var option = $('#search-option');
    var linkItem = $('#link-item');

    function SearchAction(key) {
        if (key == null || key.length == 0) {
            return;
        }

        var optionValue = $(option).attr('key');
        
    }

    function ChangeItem(e) {
        var value = $(this.target);
        
    }

    linkItem.click((e) => ChangeItem(e));

    input.keyup((e) => {
        if (e.which == 13) {
            SearchAction($(e.target).val());
        }
    });

    btnSearch.click(SearchAction($(input).val()));
}

$(search);