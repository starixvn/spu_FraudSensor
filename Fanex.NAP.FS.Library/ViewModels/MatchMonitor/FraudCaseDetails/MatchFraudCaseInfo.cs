namespace Fanex.NAP.FS.Library.ViewModels.MatchMonitor.FraudCaseDetails
{
    using System;

    public class MatchFraudCaseInfo
    {
        public int FraudCaseId { get; set; }

        public int MatchId { get; set; }

        public string Event { get; set; }

        public string FraudCase { get; set; }

        public string HTScore { get; set; }

        public string FTScore { get; set; }

        public int HomeId { get; set; }

        public int AwayId { get; set; }

        public DateTime EventDate { get; set; }

        public dynamic Away { get; set; }

        public dynamic Home { get; set; }

        public dynamic Filter { get; set; }
    }
}