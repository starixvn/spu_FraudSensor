namespace Fanex.NAP.FS.WinService
{
    using Fanex.NAP.FS.Library.Core.BaseClasses;
    using Fanex.NAP.FS.Library.Core.Helpers.Api;
    using Fanex.NAP.FS.Library.Core.Helpers.Xml;
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
                matches
                .Chunk(batch)
                .AsParallel()
                .SelectMany(ids => DataService.GetData<ArbitrageMatch>(new ArbitragePreRuleCriteria { MatchIds = string.Join(",", ids) }))
                .GroupBy(m => m.MatchId, m => m.CustId)
                .ForAll(ClusterData);
            }
        }

        protected override void TakeNextAction()
        {
        }

        private void ClusterData(IGrouping<int, int> custids)
        {
            var matchId = custids.Key;
            var data = ApiHelper.Post<IEnumerable<int[]>>(AppSettings.MS_WEBAPI_GROUP_CUSTOMER, custids).Result
                .SelectMany((arr, i) => arr.Select(id => new ArbitrageGroup { CustId = id, GroupId = i })).ToXmlString();

            var criteria = new ArbitrageRuleCriteria
            {
                MatchIds = matchId.ToString(),
                Associations = data
            };

            DataService.Execute(criteria);
        }
    }
}