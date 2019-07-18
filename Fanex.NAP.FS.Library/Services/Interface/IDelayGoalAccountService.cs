namespace Fanex.NAP.FS.Library.Services
{
    using Data.Repository;
    using Fanex.NAP.FS.Library.Core.BaseClasses;
    using Models;
    using System.Collections.Generic;

    public interface IDelayGoalAccountService
    {
        DBResult InsertAccounts(CommandBase criteria);

        IEnumerable<int> GetManualAccounts();
    }
}