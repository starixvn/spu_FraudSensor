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
        public string MatchIds { get; set; }

        public string FlaggedAccounts { get; set; }

        public IEnumerable<Match> PreCondition<Match>()
        {
            var criteria = new FlaggedAccountPreconditionCriteria();

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.Query<Match>(criteria);
            }
        }               

        public void Rule()
        {
            var criteria = new FlaggedAccountRuleCriteria()
            {
                MatchIds = this.MatchIds,
                FlaggedAccountsXML = this.FlaggedAccounts
            };

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var param = criteria.GetSpParams();
                db.ExecuteNonQuery(param);
            }
        }
    }
}
