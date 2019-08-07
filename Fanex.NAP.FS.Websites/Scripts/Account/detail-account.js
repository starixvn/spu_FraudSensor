function detail_account() {
    var associated_report = $('#associated_report');
    var collapseAll = ('.collapse-all');
    var expandAll = $('.expand-all');
    var formException;

    var containers = [
        { name: "chains_evidence", url: common.modifyRelativeUrl('/Account/GetChainsToEvidence') },
        { name: "retract_evidence", url: common.modifyRelativeUrl('/Account/GetRetractAndException') },
        { name: "associated_account", url: common.modifyRelativeUrl('/Account/GetAssociatedAccountByAccount') },
        { name: "associated_device", url: common.modifyRelativeUrl('/Account/GetAssociatedDeviceByAccount') },
        { name: "transaction_history", url: common.modifyRelativeUrl('/Transaction/GetTransactionHistory') }
    ];   

    function ToogleDetail() {  
        var refContainer = $(this).attr('ref');
        var container = '#' + refContainer;

        RemoveHidden(container);

        if ($(container).hasClass('display')) {
            $('span', $(this)).removeClass('ui-icon-triangle-1-s').addClass('ui-icon-triangle-1-e');
            $(container).removeClass('display').hide();
            $(container).parent().removeClass('expanded');
        }
        else {
            if ($(container).hasClass('data-collected')) {
                $(container).addClass('display').show();
                $(container).parent().addClass('expanded');
            }
            else {
                GetInformation(container, GetUrl(refContainer));
            }

            $('span', $(this)).removeClass('ui-icon-triangle-1-e').addClass('ui-icon-triangle-1-s');

            ToogleExpandButton(false);
        }
    }

    function GetInformation(container, url) {
        
        $.ajax(url, {
            type: 'POST',
            data: {
                accountId: ACCOUNTID
            },
            success: function (response) {
                AppendToHtml(container, response);
            },
            error: function (errMsg) {
                console.log(errMsg);
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
        $(container).addClass('data-collected').addClass('display');
        $(container).parent().addClass('expanded');
        $(container).show();
        $(container).html(data);

        InitTabsForm(container);
    }    

    function ExpandAll() {
        $(associated_report).find('.head').each(ToogleDetail);

        $(expandAll).hide();
        $(collapseAll).show();
    }

    function CollapseAll() {
        $(associated_report).find('.head').each(function () {
            var container = '#' + $(this).attr('ref');
            $(container).removeClass('display').hide();
            $(container).parent().removeClass('expanded');
            $('span', $(this)).removeClass('ui-icon-triangle-1-s').addClass('ui-icon-triangle-1-e');
        });

        $(collapseAll).hide();
        $(expandAll).show();        
    }

    function InitTabsForm(container) {
        switch (container) {
            case "#chains_evidence":
                formException = new form_exception(ACCOUNTID, USERNAME);

                $('span', $('.chain-place')).click(OpenAddException);
                $('#btn_cancel').click(CloseException);
                $('#btn_addException').click(AddException);
                break;
            case "#associated_account":
                account_by_account();
                break;
            case "#associated_device":
                device_by_account();
                break;
            case "#transaction_history":
                transaction_history();
                break;
            default:
                break;
        }
    }

    function CloseException() {
        formException.CloseException();
    }

    function OpenAddException() {
        var nodeId = $(this).attr('node-id');
        var nodeName = $(this).attr('node-name');
        
        formException.OpenAddException(nodeId, nodeName);
    }

    function AddException() {
        formException.AddException();

        GetInformation("chains_evidence", GetUrl("chains_evidence"));

        if ($('#retract_evidence').hasClass('display')) {
            GetInformation("retract_evidence", GetUrl("retract_evidence"));
        }
    }

    function RemoveHidden(container) {
        if ($(container).is(':hidden')) {
            $(container).removeAttr('hidden');
            $(collapseAll).removeAttr('hidden');
        }        
    }

    function ToogleExpandButton(isShow) {
        if (isShow) {
            $(expandAll).show();
            $(collapseAll).hide();
        }
        else {
            $(expandAll).hide();
            $(collapseAll).show();
        }
    }

    $(collapseAll).hide();

    $(associated_report).find('.head').each(function () {
        $(this).click(ToogleDetail);
    });

    $(expandAll).click(ExpandAll);

    $(collapseAll).click(CollapseAll);
}

$(detail_account);