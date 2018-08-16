namespace Fanex.NAP.FS.Library.ViewModels.MatchMonitor.FraudCaseDetails
{
    using System.Collections.Generic;

    public class ArbitrageTicket
    {
        public string Username { get; set; }

        public IEnumerable<string> Evidences { get; set; }

        public int DangerousLevel { get; set; }

        public string Currency { get; set; }

        public long BetId { get; set; }

        public double Odds { get; set; }

        public ArbitrageStake Stake { get; set; }

        public string TransDate { get; set; }
    }
}