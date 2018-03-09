var timeout_holder = null;

function load_page_events() {
    $("#generate, #refresh_btn").click(load_grid);

    $("#league").select2();

    getLeague();

    $("#event_date")
        .datepicker({
            dateFormat: "M dd, yy",
            minDate: balanceUpDay
        })
        .change(getLeague);

    $(".icon-calendar-text").click(() => $("#event_date").focus());

    $("#refresh_interval").change(e => refreshChange($(e.target)));

    $("#export").click(exportExcel);

    $("table").DataTable({
        ordering: false,
        searching: false,
        lengthMenu: [50, 100, 200],
        createdRow: function (row, data, idx) {
            var $tds = $("td", row);

            var center = [0, 1, 4, 5, 6, 7, 8];

            for (var i = 0; i < center.length; i++) {
                $tds.eq(center[i]).addClass("text-center");
            }

            var icon = [6, 7, 8];

            for (var i = 0; i < icon.length; i++) {
                $tds.eq(icon[i]).addClass("has-icon");
            }

            $tds.eq($tds.length - 1).addClass("col-note");
        },
        scrollY: "55vh",
        scrollCollapse: true
    });
}

function load_grid() {
    var url = "matchmonitor/GetMatches";
    var dto = {};

    dto.EventDate = $("#event_date").val();
    dto.IsVerified = $("#is_verified").val();
    dto.MatchStatus = $("#match_status").val();
    dto.LeagueIds = $("#league").val();
    dto.IsMainMarket = $("#is_main_market:checked").length > 0;

    common.showLoading();

    $.post(url, dto, function (data) {
        $("#event_date").datepicker("hide");

        data_holder = data;

        renderGrid(data);

        common.closeLoading();

        if ($("#refresh_interval").hasClass("first")) {
            $("#refresh_interval").val(60).change().removeClass("first");
        }
        else {
            $("#refresh_interval").change();
        }
    });
}

function renderGrid(data) {
    $("#match_grid").show();

    var matches = [];

    if (data != null && data.Matches != null && data.Matches.length > 0) {
        var fraudCases = ["BSNG", "Flagged Accounts", "Delay Goal"];
        matches = data.Matches.map((m, i) => buildRow(fraudCases, m, i));
    }

    var table = $("table").DataTable();

    table.clear();
    table.rows.add(matches);
    table.draw();

    $(".verified-checkbox").change(updateVerification);
}

function buildRow(fraudCases = [], rowObj, index) {
    var rowNo = index + 1;

    var verified =
        "<label class='checkbox'>"
        + "<input type= 'checkbox' class='verified-checkbox'" + (rowObj.Verified ? "checked" : "") + " data-match-id='" + rowObj.MatchId + "'/>"
        + "<span></span></label>";

    var verifiedBy = rowObj.VerifiedBy == null ? "-" : rowObj.VerifiedBy;

    var event =
        "<span class='team-home'>" + rowObj.Event.HomeName + "</span>"
        + " - vs - "
        + "<span class='team-away'>" + rowObj.Event.AwayName + "</span>"
        + "<div class='league-name'>" + rowObj.Event.LeagueName + "</div>";

    var score = rowObj.Score;

    var status = rowObj.Status;

    var note =
        "<div class='note'>"
        + "adfdaf"
        + "<span class='icon-dots-horizontal-circle icon-note'></span>"
        + "</div>";

    var row = [rowNo, verified, verifiedBy, event, score, status];

    fraudCases.map(f => buildFraudCase(f, rowObj)).map(e => row.push(e));

    row.push(note);

    return row;
}

function buildFraudCase(fraudCase, rowObj) {
    var fraud_idx = mappingFraudCase(fraudCase).toString();

    if (rowObj.FraudCases[fraud_idx] == null) {
        return "<span class='icon-close'></span>";
    }

    var fraud_case = rowObj.FraudCases[fraud_idx];

    if (fraud_case.Violations == null) {
        return '<span class="icon-close"></span>';
    }

    if (fraud_case.Violations == 0) {
        return '<span class="icon-check"></span>';
    }

    return "<div class='warning'>"
        + "<span class='icon-alert-circle'></span>"
        + "<span class='number'>" + renderViolations(fraud_case.Violations) + "</span>"
        + "</div>";
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

function renderFraudCasesHeader(fraudCases) {
    var start = $("#start");

    start.nextUntil("#end").remove();

    start.after(fraudCases.map(f => "<th class='col-status'>" + f + "</th>"));
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

        generate_btn.prop("disabled", false).addClass("-primary");
        export_btn.prop("disabled", false).addClass("-export");
    }
    else {
        league_selector.append("<option value=''>No available league</option>");

        generate_btn.prop("disabled", true).removeClass("-primary");
        export_btn.prop("disabled", true).removeClass("-export");
        $("#refresh_interval").val("--");
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
    var url = "matchmonitor/ExportExcel";
    var dto = {};

    dto.EventDate = $("#event_date").val();
    dto.IsVerified = $("#is_verified").val();
    dto.MatchStatus = $("#match_status").val();
    dto.LeagueIds = $("#league").val();
    dto.IsMainMarket = $("#is_main_market:checked").length > 0;

    common.downloadFile(url, dto);
}

$(load_page_events);