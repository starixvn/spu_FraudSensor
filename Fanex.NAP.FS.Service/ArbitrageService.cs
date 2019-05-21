namespace Fanex.NAP.FS.Service
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NAP.FS.Library.Criteria.FraudSensorTask;
    using System.Collections.Generic;

    public class ArbitrageService
    {
        public IEnumerable<T> PreCondition<T>()
        {
            var criteria = new ArbitragePreconditionCriteria();
            var repository = new DynamicRepository();

            return repository.Fetch<T>(criteria);
        }       
      
        public void Rule(string matchIds, string associations)
        {
            var repository = new DynamicRepository();
            
            var criteria = new ArbitrageRuleCommand()
            {
                Matches = matchIds,
                Associations = associations
            };

            repository.Execute(criteria);
        }
    }
}
