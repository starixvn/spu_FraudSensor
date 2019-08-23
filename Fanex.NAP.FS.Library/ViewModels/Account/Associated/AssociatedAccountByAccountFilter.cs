namespace Fanex.NAP.FS.Library.ViewModels.Account.Associated
{
    using Fanex.NAP.FS.Library.Models;

    public class AssociatedAccountByAccountFilter
    {
        public int AccountId { get; set; }

        public int CustId { get; set; }

        public bool DeviceAwayOne { get; set; }       

        public AccountStatus Status { get; set; }

        public FraudStatus FraudStatus { get; set; }
    }
}