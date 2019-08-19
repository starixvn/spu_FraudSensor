namespace Fanex.NAP.FS.Library.ViewModels.MatchMonitor
{
    using System;

    public class MatchFilter
    {
        public string Sites { get; set; }

        public DateTime EventDate { get; set; }

        public bool? IsVerified { get; set; }

        public string MatchStatus { get; set; }

        public string LeagueIds { get; set; }

        public bool IsMainMarket { get; set; }

        public bool IsShowSpecialCurrencies { get; set; }
    }
}