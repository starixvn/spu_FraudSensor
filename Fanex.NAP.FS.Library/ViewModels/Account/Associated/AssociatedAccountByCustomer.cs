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

        public FraudStatus GetFraudStatus()
        {
            return this.EvidenceLevel > 0 ? FraudStatus.Fraud : FraudStatus.Clean;
        }

        public bool CompareFraudStatus(FraudStatus fraudStatus)
        {
            if (fraudStatus == FraudStatus.All)
            {
                return true;
            }

            var currentFraudStatus = GetFraudStatus();

            return currentFraudStatus.Equals(fraudStatus);
        }
    }
}