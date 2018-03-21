using Fanex.NAP.FS.Websites.Util.WebRendering;
using System.Collections.Generic;

namespace Fanex.NAP.FS.Websites.ViewModels.MatchMonitor
{
    public class Match
    {
        public static object Columns { get; }

        [Ignore]
        public int MatchId { get; set; }

        public bool Verified { get; set; }

        [Column("Last Verified By")]
        public string VerifiedBy { get; set; }

        public Event Event { get; set; }

        public string Score { get; set; }

        public string Status { get; set; }

        [Ignore]
        public Dictionary<string, FraudCase> FraudCases { get; set; }

        [Column("Note")]
        public string Notes { get; set; }

        static Match()
        {
            Columns = ColumnAttribute.GetColumnJson<Match>();
        }
    }
}