namespace Fanex.NAP.FS.Library.Services
{
    using Core;
    using Criteria;
    using DTO;
    using System.Collections.Generic;

    public class FraudSensorTaskService : FSBaseService, IFraudSensorTaskService
    {
        public IEnumerable<Match> BigStakeNearGoalExecutePrecondition()
        {
            return GetData<Match>(new BigStakeNearGoalPreconditionCriteria());
        }

        public void BigStakeNearGoalExecuteRule(string matchIds)
        {
            Execute(new BigStakeNearGoalRuleCriteria
            {
                MatchIds = matchIds
            });
        }

        public IEnumerable<Match>FlaggedAccountExecutePrecondition()
        {
            return GetData<Match>(new FlaggedAccountPreconditionCriteria());
        }

        public void FlaggedAccountExecuteRule(string matchIds)
        {
            Execute(new FlaggedAccountRuleCriteria
            {
                MatchIds = matchIds
            });
        }
    }
}
