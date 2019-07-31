function detail_account() {
    var assosiation_report = $('#assosiation_report');
    var collapseAll = ('.collapse-all');
    var expandAll = $('.expand-all');

    var containers = [
        { name: "chains_evidence", url: common.modifyRelativeUrl('/SearchAccount/ChainsToEvidence') },
        { name: "retract_evidence", url: common.modifyRelativeUrl('/SearchAccount/RetractAndException') },
        { name: "associated_account", url: common.modifyRelativeUrl('/SearchAccount/AssociatedAccountByAccount') },
        { name: "associated_device", url: common.modifyRelativeUrl('/SearchAccount/AssociatedDeviceByAccount') },
        { name: "transaction_history", url: common.modifyRelativeUrl('/SearchAccount/TransactionHistory') }
    ];   

    function GetDetail() {  
        var name = $(this).attr('ref');

        if (!$(this).hasClass('display')) {
            $(this).addClass('display');           

            $('#' + name).removeAttr('hidden').show();
            $('span', $(this)).removeClass('ui-icon-triangle-1-e').addClass('ui-icon-triangle-1-s');
            $(collapseAll).removeAttr('hidden');

            return GetInformation(name, GetUrl(name));
        }

        $(this).removeClass('display');
        $('span', $(this)).removeClass('ui-icon-triangle-1-s').addClass('ui-icon-triangle-1-e');
        $('#' + name).hide();
    }

    function GetInformation(name, url) {
        
        $.ajax(url, {
            type: 'POST',
            data: {
                keySearch: ACCOUNTID
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

    function ExpandAll() {
        $(assosiation_report).find('.head').each(GetDetail);

        $(expandAll).hide();
        $(collapseAll).show();
    }

    function CollapseAll() {
        $(assosiation_report).find('.head').each(function () {
            var name = $(this).attr('ref');
            $('#' + name).hide();
            $('span', $(this)).removeClass('ui-icon-triangle-1-s').addClass('ui-icon-triangle-1-e');
        });

        $(collapseAll).hide();
        $(expandAll).show();        
    }

    $(assosiation_report).find('.head').each(function () {
        $(this).click(GetDetail);
    });

    $(expandAll).click(ExpandAll);
    $(collapseAll).click(CollapseAll);
}

$(detail_account);