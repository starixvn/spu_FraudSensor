namespace Fanex.NAP.FS.Library.Services
{
    using Fanex.NAP.FS.Library.Models.Subscriber;
    using Fanex.NAP.FS.Library.ViewModels.SubscriberAccount;
    using Fanex.NAP.FS.Library.ViewModels.SubscriberAccount.Associated;
    using Fanex.NAP.FS.Library.ViewModels.SubscriberAccount.Evidence;
    using System.Collections.Generic;

    public interface IAccountService
    {
        IEnumerable<Subscriber> GetSubscribers();

        IEnumerable<SubscriberAccount> SearchSubscriberAccounts(string key, string subscribers);

        SubscriberAccountDetail GetSubscriberAccountDetail(int accountId);

        IEnumerable<AssociatedAccount> GetAssociatedAccount(int accountId);
    }
}