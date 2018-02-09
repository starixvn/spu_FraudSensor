namespace Fanex.NAP.FS.Websites.Models.MatchMonitor
{
    public class FraudCase
    {
        public int MatchId { get; set; }

        public int FraudCaseId { get; set; }

        public string FraudCaseName { get; set; }

        public int? SuspiciousTickets { get; set; }
    }
}