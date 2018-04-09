namespace Fanex.NAP.FS.Library.Services
{
    using Core;
    using Data.Repository;
    using NAP.Core.Command;
    using System.Collections.Generic;
    using System;
    using Criteria;

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
