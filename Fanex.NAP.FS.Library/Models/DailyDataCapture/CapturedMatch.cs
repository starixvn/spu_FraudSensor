namespace Fanex.NAP.FS.Library.Models.DailyDataCapture
{
    using System;

    public class CapturedMatch
    {
        public int MatchId { get; set; }

        public int HomeId { get; set; }

        public int AwayId { get; set; }

        public double OldTotalCompanyTO { get; set; }

        public double NewTotalCompanyTO { get; set; }

        public double OldTotalSuperTO { get; set; }

        public double NewTotalSuperTO { get; set; }

        public double OldTotalMasterTO { get; set; }

        public double NewTotalMasterTO { get; set; }

        public double OldTotalAgentTO { get; set; }

        public double NewTotalAgentTO { get; set; }

        public int OldTotalTickets { get; set; }

        public int NewTotalTickets { get; set; }

        public int OldTotalSettledTickets { get; set; }

        public int NewTotalSettledTickets { get; set; }

        public int OldTotalCancelledTickets { get; set; }

        public int NewTotalCancelledTickets { get; set; }

        public DateTime CapturedDate { get; set; }

        public DateTime EventDate { get; set; }
    }
}