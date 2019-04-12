using Fanex.Data;
using Fanex.NAP.FS.Library.Criteria.FraudSensorTask;
using System.Collections.Generic;

namespace Fanex.NAP.FS.Service
{
    public class DelayGoalService : IExecutorService
    {
        public string MatchIds { get; set; }

        public IEnumerable<Match> PreCondition<Match>()
        {
            var criteria = new DelayGoalPreconditionCriteria();

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.Query<Match>(criteria);
            }
        }

        public void Rule()
        {
            var criteria = new DelayGoalRuleCriteria()
            {
                MatchIds = this.MatchIds
            };

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var param = criteria.GetSpParams();

                db.ExecuteNonQuery(param);
            }
        }
    }
}
