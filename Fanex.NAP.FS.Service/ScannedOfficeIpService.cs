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
    public class ScannedOfficeIpService : IFraudcaseService<Match>
    {
        public IEnumerable<Match> PreCondition()
        {
            var criteria = new OfficeIPPreconditionCriteria();

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.Query<Match>(criteria);
            }
        }        

        public void Rule(string matchIds)
        {
            var criteria = new OfficeIPRuleCriteria()
            {
               MatchIds = matchIds
            };

            using (var db = new ObjectDb(criteria.GetSettingKey(), 3600))
            {
                var param = criteria.GetSpParams();

                db.ExecuteNonQuery(param);
            }
        }

        public void Rule(string matchIds, string associations)
        {
            throw new NotImplementedException();
        }
    }
}
