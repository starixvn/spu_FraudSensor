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
        public IEnumerable<ArbitrageTicket> PreCondition<ArbitrageTicket>()
        {
            var criteria = new ArbitragePreconditionCriteria();

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.Query<ArbitrageTicket>(criteria);
            }
        }        

        public void Rule(string matchIds, string param1)
        {
            var criteria = new ArbitrageRuleCriteria()
            {
                Matches = matchIds,
                Associations = param1
            };

            using (var db = new ObjectDb(criteria.GetSettingKey(), 3600))
            {
                var param = criteria.GetSpParams();

                db.ExecuteNonQuery(param);
            }
        }

        public void Rule(string matchIds)
        {
            throw new NotImplementedException();
        }

        public void Rule()
        {
            throw new NotImplementedException();
        }
    }
}
