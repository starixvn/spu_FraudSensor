namespace Fanex.NAP.FS.Model
{
    using System;

    public class FraudCaseHealth
    {
        public int FraudCaseId { get; set; }

        public bool IsLive { get; set; }

        public int TimeOut { get; set; }

        public int LetstalkCurrentTimes { get; set; }

        public bool IsActive { get; set; }

        public DateTime LastTimeSignal { get; set; }

        public bool IsOkNow { get; set; }
    }
}
