namespace Fanex.NAP.FS.Service
{
    using Fanex.Data;
    using Fanex.NAP.FS.Library.Criteria.FraudSensorTask;
    using System.Collections.Generic;

    public class BetsNearGoalService
    {
        public string MatchIds { get; set; }

        public IEnumerable<Match> PreCondition<Match>()
        {
            var criteria = new BetsNearGoalPreconditionCriteria();

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.Query<Match>(criteria);
            }
        }       
  
        public void Rule()
        {
            var criteria = new BetsNearGoalRuleCriteria()
            {
                MatchIds = this.MatchIds
            };

            using (var db = new ObjectDb(criteria.GetSettingKey(), 3600))
            {
                var param = criteria.GetSpParams();

                db.ExecuteNonQuery(param);
            }
        }
    }
}
