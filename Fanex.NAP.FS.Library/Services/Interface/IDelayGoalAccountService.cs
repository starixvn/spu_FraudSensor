namespace Fanex.NAP.FS.Library.Services
{
    using Data.Repository;
    using NAP.Core.Command;
    using System.Collections.Generic;

    public interface IDelayGoalAccountService
    {
        ICommandResult InsertAccounts(ICriteria criteria);

        IEnumerable<int> GetManualAccounts();
    }
}