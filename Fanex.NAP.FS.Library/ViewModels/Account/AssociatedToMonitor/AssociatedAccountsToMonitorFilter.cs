namespace Fanex.NAP.FS.Library.ViewModels.Account.AssociatedToMonitor
{
    using System;

    public class AssociatedAccountsToMonitorFilter
    {
        public DateTime AssociatedDateFrom { get; set; }

        public DateTime AssociatedDateTo { get; set; }

        public VerifyStatus VerifyStatus { get; set; }

        public string EvidenceCodes { get; set; }
    }
}