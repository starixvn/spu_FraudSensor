namespace Fanex.NAP.FS.Library.ViewModels.Account.Associated
{
    using Fanex.NAP.FS.Library.Models;
    using System;

    public class AssociatedAccountByCustomer
    {
        public int AccountID { get; set; }

        public int CustID { get; set; }
     
        public DateTime LastLogin { get; set; }

        public DateTime FirstLogin { get; set; }

        public DateTime AssociatedWithRootAccountOn { get; set; }

        public int TotalLoginWithRootAccount { get; set; }

        public int EvidenceLevel { get; set; }

        public EvidenceStatus GetEvidenceStatus()
        {
            return this.EvidenceLevel > 0 ? EvidenceStatus.Affected : EvidenceStatus.Clean;
        }

        public bool CompareEvidenceStatus(EvidenceStatus evidenceStatus)
        {
            if (evidenceStatus == EvidenceStatus.All)
            {
                return true;
            }

            var currentFraudStatus = GetEvidenceStatus();

            return currentFraudStatus.Equals(evidenceStatus);
        }
    }
}