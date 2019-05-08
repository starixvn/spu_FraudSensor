namespace Fanex.NAP.FS.Service
{
    using Fanex.Data;
    using Fanex.NAP.FS.Library.Criteria.FraudSensorTask;
    using System.Collections.Generic;

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

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var param = criteria.GetSpParams();

                db.ExecuteNonQuery(param);
            }
        }
    }
}
