using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fanex.NAP.FS.Websites.ViewModels.MatchMonitor.FraudCaseDetails
{
    public class MatchFraudCaseInfo
    {
        public int FraudCaseId { get; set; }

        public int MatchId { get; set; }

        public string Event { get; set; }

        public string FraudCase { get; set; }

        public string HTScore { get; set; }

        public string FTScore { get; set; }
    }
}