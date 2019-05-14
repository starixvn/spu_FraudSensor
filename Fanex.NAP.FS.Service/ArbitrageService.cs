namespace Fanex.NAP.FS.Service
{
    using Fanex.Data;
    using Fanex.Data.Repository;
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
            var repository = new DynamicRepository();
            
            var criteria = new ArbitrageRuleCommand()
            {
                Matches = this.MatchIds,
                Associations = this.Associations
            };

            repository.Execute(criteria);
        }
    }
}
