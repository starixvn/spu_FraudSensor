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
    public class FlaggedAccountService
    {
        public IEnumerable<Match> ExecutePrecondition()
        {
            var criteria = new FlaggedAccountPreconditionCriteria();

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.Query<Match>(criteria);
            }
        }

        public void ExecuteRule(string matchIds, string flaggedAccounts)
        {
            var criteria = new FlaggedAccountRuleCriteria()
            {
                MatchIds = matchIds,
                FlaggedAccountsXML = flaggedAccounts
            };

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var param = criteria.GetSpParams();
                db.ExecuteNonQuery(param);
            }
        }
    }
}
