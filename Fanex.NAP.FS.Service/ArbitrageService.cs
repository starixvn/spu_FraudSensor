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
    public class ArbitrageService
    {
        public IEnumerable<ArbitrageTicket> PreCondition()
        {
            var criteria = new ArbitragePreconditionCriteria();

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
               // return null;
                return db.Query<ArbitrageTicket>(criteria);
            }
        }

        public void Rule(string matchIds, string associations)
        {
            var criteria = new ArbitrageRuleCriteria()
            {
                Matches = matchIds,
                Associations = associations
            };

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var param = criteria.GetSpParams();

                db.ExecuteNonQuery(param);
            }
        }
    }
}
