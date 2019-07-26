namespace Fanex.NAP.FS.Library.Services
{
    using Fanex.NAP.FS.Library.Models.Subscriber;
    using Fanex.NAP.FS.Library.ViewModels.SubscriberAccount;
    using System.Collections.Generic;

    public interface ISearchAccountService
    {
        IEnumerable<Subscriber> GetSubscribers();

        IEnumerable<SubscriberAccount> SearchSubscriberAccounts(string key, string subscribers);

        SubscriberAccountDetail GetSubscriberAccountDetail(int accountID);
    }
}