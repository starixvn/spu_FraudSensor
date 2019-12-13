namespace Fanex.NAP.FS.Library.Services.Interface
{
    using Fanex.NAP.FS.Library.DTO;
    using Fanex.NAP.FS.Library.ViewModels.AssociatedAccountMonitor;
    using System.Collections.Generic;

    public interface IAssociatedAccountsMonitorService
    {
        IEnumerable<Subscriber> GetAllSubscribers();

        IEnumerable<Evidence> GetAllEvidences();

        IEnumerable<AssociatedAccountsMonitor> GetAssociatedAccountsMonitor(AssociatedAccountsMonitorFilter filter);

        IEnumerable<CustomerAssociation> GetAllAssociatedAccounts(AllAssociatedAccountsFilter filter);

        IEnumerable<AssociatedAccountsMonitorDetail> GetAssociatedAccountsMonitorDetail(AssociatedAccountsMonitorDetailFilter filter);

        UpdateResult VerifyAssociatedAccountMonitor(AssociatedAccountMonitorVerify account);

        UpdateResult UpdateNoteAssociatedAccountMonitor(AssociatedAccountMonitorNote account);
    }
}