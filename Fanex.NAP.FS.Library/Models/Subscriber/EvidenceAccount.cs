namespace Fanex.NAP.FS.Library.Models.Subscriber
{
    using Fanex.NAP.FS.Library.Models.MatchMonitor.FraudCaseDetails;
    using Fanex.NAP.FS.Library.Models.Account;
    using System;

    public class EvidenceAccount
    {
        public int Id { get; set; }

        public Account Account { get; set; }

        public Evidence Evidence { get; set; }

        public string NodeRoot { get; set; }        

        public DateTime AffectedDate { get; set; }

        public string Comment { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}