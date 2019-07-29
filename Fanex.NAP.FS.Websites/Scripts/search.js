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
                break;
            default:
                break;
        }
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
}

$(search);