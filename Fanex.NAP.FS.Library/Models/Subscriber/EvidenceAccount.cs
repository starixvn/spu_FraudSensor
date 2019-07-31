namespace Fanex.NAP.FS.Library.Models.Subscriber
{
    using Fanex.NAP.FS.Library.Models.MatchMonitor.FraudCaseDetails;
    using System;

    public class EvidenceAccount
    {
        public int Id { get; set; }

        public Evidence EvidenceKey { get; set; }

        public string CentralPoint { get; set; }        

        public DateTime AffectedDate { get; set; }

        public string Comment { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}