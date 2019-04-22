namespace Fanex.NAP.FS.Service
{
    using Fanex.Data;
    using Fanex.NAP.FS.Library.Criteria.FraudSensorTask;
    using System.Collections.Generic;

    public class BigStakeNearGoalService : IExecutorService
    {
        public string MatchIds { get; set; }

        public IEnumerable<Match> PreCondition<Match>()
        {
            var criteria = new BigStakeNearGoalPreconditionCriteria();

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.Query<Match>(criteria);
            }
        }

        public void Rule()
        {
            var criteria = new BigStakeNearGoalRuleCriteria()
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
