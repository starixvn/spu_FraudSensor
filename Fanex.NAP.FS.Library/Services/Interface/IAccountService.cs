namespace Fanex.NAP.FS.Library.Services.Interface
{
    using Fanex.NAP.FS.Library.Models;
    using Fanex.NAP.FS.Library.ViewModels.Account.Associated;
    using System.Collections.Generic;

    public interface IAccountService
    {
        IEnumerable<Evidence> GetAllEvidences();

        IEnumerable<AssociatedAccounts> GetAssociatedAccounts(AssociatedAccountsFilter filter);

        DBResult UpdateAssociatedAccountOfVerify(AssociatedAccountOfVerify accountOfVerify);

        DBResult UpdateAssociatedAccountOfNote(AssociatedAccountOfNote accountOfNote);
    }
}
