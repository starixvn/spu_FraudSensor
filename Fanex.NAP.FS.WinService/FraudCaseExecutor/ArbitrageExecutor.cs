namespace Fanex.NAP.FS.WinService
{
    using Fanex.NAP.FS.Library.Core.BaseClasses;
    using Fanex.NAP.FS.Library.Core.Utilities;
    using Fanex.NAP.FS.Library.Criteria.FraudSensorTask;
    using Fanex.NAP.FS.Library.DTO;
    using Fanex.NAP.FS.Library.Models.FraudCaseTask;
    using System.Collections.Generic;
    using System.Linq;

    [FraudCase(Library.Models.FraudCaseId.Arbitrage)]
    public class ArbitrageExecutor : FraudCaseExecutor
    {
        private IEnumerable<int> matches;
        private readonly int batch = 100;

        public ArbitrageExecutor(FraudCase fraudCase) : base(fraudCase)
        {
        }

        protected override void ClearOldResult()
        {
            matches = null;
        }

        protected override void ExecutePrecondition()
        {
            matches = DataService.GetData<int>(new ArbitragePreconditionCriteria());
        }

        protected override void ExecuteRule()
        {
            if (matches != null && matches.Any())
            {
                var data = 
                    matches
                    .Chunk(batch)
                    .AsParallel()
                    .SelectMany(ids => DataService.GetData<ArbitrageMatch>(new ArbitragePreRuleCriteria { MatchIds = string.Join(",", ids) }));
            }
        }

        protected override void TakeNextAction()
        {
        }
    }
}