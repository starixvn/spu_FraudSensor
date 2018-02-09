function load_page_events() {
    $("#generate").click(load_grid);
}

function load_grid() {
    var url = "matchmonitor/GetMatches";
    var dto = {};

    dto.EventDate = $("#event_date").val();
    dto.Verified = $("#is_verified").val();
    dto.MatchStatus = $("#match_status").val();
    dto.LeagueIds = $("#league").val();
    dto.IsMainMarket = $("#is_main_market").val();

    $.post(url, dto, renderGrid);
}

function renderGrid(data) {
    console.log(data);
}

$(load_page_events)