namespace Fanex.NAP.FS.Library.Services.Interface
{
    using Fanex.NAP.FS.Library.ViewModels.AssociatedAccountMonitor;
    using System.Collections.Generic;

    public interface IAssociatedAccountsMonitorService
    {
        IEnumerable<Subscriber> GetAllSubscribers();

        IEnumerable<Evidence> GetAllEvidences();

        IEnumerable<AssociatedAccountsMonitor> GetAssociatedAccountsToMonitor(AssociatedAccountsMonitorFilter filter);

        IEnumerable<AssociatedAccountsDetailMonitor> GetAssociatedAccountsDetailToMonitor(AssociatedAccountsDetailMonitorFilter filter);

        UpdateResult VerifyAssociatedAccountMonitor(AssociatedAccountMonitorVerify account);

        UpdateResult UpdateNoteAssociatedAccountMonitor(AssociatedAccountMonitorNote account);
    }
}