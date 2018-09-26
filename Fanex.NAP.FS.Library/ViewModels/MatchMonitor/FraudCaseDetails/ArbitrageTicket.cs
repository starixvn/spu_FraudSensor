namespace Fanex.NAP.FS.Library.ViewModels.MatchMonitor.FraudCaseDetails
{
    using System;
    using System.Collections.Generic;

    public class ArbitrageTicket
    {
        public string Username { get; set; }

        public IEnumerable<string> Evidences { get; set; }

        public int DangerousLevel { get; set; }

        public string Currency { get; set; }

        public long BetId { get; set; }

        public decimal Odds { get; set; }

        public ArbitrageStake Stake { get; set; }

        public DateTime RawTransDate { get; set; }

        public string TransDate => RawTransDate.ToString("MMM dd, yyyy <br/> hh:mm:ss tt");

        public string BetTeam { get; set; }

        public string BettypeName { get; set; }

        public double Handicap { get; set; }

        public string Score { get; set; }
    }
}