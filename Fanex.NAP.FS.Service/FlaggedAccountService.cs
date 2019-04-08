namespace Fanex.NAP.FS.Service
{
    using Fanex.Data;
    using Fanex.NAP.FS.Library.Criteria.FraudSensorTask;
    using Fanex.NAP.FS.Library.DTO;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class FlaggedAccountService: IExecutorService
    {      
        public IEnumerable<Match> PreCondition<Match>()
        {
            var criteria = new FlaggedAccountPreconditionCriteria();

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.Query<Match>(criteria);
            }
        }

        public void Rule(string matchIds, string param1)
        {
            var criteria = new FlaggedAccountRuleCriteria()
            {
                MatchIds = matchIds,
                FlaggedAccountsXML = param1
            };

            using (var db = new ObjectDb(criteria.GetSettingKey()))
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
