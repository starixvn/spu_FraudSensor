namespace Fanex.NAP.FS.Library.Models.MatchMonitor.FraudCaseDetails
{
    using System;

    internal class MatchInfo
    {
        public DateTime EventDate { get; set; }

        public int HomeId { get; set; }

        public int AwayId { get; set; }

        public int? HTHomeScore { get; set; }

        public int? HTAwayScore { get; set; }

        public int? FinalHomeScore { get; set; }

        public int? FinalAwayScore { get; set; }
    }
}