using Fanex.Data;
using Fanex.NAP.FS.Library.Criteria.FraudSensorTask;
using Fanex.NAP.FS.Library.Models.FraudCaseTask;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.FS.Service
{
    public class ArbitrageService: IExecutorService
    {
        public string MatchIds { get; set; }

        public string Associations { get; set; }

        public IEnumerable<ArbitrageTicket> PreCondition<ArbitrageTicket>()
        {
            var criteria = new ArbitragePreconditionCriteria();

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.Query<ArbitrageTicket>(criteria);
            }
        }       
      
        public void Rule()
        {
            var criteria = new ArbitrageRuleCriteria()
            {
                Matches = this.MatchIds,
                Associations = this.Associations
            };

            using (var db = new ObjectDb(criteria.GetSettingKey(), 3600))
            {
                var param = criteria.GetSpParams();

                db.ExecuteNonQuery(param);
            }
        }
    }
}
