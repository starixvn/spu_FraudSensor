var timeout_holder = null;
var match_table = $("#match_grid table");
var noteMaxLength = 500;
var noteLengthCollapse = 180;

function load_page_events() {

    $("#generate, #refresh_btn").click(load_grid);

    $("#league").select2();

    $("#sport, #is_verified, #match_status, #refresh_interval").select2({
        minimumResultsForSearch: -1
    })

    $("#ddlSites").multiselect({
        menuWidth: 225,
        menuHeight: 350
    }).multiselect('checkAll')
        .multiselectfilter({
            width: 140,
            placeholder: ''
        });

    $("#event_date")
        .datepicker({
            dateFormat: "M dd, yy",
            minDate: balanceUpDay,
            changeMonth: true,
            changeYear: true,
        })
        .change(getLeague)
        .datepicker("setDate", moment().toDate());

    getLeague();

    $(".icon-calendar-text").click(() => $("#event_date").focus());

    $("#refresh_interval").val(60).change(e => refreshChange($(e.target))).change();

    $("#export").click(exportExcel);

    var t = $("#match_grid table")
        .DataTable({
            searching: false,
            lengthMenu: [10, 20, 50, 100, 500],
            pageLength: 50,
            scrollY: "61vh",
            scrollX: true,
            scrollCollapse: true,
            columnDefs: [
                { targets: "_all", defaultContent: "-" },
                { targets: [-2, -3, -4], type: "fraud-case" },
                { targets: 1, type: "check-box" },
                { targets: 4, type: "score" },
                { targets: [0, -1, 1, 2], orderable: false }
            ],
            columns: [
                { data: (row, type, set, meta) => meta.row + 1, className: "text-center", width: "40px" },
                { data: renderVerifyCheckBox, className: "text-center", width: "40px" },
                { data: row => row.VerifiedBy, width: "100px" },
                { data: renderEvent },
                { data: row => row.Score, className: "text-center", width: "60px" },
                { data: row => row.Status, className: "text-center", width: "70px" },
                { data: row => renderFraudCase(row, 1), className: "has-icon text-center", width: "70px" },
                { data: row => renderFraudCase(row, 6), className: "has-icon text-center", width: "80px" },
                { data: row => renderFraudCase(row, 8), className: "has-icon text-center", width: "70px" },
                { data: row => renderFraudCase(row, 10), className: "has-icon text-center", width: "50px" },
                {
                    data: renderNote, className: "col-note", width: "300px", createdCell: (cell, _, row) => {
                        $(".icon-note", cell).click(() => renderNoteEdit(cell, row));
                        $(".note-ellipsis", cell).click(() => expandNote(cell));
                    }
                }
            ],
            rowCallback: function (cell) {
                if ($(window).width() < 1085) {
                    $(cell).find("#note-content").css('min-height', '158px');
                } else if ($(window).width() < 1200) {
                    $(cell).find("#note-content").css('min-height', '140px');
                } else if ($(window).width() < 1366) {
                    $(cell).find("#note-content").css('min-height', '78px');
                }
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
            headerCallback: th => $("th", th).removeClass("has-icon"),
            order: [[3, "asc"]]
        });

    t.on('order.dt search.dt', function () {
        t.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
            cell.innerHTML = i + 1;
        });
    });

    $.fn.dataTable.ext.errMode = "none";

    $.fn.dataTable.ext.type.order["fraud-case-pre"] = function (data) {
        var num = $(".number", data).html();

        if (num == null || num.length == 0) {
            return 0;
        }
        else if (num.includes("K")) {
            return parseFloat(num) * 1e3;
        }
        else if (num.includes("M")) {
            return parseFloat(num) * 1e6;
        }
        else {
            return parseFloat(num);
        }
    };

    $.fn.dataTable.ext.type.order["check-box-pre"] = function (data) {
        var isChecked = $("input", data).is(":checked");

        return isChecked ? 1 : 0;
    };

    $.fn.dataTable.ext.type.order["score-pre"] = function (data) {
        var arr = data.split("-");

        var home = parseInt(arr[0]);

        var away = parseInt(arr[1]);

        return home + away;
    };

    t.on('order.dt search.dt', function () {
        t.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
            cell.innerHTML = i + 1;
        });
    }).draw();

    load_grid();
}

function buildGridParam() {
    var dto = {};

    var sites = [];
    $('option:selected', "#ddlSites").each(function (idx, ele) {
        sites.push($(ele).val());
    });

    dto.Sites = sites.join(',');
    dto.EventDate = $("#event_date").val();
    dto.IsVerified = $("#is_verified").val();
    dto.MatchStatus = $("#match_status").val();
    dto.LeagueIds = $("#league").val();
    dto.IsMainMarket = $("#is_main_market:checked").length > 0;

    return dto;
}

