namespace Fanex.NAP.FS.Library.ViewModels.Account
{
    using Fanex.NAP.FS.Library.Models.Account;
    using Fanex.NAP.FS.Library.ViewModels.Account.Associated;
    using System.Collections.Generic;

    public class AccountsCollected
    {
        public IEnumerable<CustomerByGraphDB> CustomersFromAPI { get; set; }

        public IEnumerable<AssociatedAccountByCustomer> AccountsByCustomers { get; set; }

        public CustomerFraudSuspicious CustomerFraudSuspicious { get; set; }
    }
}