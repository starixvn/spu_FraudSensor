namespace Fanex.NAP.FS.Library.Models.MatchMonitor.FraudCaseDetails
{
    using System;
    public class ScoreHDP
    {
        public int BetType { get; set; }

        public string BetTypeName
        {
            get
            {
                string name = string.Empty;
                name = LiveHomeScore.ToString() + " - " + LiveAwayScore.ToString();
                name += " Handicap " + Handicap;
                return name;
            }
        }

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
    }
}
