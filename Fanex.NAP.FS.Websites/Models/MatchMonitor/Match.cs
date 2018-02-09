namespace Fanex.NAP.FS.Websites.Models.MatchMonitor
{
    public class Match
    {
        public int MatchId { get; set; }

        public string LeagueName { get; set; }

        public bool IsVerified { get; set; }

        public string VerifiedBy { get; set; }

        public string Home { get; set; }

        public string Away { get; set; }

        public string EventStatus { get; set; }

        public int LiveHomeScore { get; set; }

        public int LiveAwayScore { get; set; }
    }
}