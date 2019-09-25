using System;

namespace Fanex.NAP.FS.Library.ViewModels.Account.Associated
{
    public class AssociatedAccountsFilter
    {
        public DateTime AssociatedDateFrom { get; set; }

        public DateTime AssociatedDateTo { get; set; }

        public VerifyStatus VerifyStatus { get; set; }

        public string EvidenceCodes { get; set; }
    }
}