var timeout_holder = null;
var data_holder = null;
var pageSize = 0;

function load_page_events() {
    $("#generate").click(load_grid);

    getLeague();

    $("#event_date")
        .datepicker({
            dateFormat: "yy-mm-dd",
            minDate: balanceUpDay
        })
        .change(getLeague);

    $(".icon-calendar-text").click(() => $("#event_date").focus());

    $("#refresh_interval").change(e => refreshChange($(e.target)));

    pageSize = parseInt($("#pager").val());

    $("#pager").change(function () {
        pageSize = $(this).val();
        page();
        var paged_data = sliceData(0, pageSize);
        renderGrid(paged_data);
    });

    $("#page_index").change(function () {
        var page_idx = parseInt($(this).val());

        var paged_data = sliceData(page_idx, pageSize);
        renderGrid(paged_data);
    })

    $("#back").click(function () {
        var indexer = $("#page_index");

        var page_idx = parseInt(indexer.val());

        var new_page_idx = Math.max(page_idx - 1, 0);

        indexer.val(new_page_idx);

        if (new_page_idx == 0) {
            $(this).parent.addClass("disabled")
        }
    });

    $("#next").click(function () {
        var indexer = $("#page_index");

        var page_idx = parseInt(indexer.val());

        var max_page = data_holder.length / pageSize + Math.min(data_holder.length % pageSize, 1);

        var new_page_idx = Math.min(page_idx + 1, max_page);

        indexer.val(new_page_idx);

        $("#back").parent.removeClass("disabled");

        if (new_page_idx == max_page) {
            $(this).parent.addClass("disabled");
        }
    })
}

function load_grid() {
    var url = "matchmonitor/GetMatches";
    var dto = {};

    dto.EventDate = $("#event_date").val();
    dto.IsVerified = $("#is_verified").val();
    dto.MatchStatus = $("#match_status").val();
    dto.LeagueIds = $("#league").val();
    dto.IsMainMarket = $("#is_main_market:checked").length > 0;

    $.post(url, dto, function (data) {
        data_holder = data;
        page()
        var paged_data = sliceData(0, pageSize);
        renderGrid(paged_data);
    });
}

function renderGrid(data) {
    if (data != null && data.Matches != null && data.Matches.length > 0) {
        renderFraudCasesHeader(data.FraudCases);
        $("tbody").empty().append(data.Matches.map((m, i) => buildRow(data.FraudCases, m, data.PageIdx * pageSize + i)));
        $("#table_summary").text("Displaying "+ (data.PageIdx * pageSize + 1) + " to "+ (data.PageIdx * pageSize + pageSize) + " of " +  (data_holder.Matches.length)  + "item(s)")
    }
    else {
        renderFraudCasesHeader([]);
        $("tbody").empty().append("<tr class='text-center'><td  colspan='6'>There's no data</td></tr>");
        $("#table_summary").text("");
        $("#back, #next").parent.addClass("disabled");
    }

    $("#match_grid").show();

    $(".verified-checkbox").change(updateVerification);
}

function buildRow(fraudCases, rowObj, index) {
    var row = $("<tr></tr>");

    // render row no
    row.append("<td class='text-center'>" + (index + 1) + "</td>");

    // render checkbox
    row.append(
        "<td class='text-center'> "
        + "<label class='checkbox'>"
        + "<input type= 'checkbox' class='verified-checkbox'" + (rowObj.Verified ? "checked" : "") + " data-match-id='" + rowObj.MatchId + "'/>"
        + "<span></span></label></td>"
    );

    // render verified by
    row.append("<td>" + (rowObj.VerifiedBy == null ? "-" : rowObj.VerifiedBy) + "</td>");

    // render event
    row.append(
        "<td>"
        + "<span class='team-home'>" + rowObj.Event.HomeName + "</span>"
        + " vs "
        + "<span class='team-away'>" + rowObj.Event.AwayName + "</span>"
        + "<div class='league-name'>" + rowObj.Event.LeagueName + "</div>"
        + "</td>");

    //render score
    row.append("<td class='text-center'>" + rowObj.Score + "</td>");

    //render status
    row.append("<td class='text-center'>" + rowObj.Status + "</td>");

    row.append(fraudCases.map(f => buildFraudCase(f, rowObj)));

    return row;
}

function buildFraudCase(fraudCase, rowObj) {
    if (rowObj.FraudCases[fraudCase] == null) {
        return "<td class='has-icon text-center'><span class='icon-close'></span></td>";
    }

    var fraud_case = rowObj.FraudCases[fraudCase];

    if (fraud_case.Violations == null) {
        return '<td class="has-icon text-center"><span class="icon-timer-sand"></span></td>';
    }

    if (fraud_case.Violations == 0) {
        return '<td class="has-icon text-center"><span class="icon-check"></span></td>';
    }

    return "<td class='has-icon text-center'>"
        + "<div class='warning'>"
        + "<span class='icon-alert-circle'></span>"
        + "<span class='number'>" + fraud_case.Violations + "</span>"
        + "</div>"
        + "</td>";
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
            $target.next().text("-");
        }
    });
}

function sliceData(page = 0, pageSize = 50) {
    if (data_holder == null) {
        return null;
    }

    let sliced_data =
        {
            FraudCases: data_holder.FraudCases,
            Matches: data_holder.Matches.slice(page, page + pageSize),
            PageIdx: page
        };

    return sliced_data;
}

function page() {
    if (data_holder != null) {
        var n = parseInt(data_holder.Matches.length / pageSize) + Math.min(data_holder.Matches.length % pageSize, 1);

        var indexer = $("#page_index").empty();

        for (var i = 0; i < n; i++) {
            indexer.append("<option value='" + i + "'>" + (i + 1) + "</option>");
        }
    }
}

$(load_page_events);