namespace Fanex.NAP.FS.Service
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NAP.FS.Library.Criteria.FraudSensorTask;
    using System.Collections.Generic;

    public class BetsNearGoalService
    {
        public IEnumerable<Match> PreCondition<Match>()
        {
            var criteria = new BetsNearGoalPreconditionCriteria();

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.Query<Match>(criteria);
            }
        }       
  
        public void Rule(string matchIds)
        {
            var repository = new DynamicRepository();

            var criteria = new BetsNearGoalRuleCommand()
            {
                MatchIds = matchIds
            };

            repository.Execute(criteria);
        }
    }
}
