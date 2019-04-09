using Fanex.Data;
using Fanex.NAP.FS.Library.Criteria.FraudSensorTask;
using Fanex.NAP.FS.Library.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.FS.Service
{
    public class BetsNearGoalService : IExecutorService
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
