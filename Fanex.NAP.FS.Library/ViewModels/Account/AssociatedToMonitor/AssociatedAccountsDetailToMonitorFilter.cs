namespace Fanex.NAP.FS.Library.ViewModels.Account.AssociatedToMonitor
{
    using System;

    public class AssociatedAccountsDetailToMonitorFilter
    {
        public DateTime AssociatedDateFrom { get; set; }

        public DateTime AssociatedDateTo { get; set; }

        public int MainCustId { get; set; }

        public string Sites { get; set; }

        public string CustLevels { get; set; }

        public string EvidenceCodes { get; set; }

        public VerifyStatus VerifyStatus { get; set; }
    }
}
