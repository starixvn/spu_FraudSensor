namespace Fanex.NAP.FS.Library.ViewModels.Account.Associated
{
    using System;   

    public class AssociatedAccountByAccount
    {
        public int AccountID { get; set; }

        public int CustID { get; set; }

        public string Username { get; set; }

        public string Username2 { get; set; }

        public DateTime LastLogin { get; set; }

        public DateTime FirstLogin { get; set; }       

        public DateTime AssociatedWithRootAccountOn { get; set; }

        public int TotalLoginWithRootAccount { get; set; }

        public int EvidenceLevel { get; set; }

        public string Status { get; set; }
    }
}