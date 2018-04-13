namespace Fanex.NAP.FS.Library.Services
{
    using Core.BaseClasses;
    using Criteria.FraudCase.DelayedGoal;
    using Data.Repository;
    using Models;
    using System.Collections.Generic;

    public class DelayGoalAccountService : IDelayGoalAccountService
    {
        public DBResult InsertAccounts(CriteriaBase criteria)
        {
            return DataService.UpdateData(criteria, "Insert Delay Goal Account");
        }

        public IEnumerable<int> GetManualAccounts()
        {
            return DataService.GetData<int>(new DelayGoalManualAccountGetCriteria());
        }
    }
}