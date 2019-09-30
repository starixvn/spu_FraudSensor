namespace Fanex.NAP.FS.Library.Services.Interface
{
    using Fanex.NAP.FS.Library.Models;
    using Fanex.NAP.FS.Library.ViewModels.Account.AssociatedToMonitor;
    using System.Collections.Generic;

    public interface IAccountService
    {
        IEnumerable<Evidence> GetAllEvidences();

        IEnumerable<AssociatedAccountsToMonitor> GetAssociatedAccountsToMonitor(AssociatedAccountsToMonitorFilter filter);

        DBResult UpdateAssociatedAccountToMonitorOfVerify(AssociatedAccountToMonitorOfVerify accountOfVerify);

        DBResult UpdateAssociatedAccountToMonitorOfNote(AssociatedAccountToMonitorOfNote accountOfNote);
    }
}
