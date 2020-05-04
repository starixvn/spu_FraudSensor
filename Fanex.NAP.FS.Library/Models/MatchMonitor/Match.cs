namespace Fanex.NAP.FS.Library.Models.MatchMonitor
{
    public class Match
    {
        public int MatchId { get; set; }

        public string MatchCode { get; set; }

        public string LeagueName { get; set; }

        public bool IsVerified { get; set; }

        public int? VerifiedBy { get; set; }

        public int HomeId { get; set; }

        public int AwayId { get; set; }

        public string EventStatus { get; set; }

        public int LiveHomeScore { get; set; }

        public int LiveAwayScore { get; set; }

        public string Notes { get; set; }
    }
}