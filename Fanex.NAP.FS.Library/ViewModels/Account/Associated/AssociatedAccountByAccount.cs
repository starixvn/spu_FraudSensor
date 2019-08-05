namespace Fanex.NAP.FS.Library.ViewModels.Account.Associated
{
    using Fanex.NAP.FS.Library.Models.Account;
    using System;

    public class AssociatedAccountByAccount
    {
        public Account AccountAssociated { get; set; }

        public AccountInfo AccountInfoAssociated { get; set; }

        public string Status { get; set; }

        public int TotalSharedDeviceLogin { get; set; }

        public DateTime AssociatedDate { get; set; }

        public string GraphURL { get; set; }

    }
}