namespace Fanex.NAP.FS.Library.Models.MatchMonitor.FraudCaseDetails
{
    using Core.BaseClasses;
    using System;
    public class ScoreHDP
    {
        public string KeyName
        {
            get
            {
                return LiveHomeScore.ToString() + " - " + LiveAwayScore.ToString()
                        + " " + CachedRef.GetBettypeName(BetType) + " " + Handicap;
            }
        }

        public string KeyValue
        {
            get
            {
                return BetType + ";" + LiveHomeScore.ToString() + ";" + LiveAwayScore.ToString() + ";" + Handicap;
            }
        }

        public int BetType { get; set; }

        public float Handicap { get; set; }

        public int LiveHomeScore { get; set; }

        public int LiveAwayScore { get; set; }

        public int TotalScore
        {
            get
            {
                return LiveHomeScore + LiveAwayScore;
            }
        }

        public int TicketCount { get; set; }
    }
}
