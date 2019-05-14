using Fanex.Data.Repository;

namespace Fanex.NAP.FS.Service
{
    using Fanex.Data;
    using Fanex.NAP.FS.Library.Criteria.FraudSensorTask;
    using System.Collections.Generic;

    public class FlaggedAccountService
    {
        public IEnumerable<Match> PreCondition<Match>()
        {
            var criteria = new FlaggedAccountPreconditionCriteria();

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.Query<Match>(criteria);
            }
        }               

        public void Rule(string matchIds, string flaggedAccounts)
        {
            var repository = new DynamicRepository();

            var criteria = new FlaggedAccountRuleCommand()
            {
                MatchIds = matchIds,
                FlaggedAccountsXML = flaggedAccounts
            };

            repository.Execute(criteria);
        }
    }
}
