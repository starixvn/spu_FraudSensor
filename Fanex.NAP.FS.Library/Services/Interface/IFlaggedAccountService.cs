namespace Fanex.NAP.FS.Library.Services
{
    using Data.Repository;
    using NAP.Core.Command;
    using System.Collections.Generic;

    public interface IFlaggedAccountService
    {
        ICommandResult InsertAccounts(ICriteria criteria);

        ICommandResult DeleteAccounts(ICriteria criteria);
    }
}
