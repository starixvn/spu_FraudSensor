namespace Fanex.NAP.FS.Library.Services.Interface
{
    using Fanex.NAP.FS.Library.Models;
    using Fanex.NAP.FS.Library.ViewModels.Account.AssociatedToMonitor;
    using System.Collections.Generic;

    public interface IAccountService
    {
        IEnumerable<Subscriber> GetAllSubscribers();

        IEnumerable<Evidence> GetAllEvidences();

        IEnumerable<AssociatedAccountsToMonitor> GetAssociatedAccountsToMonitor(AssociatedAccountsToMonitorFilter filter);

        IEnumerable<AssociatedAccountsDetailToMonitor> GetAssociatedAccountsDetailToMonitor(AssociatedAccountsDetailToMonitorFilter filter);

        UpdateResult UpdateAssociatedAccountToMonitorOfVerify(AssociatedAccountToMonitorOfVerify accountOfVerify);

        UpdateResult UpdateAssociatedAccountToMonitorOfNote(AssociatedAccountToMonitorOfNote accountOfNote);
    }
}