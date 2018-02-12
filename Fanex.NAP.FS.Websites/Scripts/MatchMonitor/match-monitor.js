function load_page_events() {
    $("#generate").click(load_grid);
    getLeague();
    $("#event_date").change(getLeague);
}

function load_grid() {
    var url = "matchmonitor/GetMatches";
    var dto = {};

    dto.EventDate = $("#event_date").val();
    dto.Verified = $("#is_verified").val();
    dto.MatchStatus = $("#match_status").val();
    dto.LeagueIds = $("#league").val();
    dto.IsMainMarket = $("#is_main_market:checked").length > 0;

    $.post(url, dto, renderGrid);
}

function renderGrid(data) {
    var options =
        {
            headerCallback: renderHeader,
            rowCallback: renderRow
        };

    $("#match-grid").autoGrid(columns, options, data);
}

function renderRow(rowObj, rowArray) {
    console.log(rowObj);
}

function renderHeader(headerObj, headerArray) {
    console.log(headerObj);
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

    if (data.length != 0) {
        var all_league = data.map(l => l.LeagueId).join();

        league_selector.append("<option value='" + all_league + "'>All League</option>");

        for (var i = 0; i < data.length; i++) {
            league_selector.append("<option value='" + data[i].LeagueId + "'>" + data[i].LeagueName + "</option>");
        }

        generate_btn.prop("disabled", false);
    }
    else {
        league_selector.append("<option>No available league</option>");
        generate_btn.prop("disabled", true);
    }
}

$(load_page_events);