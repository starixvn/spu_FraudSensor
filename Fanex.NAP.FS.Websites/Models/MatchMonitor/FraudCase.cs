namespace Fanex.NAP.FS.Websites.Models.MatchMonitor
{
    public class FraudCase
    {
        public int MatchId { get; set; }

        public int FraudCaseId { get; set; }

        public string FraudCaseName { get; set; }

        public int? SuspiciousTicketsCount { get; set; }
    }
}