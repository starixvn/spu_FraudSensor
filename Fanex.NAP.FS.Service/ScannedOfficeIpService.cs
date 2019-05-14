

namespace Fanex.NAP.FS.Service
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NAP.FS.Library.Criteria.FraudSensorTask;
    using System.Collections.Generic;

    public class ScannedOfficeIpService
    {
        public IEnumerable<Match> PreCondition<Match>()
        {
            var criteria = new OfficeIPPreconditionCriteria();

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.Query<Match>(criteria);
            }
        }        

        public void Rule(string matchIds)
        {
            var repository = new DynamicRepository();

            var criteria = new OfficeIPRuleCommand()
            {
               MatchIds = matchIds
            };

            repository.Execute(criteria);
        }       
    }
}
