function search() {
    var common = new UICommon();  
    var input = $('#txt-search');
    var option = $('#search-option');
    var linkItem = $('#link-item');

    function SearchAction(key) {
        if (key == null || key.length == 0) {
            return;
        }

        var itemKey = $(option).attr('item-key');

        switch (itemKey) {
            case 'search-account':               
                window.location.href = common.modifyRelativeUrl('/SearchAccount/SearchAccount?key=' + key);
                break;
                
            case 'search-device':
                window.location.href = common.modifyRelativeUrl('/SearchDevice/SearchDevice?key=' + key);
                break;

            default:
                break;
        }
    }

    function ChangeItem(e) {
        var itemKey = $(e.target).attr('item-key');
        var itemText = $(e.target).text();
        var optionKey = $(option).attr('item-key');
        var optionText = $('span', option).text();

        $(linkItem).text(optionText);
        $(linkItem).attr('item-key', optionKey);

        $('span', option).text(itemText);
        $(option).attr('item-key', itemKey);
    }

    linkItem.click((e) => ChangeItem(e));

    input.keyup((e) => {
        if (e.which == 13) {
            SearchAction($(e.target).val());
        }
    });   
}

$(search);