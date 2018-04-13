namespace Fanex.NAP.FS.Library.Services
{
    using Data.Repository;
    using Models;
    using System.Collections.Generic;

    public interface IDelayGoalAccountService
    {
        DBResult InsertAccounts(CriteriaBase criteria);

        IEnumerable<int> GetManualAccounts();
    }
}