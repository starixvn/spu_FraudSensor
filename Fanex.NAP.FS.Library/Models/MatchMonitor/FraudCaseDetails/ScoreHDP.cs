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
                string name = string.Empty;
                name = LiveHomeScore.ToString() + " - " + LiveAwayScore.ToString();
                name += " " + CachedRef.GetBettypeName(BetType) + " " + Handicap;
                return name;
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
