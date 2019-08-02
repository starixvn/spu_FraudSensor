namespace Fanex.NAP.FS.Library.Models.Account
{
    using Fanex.NAP.FS.Library.Models.MatchMonitor.FraudCaseDetails;    
    using System;

    public class AccountEvidence
    {
        public int Id { get; set; }

        public Account Account { get; set; }

        public Evidence Evidence { get; set; }

        public int NodeIdRoot { get; set; }

        public string NodeNameRoot { get; set; }        

        public DateTime AffectedDate { get; set; }

        public string Comment { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}