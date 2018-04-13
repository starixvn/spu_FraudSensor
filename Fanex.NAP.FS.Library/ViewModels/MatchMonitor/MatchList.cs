namespace Fanex.NAP.FS.Library.ViewModels.MatchMonitor
{
    using System.Collections.Generic;

    public class MatchList
    {
        public IEnumerable<Match> Matches { get; set; }

        public IEnumerable<string> FraudCases { get; set; }
    }
}