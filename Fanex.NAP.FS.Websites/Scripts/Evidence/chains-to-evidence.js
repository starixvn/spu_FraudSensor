function chains_to_evidence() {
    var chain_place = $('.chain-place');

    function OpenAddException() {
        $(function () {
            $('#addException').dialog({
                modal: true,
                width: '650px',
                resizable: false,
                draggable: false,
                close: function (event, ui) { $(this).dialog('close'); }              
            }).dialog('open');
        });
    }

    function AddException() {
        $('#addException').dialog('close');
    }

    function CloseException() {
        $('#addException').dialog('close');
    }

    $('#btn_cancel').click(CloseException);

    $('#btn_addException').click(AddException);

    $('span', chain_place).click(OpenAddException);
}