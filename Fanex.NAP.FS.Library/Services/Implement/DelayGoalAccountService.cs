namespace Fanex.NAP.FS.Library.Services
{
    using Core;
    using Criteria;
    using Data.Repository;
    using NAP.Core.Command;
    using System.Collections.Generic;

    public class DelayGoalAccountService : FSBaseService, IDelayGoalAccountService
    {
        public ICommandResult InsertAccounts(ICriteria criteria)
        {
            return UpdateData(criteria);
        }

        public IEnumerable<int> GetManualAccounts()
        {
            return GetData<int>(new DelayGoalManualAccountGetCriteria());
        }
    }
}