function renderVerifyCheckBox(obj) {
    return "<label class='checkbox'>"
        + "<input type= 'checkbox' class='verified-checkbox'" + (obj.Verified ? "checked" : "") + " data-match-id='" + obj.MatchId + "'/>"
        + "<span></span></label>";
}

function renderEvent(obj) {
    return "<span class='team-home'>" + obj.Event.HomeName + "</span>"
        + " - vs - "
        + "<span class='team-away'>" + obj.Event.AwayName + "</span>"
        + "<div class='league-name'>" + obj.Event.LeagueName + "</div>";
}

function renderFraudCase(obj, selector) {
    var fraudCase = obj.FraudCases[selector];

    if (selector == 1 && obj.Score == "0 - 0" && fraudCase == null && obj.Status.toLowerCase() == "completed") {
        return '<span class="icon-approval"></span>';
    }

    if (fraudCase == null && obj.Status.toLowerCase() == "completed") {
        return '<span class="icon-approval"></span>';
    }

    if (fraudCase == null) {
        return "<span class='icon-neutral'></span>";
    }

    if (fraudCase.Violations == 0) {
        return '<span class="icon-approval"></span>';
    }

    var eventDate = $("#event_date").val();

    return `<a class='warning' href='MatchMonitor/GetFraudCaseDetails?fraudCaseId=${selector}&matchId=${obj.MatchId}&eventDate=${eventDate}&violations=${fraudCase.Violations}' target='_blank'>
            <span class='icon-alert-circle' ></span>
            <span class='number'>${renderViolations(fraudCase.Violations)}</span>
        </a>`;
}

function renderNote(row) {
    var noteWrap = wrapString(row.Notes, noteLengthCollapse);

    return '<div class="container-note">'
        + '<textarea class="input-note-content hide" rows="5">' + (row.Notes != '-' ? row.Notes : '') + '</textarea>'
        + '<div class="note-content" id="note-content">' + noteWrap + '</div>'
        + '<div class="icon-note-text icon-note" id="icon"></div>'
        + '</div>';
}

function wrapString(str, maxLength) {

    if (str == null || str == '') {
        return '-';
    }

    var newStr = '';
    var breaksStr = str.split('\n');

    for (var i = 0; i < breaksStr.length; i++) {

        if (i == 5) {
            newStr = newStr + breaksStr[i];
            return newStr.substring(0, maxLength - (i * 2) - 3) + '<span class="note-ellipsis" title="click to see whole note"> ... </span>';
        }

        newStr = newStr + breaksStr[i] + "\n";
        if (newStr.length > maxLength) {
            return newStr.substring(0, maxLength - (i * 2) - 3) + '<span class="note-ellipsis" title="click to see whole note"> ... </span>';
        }
    }

    return str;
}

function splitString(inputNote, str, maxLength) {
    if (str != null && str.length > maxLength) {

        var strSplited = str.substring(0, maxLength);

        $(inputNote).val(strSplited);

        return strSplited;
    }

    return str;
}

function replaceSpecialCharacter(str) {
    if (str != null) {
        return str.replace(/&/g, "&amp;").replace(/>/g, "&gt;").replace(/</g, "&lt;");
    }
    return str;
}

function expandNote(cell) {
    var noteExpand = $(cell).find('.input-note-content').val();

    var noteExpand = noteExpand + "  " + '<span class="note-collapse icon-chevron-up" title="click to collapse up"></span>';

    $(cell).find("#note-content").html(noteExpand);

    $(".note-collapse", cell).click(() => collapseNote(cell));

    match_table.DataTable().columns.adjust();
}

function collapseNote(cell) {
    var noteCollapse = $(cell).find('.input-note-content').val();
    noteCollapse = wrapString(noteCollapse, noteLengthCollapse);

    $(cell).find("#note-content").html(noteCollapse);

    $(".note-ellipsis", cell).click(() => expandNote(cell));

    match_table.DataTable().columns.adjust();
}

function renderNoteEdit(cell, row) {
    var inputNoteContent = $(cell).find('.input-note-content');
    var noteContent = $(cell).find("#note-content");
    var noteIcon = $(cell).find("#icon");
    var heightNote = $(noteContent).height();

    if ($(inputNoteContent).hasClass('hide')) {
        $(inputNoteContent).removeClass('hide').focus().select();

        // adjust height input Note
        $(inputNoteContent).height(heightNote);

        $(noteContent).addClass('hide').hide();
        $(noteIcon).removeClass('icon-note-text').addClass('icon-note-save').attr('title', 'save');

        $(inputNoteContent).mouseout(function () {
            match_table.DataTable().columns.adjust();
        });

        $(inputNoteContent).keyup(function () {
            $(inputNoteContent).addClass('change');
        });

    } else if ($(noteIcon).hasClass('icon-note-save')) {
        saveNote(cell, row);
    }
}

