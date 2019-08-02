namespace Fanex.NAP.FS.Library.Services
{
    using Fanex.NAP.FS.Library.Models.Subscriber;
    using Fanex.NAP.FS.Library.ViewModels.Account;
    using Fanex.NAP.FS.Library.ViewModels.Account.Associated;
    using System.Collections.Generic;

    public interface IAccountService
    {
        IEnumerable<Subscriber> GetSubscribers();

        IEnumerable<SubscriberAccount> SearchSubscriberAccounts(string key, string subscribers);

        AccountDetail GetAccountDetail(int accountId);

        IEnumerable<AssociatedAccountByAccount> GetAssociatedAccountByAccounts(AssociatedAccountByAccountFilter filter);

        IEnumerable<AssociatedAccountByDevice> GetAssociatedAccountByDevices(AssociatedAccountByDeviceFilter filter);
    }
}