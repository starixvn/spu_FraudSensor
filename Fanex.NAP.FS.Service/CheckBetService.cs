namespace Fanex.NAP.FS.Service
{
    using Fanex.Data;
    using Fanex.NAP.FS.Library.Criteria.FraudSensorTask;
    using System.Collections.Generic;

    public class CheckBetService : IExecutorService
    {
        public string MatchIds { get; set; }

        public IEnumerable<Match> PreCondition<Match>()
        {
            return null;
        }
  
        public void Rule()
        {
            var criteria = new CheckBetRuleCriteria();

            using (var db = new ObjectDb(criteria.GetSettingKey(), 3600))
            {
                var param = criteria.GetSpParams();

                db.ExecuteNonQuery(param);
            }
        }
    }
}
