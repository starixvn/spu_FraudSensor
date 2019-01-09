namespace Fanex.NAP.FS.Library.ViewModels.DailyDataMonitor
{
    public class CapturedMatch
    {
        public int MatchId { get; set; }

        public string MatchCode { get; set; }

        public string League { get; set; }

        public string Home { get; set; }

        public string Away { get; set; }

        public string EventDate { get; set; }

        public string ScannedDate { get; set; }

        public History<double?> CompanyTO { get; set; }

        public History<double?> SuperTO { get; set; }

        public History<double?> MasterTO { get; set; }

        public History<double?> AgentTO { get; set; }

        public History<int?> Tickets { get; set; }

        public History<int?> SettledTickets { get; set; }

        public History<int?> CancelledTickets { get; set; }
    }
}