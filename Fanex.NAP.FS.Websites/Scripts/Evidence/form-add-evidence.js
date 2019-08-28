function form_add_evidence(accountId) {
    var form = $('#form-evidence');
    var formGroupOrEvidence = new form_group_or_evidence();
    var evidences_groups_all = $('#evidences-groups-all');
    var evidences_groups_added = $('#evidences-groups-added');
    var show_div_evidence_group = $('#show-div-evidence-group');
    var div_link_modify = $('#div-link-modify');
    var div_evidence_group_management = $('#div-evidence-group-management');    

    this.OpenModal = function () {
        if (form.is('dialog')) {
            form.dialog('open');
        } else {
            $(form).dialog({
                modal: true,
                width: '850px',
                title: 'ADD EVIDENCE',
                resizable: true,
                draggable: true,
                close: function () { $(this).dialog('close'); }
            })
        }

        Clear();

        GetAllEvidences();

        GetEvidencesByAccount();
    }

    this.Submit = function () {
        var comment = $('#comment').val();
        var url = common.modifyRelativeUrl('/Account/UpdateEvidenceByAccount');

        $.ajax(url, {
            type: 'POST',
            data: {
                accountId: accountId,
                evidences: GetEvidenceForUpdate(),
                comment: comment
            },
            success: function (response) {
                common.showSuccess('The Evidence has added!');
            },
            error: function (errorMessage) {
                console.log(errorMessage);
                common.showError('The Evidence add fail!');
            }
        });

        form.dialog('close');
    }

    function CloseModal() {
        form.dialog('close');
    }

    function Clear() {
        $(evidences_groups_all).html(Loading);
        $(evidences_groups_added).html(Loading);
        $('#comment').html('');
    }

    function GetEvidenceForUpdate() {
        var evidences = [];
        var items = $('.item', evidences_groups_added);
        
        $(items).each((idx, eleItem) => {
            var id = $(eleItem).attr('id');
            evidences.push(id);
        });

        return evidences.join(',');
    }

    function GetAllEvidences() {
        var url = common.modifyRelativeUrl("/Account/GetAllEvidences");

        $.get(url, function (json) {
            BindEvidences(json, evidences_groups_all);
        });
    }

    function GetEvidencesByAccount() {
        var url = common.modifyRelativeUrl("/Account/GetEvidencesByAccount");

        $.get(url, { accountId }, function (json) {
            BindEvidences(json, evidences_groups_added);
        });
    }

    function BindEvidences(data, container) {
        var html = '';
        var groups = [];
        var length = data.length;

        for (var i = 0; i < length; i++) {

            html += ToHtmlGroups(data[i], groups, container);

            var text = data[i].EvidenceTypeCode + " " + data[i].EvidenceTypeName;

            html += ToHtmlEvidence(data[i].EvidenceTypeID, text, container);

            if (i == length - 1) {
                html += '</div></div>';
            }
        }

        $('.loading', container).hide();
        $(container).append(html);

        HandleEventInContainer(container);
        ToogleIconEvidenceGroupManagement(false);
    }

    function ToHtmlGroups(obj, groups, container) {
        var html = '';

        if (groups.indexOf(obj.EvidenceGroupID) < 0) {
            if (groups.length > 0) {
                html += '</div></div>';
            }            

            html += ToHtmlGroup(obj.EvidenceGroupID, obj.EvidenceGroupName, container);

            groups.push(obj.EvidenceGroupID);
        }

        return html;
    }

    function ToHtmlGroup(groupId, groupName, container) {
        var html = '';
        html += '<div class="group" id=' + groupId + '>';
        html += '<div class="group-heading">' + groupName;

        //if (container == evidences_groups_all) {
        //    html += '<span class="icon-plus icon-cursor" title="Click to add Evidence"> (+) </span>';
        //}
        html += '</div>';
        html += '<div class="group-content">';

        return html;
    }

    function ToHtmlEvidence(evidenceId, text, container) {
        var html = '<div class="item" id=' + evidenceId + '>';
        //if (container == evidences_groups_all) {
        //    html += '<span class="icon-delete icon-cursor" title="Click to delete Evidence" id="' + evidenceId + '"></span>';
        //}
        html += '<span class="evidence"> ' + text + '</span>';
        html += '</div>';

        return html;
    }

    function ToogleSelectEvidence() {
        var containerFrom = evidences_groups_added;
        var containerTo = evidences_groups_all;

        if ($(this).attr('id') == 'btn-add') {
            containerFrom = evidences_groups_all;
            containerTo = evidences_groups_added;
        }

        $('.group', containerFrom).each(function (idx, eleGroup) {
            var itemsSelected = $('.selected', $(eleGroup));
            var items = $('.item', $(eleGroup));

            if (itemsSelected.length > 0) {
                var groupId = $(eleGroup).attr('id');
                var groupName = $('.group-heading', eleGroup).text();

                AddItemsToContainer(groupId, groupName, itemsSelected, containerTo);
                RemoveItemsFromContainer(eleGroup, items, itemsSelected);
            }
        });
    }

    function AddItemsToContainer(groupId, groupName, itemsAdd, container) {
        var newGroup = true;
        $('.group', container).each(function (idx, eleGroup) {
            var currentGroupId = $(eleGroup).attr('id');

            if (groupId == currentGroupId) {
                var html = ToHtmlItems(itemsAdd);
                $('.group-content', eleGroup).append(html);

                newGroup = false;
            }
        });

        if (newGroup) {
            var html = ToHtmlGroup(groupId, groupName, container);
            html += ToHtmlItems(itemsAdd, html);
            html += '</div></div>';
            $(container).append(html);
        }
    }

    function ToHtmlItems(itemsAdd) {
        var html = '';

        $(itemsAdd).each(function (itemIdx, eleItem) {
            var id = $(eleItem).parent().attr('id');
            var text = $(eleItem).text();
            html += ToHtmlEvidence(id, text);
        });

        return html;
    }

    function RemoveItemsFromContainer(group, items, itemsSelected) {
        if (itemsSelected.length == items.length) {
            $(group).remove();
        }
        else {
            $(itemsSelected).parent().remove();
        }
    }

    function HandleEventInContainer(container) {
        $(container).on('click', '.evidence', function () {
            $(this).toggleClass('selected');
        });

        $(container).on('click', '.icon-plus', OpenFormAddGroupOrEvidence);

        $(container).on('click', '.icon-delete', DeleteEvidence);
    }

    function ToogleEvidenceGroupManagement() {
        if ($(this).attr('id') == "show-div-evidence-group") {
            $(div_link_modify).hide();
            $(div_evidence_group_management).show();
            ToogleIconEvidenceGroupManagement(true);
        }
        else {
            $(div_link_modify).show();
            $(div_evidence_group_management).hide();           
            ToogleIconEvidenceGroupManagement(false);
        }        
    }

    function ToogleIconEvidenceGroupManagement(show) {
        if (show) {
            $('.icon-plus', evidences_groups_all).show();

            $('.group-content', evidences_groups_all).each(function (idx, ele) {
                $('.icon-delete', ele).show();
            });
        }
        else {
            $('.icon-plus', evidences_groups_all).hide();
            $('.group-content', evidences_groups_all).each(function (idx, ele) {
                $('.icon-delete', evidences_groups_all).hide();
            });
        }
    }

    function OpenFormAddGroupOrEvidence() {
        if ($(this).attr('id') == '.icon-plus') {
            var groupId = $(this).attr('group');
            formGroupOrEvidence.OpenModal('NEW EVIDENCE', groupId);
        } else {
            formGroupOrEvidence.OpenModal('NEW EVIDENCE GROUP');
        }

        $('#group-or-evidence-btn-add').click(AddGroupOrEvidence);
    }

    function AddGroupOrEvidence() {
        formGroupOrEvidence.AddGroupOrEvidence();
    }

    function DeleteEvidence() {
        var evidenceId = $(this).attr('id');

        $.ajax({
            url: common.modifyRelativeUrl("/Account/DeleteEvidence"),
            type: 'GET',
            data: { evidenceId: evidenceId },
            success: function (response) {
                common.showSuccess('The evidence has deleted!');
            },
            error: function (data) {
                common.showError('The evidence delete fail!');
            }
        });
    }

    function Loading() {
        return '<div class="loading"><img src="../../Content/images/ajax-loader.gif" alt="Loading ..." /></div>';
    }

    function FilterEvidence() {
        var value = $(this).val().toLowerCase();
        $('.group *', evidences_groups_all).filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1);
        });        
    }

    $('#span-account').text(USERNAME);

    $('#span-subscriber').text(SUBSCRIBER);

    $(div_evidence_group_management).hide();

    //$(show_div_evidence_group).click(ToogleEvidenceGroupManagement);

    $('#btn-add-evidence-group').click(OpenFormAddGroupOrEvidence);

    $('#btn-cancel-evidence-group').click(ToogleEvidenceGroupManagement);

    $('#btn-ok-evidence-group').click(ToogleEvidenceGroupManagement);

    $('#btn-cancel-evidence').click(CloseModal);

    $('#btn-add').click(ToogleSelectEvidence);

    $('#btn-remove').click(ToogleSelectEvidence);

    $('#txt-search-evidence').keyup(FilterEvidence);
}