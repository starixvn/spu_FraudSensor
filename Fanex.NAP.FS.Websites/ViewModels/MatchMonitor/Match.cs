using System.Collections.Generic;

namespace Fanex.NAP.FS.Websites.ViewModels.MatchMonitor
{
    public class Match
    {
        public static object Columns { get; }

        public int MatchId { get; set; }

        public bool Verified { get; set; }
        
        public string VerifiedBy { get; set; }

        public Event Event { get; set; }

        public string Score { get; set; }

        public string Status { get; set; }
        
        public Dictionary<string, FraudCase> FraudCases { get; set; }

        public string Notes { get; set; }
    }
}