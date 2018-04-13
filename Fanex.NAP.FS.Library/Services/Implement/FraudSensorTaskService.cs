namespace Fanex.NAP.FS.Library.Services
{
    using Core.BaseClasses;
    using Criteria.FraudSensorTask;
    using DTO;
    using System.Collections.Generic;

    public class FraudSensorTaskService : IFraudSensorTaskService
    {
        public IEnumerable<Match> BigStakeNearGoalExecutePrecondition()
        {
            return DataService.GetData<Match>(new BigStakeNearGoalPreconditionCriteria());
        }

        public void BigStakeNearGoalExecuteRule(string matchIds)
        {
            DataService.Execute(new BigStakeNearGoalRuleCriteria
            {
                MatchIds = matchIds
            });
        }

        public IEnumerable<Match> FlaggedAccountExecutePrecondition()
        {
            return DataService.GetData<Match>(new FlaggedAccountPreconditionCriteria());
        }

        public void FlaggedAccountExecuteRule(string matchIds)
        {
            DataService.Execute(new FlaggedAccountRuleCriteria
            {
                MatchIds = matchIds
            });
        }

        public IEnumerable<Match> DelayGoalExecutePrecondition()
        {
            return DataService.GetData<Match>(new DelayGoalPreconditionCriteria());
        }

        public void DelayGoalExecuteRule(string matchIds)
        {
            DataService.Execute(new DelayGoalRuleCriteria
            {
                MatchIds = matchIds
            });
        }
    }
}