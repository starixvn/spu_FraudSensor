namespace Fanex.NAP.FS.Library.ViewModels.Account.GeneralInformation
{
    using System;

    public class CustomerInformation
    {
        public int AccountID { get; set; }

        public int CustID { get; set; }

        public int AccountStatus { get; set; }

        public string FirstLoginName { get; set; }

        public string Username { get; set; }

        public string Username2 { get; set; }

        public int CurrencyID { get; set; }

        public string CurrencyName { get; set; }

        public int SubscriberID { get; set; }

        public int RoleID { get; set; }

        public int IsDeposit { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime LastLoginDate { get; set; }

        public int LoginCount { get; set; }

        public int AccountAge { get; set; }
    }
}