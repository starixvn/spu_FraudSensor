using Fanex.Data.Repository;

namespace Fanex.NAP.FS.Service
{
    using Fanex.Data;
    using Fanex.NAP.FS.Library.Criteria.FraudSensorTask;
    using System.Collections.Generic;

    public class DelayGoalService
    {
        public IEnumerable<Match> PreCondition<Match>()
        {
            var criteria = new DelayGoalPreconditionCriteria();

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.Query<Match>(criteria);
            }
        }

        public void Rule(string matchIds)
        {
            var repository = new DynamicRepository();
            var criteria = new DelayGoalRuleCommand()
            {
                MatchIds = matchIds
            };

            repository.Execute(criteria);
        }
    }
}
