using System.Collections.Generic;

namespace Fanex.NAP.FS.Websites.ViewModels.MatchMonitor
{
    public class MatchList
    {
        public IEnumerable<Match> Matches { get; set; }

        public IEnumerable<string> FraudCases { get; set; }
    }
}