function associated_account_monitor_detail() {
    var filter_sites = $('#filter-sites');
    var filter_cust_levels = $('#filter-cust-levels');
    var filter_verify_status = $('#filter-verify-status');
    var filter_evidences = $('#filter-evidences');   
    var btn_generate = $('#btn-generate');
    var grid_associated_account = $('#grid-associated-account-detail');
    var const_note_length_of_collapse = 50;

    function init() {
        filter_sites.multiselect({
            menuWidth: 225,
            menuHeight: 350
        }).multiselect('checkAll')
            .multiselectfilter({
                width: 145,
                placeholder: '',
                label: 'Filter'
            });

        filter_cust_levels.multiselect({
            menuWidth: 225,
            menuHeight: 350
        }).multiselect('checkAll')
            .multiselectfilter({
                width: 145,
                placeholder: '',
                label: 'Filter'
            });

        filter_evidences.multiselect({
            menuWidth: 330,
            menuHeight: 350
        })
            .multiselect('checkAll')
            .multiselectfilter({
                width: 250,
                placeholder: '',
                label: 'Filter'
            });
     
        renderAssociatedAccountGridOptions();

        generateReport();
    }


    function renderAssociatedAccountGridOptions() {
        grid_associated_account.DataTable({
            searching: false,
            lengthMenu: [10, 20, 50, 100, 500],
            pageLength: 50,          
            scrollY: "61vh",
            scrollX: true,
            scrollCollapse: true,
            columnDefs: [
                { targets: "_all", defaultContent: "-" },
                { targets: 7, type: "check-box" },
                { targets: [0, 2, 3, 4, 5, 6, 7, 8, 9], orderable: false }
            ],
            columns: [
                { data: (row, type, set, meta) => meta.row + 1, className: "text-center", width: "5%" },
                { data: row => row.AssociatedCustUsername, className: "text-center", width: "10%" },
                { data: row => row.AssociatedCustSite, className: "text-center", width: "10%" },
                { data: row => row.AssociatedCustLevelName, className: "text-center", width: "8%" },
                { data: row => renderEvidence(row.AssociatedCustEvidences), className: "text-left", width: "15%" },
                { data: row => row.FirstAssociatedDateToString, className: "text-center", width: "10%" },
                {
                    data: row => renderAllAssociations(row), className: "text-center", width: "8%", createdCell: (cell, _, row) => {
                        $('.btn-all-associations', cell).click(() => handleAllAssociatedAccounts(cell));
                    }
                },
                {
                    data: renderverificationCheckBox, className: "text-center", width: "5%", createdCell: (cell, _, row) => {
                        $('.chk-verification', cell).change(() => handleUpdateVerification(cell, row));
                    }
                },
                { data: row => row.VerifiedOfView, className: "text-center", width: "12%" },
                {
                    data: renderNote, className: "col-account-note text-left", createdCell: (cell, _, row) => {
                        $('.btn-note-change', cell).click(() => handleChangeNote(cell, row));
                        $('.btn-note-ellipsis', cell).click(() => handleExpandNote(cell));
                    }
                }
            ],
            rowCallback: function (cell, row, index) {
                adjustSizeOfNote(cell);
            },
            dom: "t<'pagination'<'page-size'lp>i>",
            language: {
                lengthMenu: "Page size _MENU_",
                paginate: {
                    previous: "Back"
                },
                info: "Displaying _START_ to _END_ of _TOTAL_ item(s)",
                infoEmpty: "",
                zeroRecords: "There's no data"
            },
            order: [[1, "asc"]]
        });
    }

    function renderEvidence(evidences) {
        if (evidences != null && evidences != '') {
            var evidencesForDisplay = common.makeWrapString(evidences, 42);
            return "<span title='" + evidences + "'>" + evidencesForDisplay + "</span>";
        }
        return evidences;
    }   

    function renderAllAssociations(row) {
        var allAssociationsFormat = '<span class="btn-all-associations link" data-cust-id="{0}" data-user-name="{1}">View</span>';

        return allAssociationsFormat.format(row.AssociatedCustId, row.AssociatedCustUsername);
    } 

    function renderverificationCheckBox(row) {
        return "<label class='checkbox'>"
            + "<input type= 'checkbox' data-ass-acc-id = '" + row.AssAccId + "' class='chk-verification'" + (row.IsVerified ? "checked" : "") + "/>"
            + "<span></span></label>";
    }

    function renderNote(row) {
        var noteForDisplay = wrapTextForDipslayNote(row.Note, const_note_length_of_collapse);
        var domElementNoteForDisplay = '<div class="note-content-display" title=' + row.Note + '>' + noteForDisplay + '</div>';

        if (noteForDisplay == '-') {
            domElementNoteForDisplay = '<div class="note-content-display note-blank" title=' + row.Note + '>' + noteForDisplay + '</div>';
        }

        return '<div class="container-note">'
            + '<textarea class="txt-note-input hide" data-ass-acc-id="' + row.AssAccId + '" rows="10">' + (row.Note != '' ? row.Note : '') + '</textarea>'
            + domElementNoteForDisplay
            + '<div class="btn-note-change icon-edit icon"></div>'
            + '</div>';
    }

    function wrapTextForDipslayNote(textInput, maxLengthDisplay) {
        if (textInput == null || textInput == '') {
            return '-';
        }
        var textOutPut = '';
        var textBreaks = textInput.split('\n');
        var htmlSpan = '<span class="btn-note-ellipsis" title="click to see whole note"> ... </span>';

        for (var i = 0; i < textBreaks.length; i++) {
            if (i == 3) {
                textOutPut = textOutPut + textBreaks[i];
                return textOutPut.substring(0, maxLengthDisplay - 4) + htmlSpan;
            }

            textOutPut = textOutPut + textBreaks[i] + "\n";
            if (textOutPut.length > maxLengthDisplay) {
                return textOutPut.substring(0, maxLengthDisplay - 4) + htmlSpan;
            }
        }

        return textInput;
    }

    function generateReport() {
        var url = common.modifyRelativeUrl("/AssociatedAccountsMonitor/GetAssociatedAccountsMonitorDetail");
        var paramFilter = getParamsFilter();

        common.showLoading();

        $.ajax({
            type: 'POST',
            url: url,
            data: paramFilter,
            success: function (response) {
                renderAssociatedAccountGrid(response);
            },
            error: function (xhr, status, error) {
                console.log('data of ' + xhr.responseText);
                common.closeLoading();
            }
        });
    }

    function getParamsFilter() {
        var filter = {};

        filter.DateFrom = DATEFROM;
        filter.DateTo = DATETO;
        filter.AccountCustId = ACCOUNTCUSTID;
        filter.Sites = getSeletedMultiple("#filter-sites");
        filter.CustLevels = getSeletedMultiple("#filter-cust-levels");
        filter.VerifyStatus = filter_verify_status.val();
        filter.EvidenceCodes = getSeletedMultiple("#filter-evidences");
        filter.EvidencesCache = EVIDENCECACHE;

        return filter;
    }

    function getSeletedMultiple(multipleName) {
        var selects = [];
        $('option:selected', multipleName).each(function (idx, ele) {
            selects.push($(ele).val());
        });

        return selects.join(",");
    }

    function renderAssociatedAccountGrid(json) {
        grid_associated_account.DataTable().clear().rows.add(json).draw();
        grid_associated_account.DataTable().columns.adjust();
        common.closeLoading();
    }

    function handleAllAssociatedAccounts(cell) {
        var target = $(cell).find('.btn-all-associations');
        var custId = target.attr('data-cust-id');
        var userName = target.attr('data-user-name');

        var formAllAssociatedAccount = new all_associated_accounts();
        formAllAssociatedAccount.OpenModal(custId, userName);
    }

    function handleUpdateVerification(cell, row) {
        var $target = $(cell).find('.chk-verification');

        var url = common.modifyRelativeUrl('/AssociatedAccountsMonitor/VerifyAssociatedAccountMonitor');
        var paramsUpdate = {};
        paramsUpdate.IsVerifying = $target.is(':checked');
        paramsUpdate.AssAccId = $target.attr('data-ass-acc-id');

        common.showLoading();
        $.ajax({
            type: 'POST',
            url: url,
            data: paramsUpdate,
            success: function (response) {
                common.closeLoading();
                if (response.IsSuccessStatusCode) {
                    if (paramsUpdate.IsVerifying) {
                        $target.parents("td").next().text(response.Content);
                    }
                    else {
                        $target.parents("td").next().text("-");
                    }
                    common.showSuccess();
                } else {
                    common.showError(response.Content);
                }
            },
            error: function (xhr, status, error) {
                console.log('data of ' + xhr.responseText);
                common.closeLoading();
            }
        });
    }

    function handleChangeNote(cell, row) {
        var domElementsNote = {};
        domElementsNote.txtNoteInput = $(cell).find('.txt-note-input');
        domElementsNote.noteForDisplay = $(cell).find(".note-content-display");
        domElementsNote.iconNote = $(cell).find(".icon");

        if ($(domElementsNote.txtNoteInput).hasClass('hide')) {
            handleEditNote(domElementsNote);
        } else if ($(domElementsNote.iconNote).hasClass('icon-save')) {
            handleSaveNote(cell, row);
        }
    }

    function handleEditNote(domElementsNote) {
        $(domElementsNote.txtNoteInput).removeClass('hide').focus().select();
        $(domElementsNote.noteForDisplay).addClass('hide').hide();
        $(domElementsNote.iconNote).removeClass('icon-edit').addClass('icon-save').attr('title', 'save');
        grid_associated_account.DataTable().columns.adjust();

        adjustHeightNoteInput(domElementsNote);
        handleNoteInputEvents(domElementsNote);
    }

    function adjustHeightNoteInput(domElementsNote) {
        $(domElementsNote.txtNoteInput).height($(domElementsNote.noteForDisplay).height());
    }

    function handleNoteInputEvents(domElementsNote) {
        $(domElementsNote.txtNoteInput).mouseout(function () {
            grid_associated_account.DataTable().columns.adjust();
        });

        $(domElementsNote.txtNoteInput).keyup(function () {
            $(domElementsNote.txtNoteInput).addClass('change');
        });
    }

    function handleSaveNote(cell, row) {
        var domElementsNote = {};
        domElementsNote.txtNoteInput = $(cell).find('.txt-note-input');
        domElementsNote.contentNoteDisplay = $(cell).find(".note-content-display");

        var noteToUpdate = $(domElementsNote.txtNoteInput).val().trim();
        noteToUpdate = handleSpecialCharacter(noteToUpdate);

        if ($(domElementsNote.txtNoteInput).hasClass('change')) {
            saveChangedNote(domElementsNote, noteToUpdate, cell);
        }

        $(domElementsNote.txtNoteInput).removeClass('change').addClass('hide');
        $(domElementsNote.contentNoteDisplay).removeClass('hide').show();
        $(cell).find(".icon").removeClass('icon-save').addClass('icon-edit').attr('title', 'edit');

        grid_associated_account.DataTable().columns.adjust();
    }

    function saveChangedNote(domElementsNote, noteToUpdate, cell) {
        var url = common.modifyRelativeUrl("/AssociatedAccountsMonitor/UpdateNoteAssociatedAccountMonitor");
        var paramsUpdate = {};
        paramsUpdate.AssAccId = $(domElementsNote.txtNoteInput).attr('data-ass-acc-id');
        paramsUpdate.Note = noteToUpdate;

        common.showLoading();
        $.ajax({
            type: 'POST',
            url: url,
            data: paramsUpdate,
            success: function (response) {
                common.closeLoading();
                if (response.IsSuccessStatusCode) {
                    var noteForDisplay = wrapTextForDipslayNote(noteToUpdate, const_note_length_of_collapse);
                    if (noteForDisplay == '-') {
                        $(domElementsNote.contentNoteDisplay).addClass('note-blank');
                        adjustSizeOfNote(cell);
                    } else {
                        $(domElementsNote.contentNoteDisplay).removeClass('note-blank');
                        $(domElementsNote.contentNoteDisplay).css({ 'padding-top': '6px' });
                    }
                    $(domElementsNote.contentNoteDisplay).html(noteForDisplay);
                    $(".btn-note-ellipsis", cell).click(() => handleExpandNote(cell));

                    common.showSuccess();
                } else {
                    common.showError(response.Content);
                }
            },
            error: function (xhr, status, error) {
                console.log('data of ' + xhr.responseText);
                common.closeLoading();
            }
        });
    }

    function adjustSizeOfNote(cell) {
        var domElementNoteDisplay = $(cell).find(".note-content-display");
        var domElementNoteBlank = $(cell).find(".note-blank");

        if ($(window).width() < 1108) {
            domElementNoteDisplay.css({ 'min-height': '77px' });
            domElementNoteBlank.css({ 'padding-top': '27px' });
        } else if ($(window).width() < 1835) {
            domElementNoteDisplay.css({ 'min-height': '57px' });
            domElementNoteBlank.css({ 'padding-top': '17px' });
        }
    }

    function handleExpandNote(cell) {
        var noteForDisplay = $(cell).find('.txt-note-input').val();

        var noteForDisplay = noteForDisplay + "  " + '<span class="btn-note-collapse icon-chevron-up" title="click to collapse up"></span>';

        $(cell).find(".note-content-display").html(noteForDisplay);

        $('.btn-note-collapse', cell).click(() => handleCollapseNote(cell));
    }

    function handleCollapseNote(cell) {
        var txtNoteInput = $(cell).find('.txt-note-input').val();
        var noteForDisplay = wrapTextForDipslayNote(txtNoteInput, const_note_length_of_collapse);
        $(cell).find(".note-content-display").html(noteForDisplay);

        grid_associated_account.DataTable().columns.adjust();
        $(".btn-note-ellipsis", cell).click(() => handleExpandNote(cell));
    }

    function handleSpecialCharacter(str) {
        if (str != null) {
            return str.replace(/&/g, "&amp;").replace(/>/g, "&gt;").replace(/</g, "&lt;");
        }
        return str;
    }

    init();

    btn_generate.click(generateReport);
}

$(associated_account_monitor_detail);