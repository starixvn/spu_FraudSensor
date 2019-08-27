namespace Fanex.NAP.FS.Library.Services
{
    using Fanex.NAP.FS.Library.Models.Subscriber;
    using Fanex.NAP.FS.Library.ViewModels.Account;
    using Fanex.NAP.FS.Library.ViewModels.Account.Associated;
    using Fanex.NAP.FS.Library.ViewModels.Account.Search;
    using System.Collections.Generic;

    public interface IAccountService
    {
        IEnumerable<SubscriberAssignment> GetSubscribers(long userId);

        IEnumerable<SubscriberAccount> SearchAccounts(SearchAccountFilter filter);

        AccountDetail GetAccountDetail(int accountId, int custId);

        IEnumerable<AssociatedAccountByAccount> GetAssociatedAccountByAccounts(AssociatedAccountByAccountFilter filter);

        IEnumerable<AssociatedDeviceByAccount> GetAssociatedDevicesByAccount(AssociatedDeviceByAccountFilter filter);        
    }
}