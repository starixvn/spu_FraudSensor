function detail_account() {
    var assosiation_report = $('#assosiation_report');   

    var containers = [
        { name: "chains_evidence", url: common.modifyRelativeUrl('/SearchAccount/ChainsToEvidence') },
        { name: "retract_evidence", url: common.modifyRelativeUrl('/SearchAccount/RetractAndException') },
        { name: "associated_account", url: common.modifyRelativeUrl('/SearchAccount/AssociatedAccount') },
        { name: "associated_device", url: common.modifyRelativeUrl('/SearchAccount/AssociatedDevice') },
        { name: "transaction_history", url: common.modifyRelativeUrl('/SearchAccount/TransactionHistory') }
    ];

    function Init() {
        $(assosiation_report).find('.head').each(function () {
            $(this).click(GetDetail);
        });
    }

    function GetDetail() {
        var name = $(this).attr('ref');
        var url = GetUrl(name);

        GetInformation(name, url);
    }

    function GetInformation(name, url) {
        
        $.ajax(url, {
            type: 'POST',
            data: {
                keySearch: KEY_SEARCH
            },
            success: function (response) {
                AppendToHtml('#' + name, response);
            },
            error: function (errorMessage) {
                console.log(errorMessage);
            }
        });
    }

    function GetUrl(name) {
        for (let i = 0; i < containers.length; i += 1) {
            if (containers[i].name == name) {
                return containers[i].url;
            }
        }

        return '';
    }

    function AppendToHtml(container, data) {
        $(container).show();
        $(container).html(data);
    } 

    Init();   
}

$(detail_account);