function saveNote(cell, row) {

    var inputContent = $(cell).find('.input-note-content');
    var content = $(cell).find("#note-content");
    var noteNew = $(inputContent).val().trim();
    noteNew = replaceSpecialCharacter(noteNew);

    if ($(inputContent).hasClass('change')) {
        var url = common.modifyRelativeUrl("/MatchMonitor/UpdateMatchNote");

        noteNew = splitString(inputContent, noteNew, noteMaxLength);

        $.post(url, { Notes: noteNew, MatchId: row.MatchId }, function (result) {
            if (result.ErrorCode == 0) {
                // wrap text on Note
                var noteWrap = wrapString(noteNew, noteLengthCollapse);
                $(content).html(noteWrap);

                $(".note-ellipsis", cell).click(() => expandNote(cell));

                common.showSuccess('Note updated!');

            } else {
                console.log(result.ErrorCode);

                common.showError('Note fail!');
            }
        });
    }

    $(inputContent).removeClass('change').addClass('hide');
    $(content).removeClass('hide').show();

    match_table.DataTable().columns.adjust();
    $(cell).find("#icon").removeClass('icon-note-save').addClass('icon-note-text').attr('title', 'edit');
}

function load_grid() {
    common.showLoading();

    var url = "matchmonitor/GetMatches";

    var dto = buildGridParam();

    if (dto.LeagueIds == null || dto.LeagueIds.length == 0) {
        common.closeLoading();
        return;
    }

    $.post(url, dto, attachGridEvent)
}

function attachGridEvent(json) {
    $("#match_grid table").DataTable().clear().rows.add(json.data).draw();

    $(".verified-checkbox").change(updateVerification);

    common.closeLoading();

    $("#refresh_interval").change();

    $("#event_date").datepicker("hide");

    //$("span .number").click(e => $(e.target).parent().find("a").click());
}

function renderViolations(violation = 1) {
    if (violation < 1000) {
        return violation;
    }

    if (parseInt(violation / 1e6) != 0) {
        return parseInt(violation / 1e6) + "M";
    }
    else {
        return parseInt(violation / 1e3) + "K";
    }
}

function getLeague() {
    var url = "matchmonitor/getleagues";
    var dto =
        {
            EventDate: $("#event_date").val(),
            SportId: $("#sport").val()
        };

    $.post(url, dto, renderLeagueDropdown);
}

function renderLeagueDropdown(data) {
    var league_selector = $("#league").empty();
    var generate_btn = $("#generate");
    var export_btn = $("#export");

    if (data.length != 0) {
        var all_league = data.map(l => l.LeagueId).join();

        league_selector.append("<option value='" + all_league + "'>All League</option>");

        for (var i = 0; i < data.length; i++) {
            league_selector.append("<option value='" + data[i].LeagueId + "'>" + data[i].LeagueName + "</option>");
        }

        generate_btn.prop("disabled", false);
        export_btn.prop("disabled", false);
    }
    else {
        league_selector.append("<option value=''>No available league</option>");

        generate_btn.prop("disabled", true);
        export_btn.prop("disabled", true);
        $("#refresh_interval").val("--").change();
    }
}

function refreshChange($target) {
    var selectedVal = parseInt($target.val());
    var countdown = $("#refresh_countdown");
    clearTimeout(timeout_holder);

    if (isNaN(selectedVal)) {
        countdown.text("-");
    }
    else {
        countdown.text(selectedVal);
        startCountingDown(countdown, selectedVal, selectedVal);
    }
}

function startCountingDown($target, originalVal, val) {
    if (val != 0) {
        $target.text(val);

        timeout_holder = setTimeout(() => startCountingDown($target, originalVal, val - 1), 1000);
    }
    else {
        $("#generate").click();
        startCountingDown($target, originalVal, originalVal);
    }
}

function updateVerification() {
    var $target = $(this);

    var dto =
        {
            MatchId: $target.attr("data-match-id"),
            IsVerified: $target.is(":checked")
        };

    $.post("matchmonitor/VerifyMatch", dto, function () {
        if (!dto.IsVerified) {
            $target.parents("td").next().text("-");
        }
        else {
            $target.parents("td").next().text(username);
        }

        common.showSuccess();
    });
}

function mappingFraudCase(fraudCase = "") {
    switch (fraudCase) {
        case "BSNG":
            return 1;
        case "Flagged Accounts":
            return 6;
        case "Delay Goal":
            return 8;
        default:
            throw "not impletmented";
    }
}

function exportExcel() {
    var url = common.modifyRelativeUrl("/Matchmonitor/GenerateExcel");
    var urlDownload = common.modifyRelativeUrl("/Matchmonitor/DownloadExcel");
    var dto = buildGridParam();
   
    $.ajax({      
        type: 'POST',
        url: url,
        data: dto,
        global: false,
        crossDomain: true,
        success: function (data) {            
            common.downloadFile(urlDownload, { fileId: data.FileId, fileName: data.FileName });            
        },
        error: function (xhr, status, error) {           
            console.log('data of ' + xhr.responseText);
        }
    });
}

$(load_page_events);