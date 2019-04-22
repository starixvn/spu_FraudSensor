namespace Fanex.NAP.FS.Library.ViewModels.MatchMonitor.FraudCaseDetails
{
    using System.Collections.Generic;

    public class ArbitrageGroupTicketList
    {
        public IEnumerable<ArbitrageTicket> Home { get; set; }

        public IEnumerable<ArbitrageTicket> Away { get; set; }

        public int BettypeId { get; set; }

        public string BettypeName { get; set; }

        public string Score { get; set; }

        public int LiveAwayScore { get; set; }

        public int LiveHomeScore { get; set; }

        public decimal Handicap { get; set; }

        public int GroupId { get; set; }
    }
}