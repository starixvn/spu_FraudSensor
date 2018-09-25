namespace Fanex.NAP.FS.WinService
{
    using Fanex.Logging;
    using Fanex.NAP.FS.Library.Core.BaseClasses;
    using Fanex.NAP.FS.Library.Core.Helpers.Api;
    using Fanex.NAP.FS.Library.Core.Helpers.Xml;
    using Fanex.NAP.FS.Library.Criteria.FraudSensorTask;
    using Fanex.NAP.FS.Library.DTO;
    using Fanex.NAP.FS.Library.Models.FraudCaseTask;
    using System.Collections.Generic;
    using System.Linq;

    [FraudCase(Library.Models.FraudCaseId.Arbitrage)]
    public class ArbitrageExecutor : FraudCaseExecutor
    {
        private IEnumerable<ArbitrageTicket> tickets;

        public ArbitrageExecutor(FraudCase fraudCase) : base(fraudCase)
        {
        }

        protected override void ExecutePrecondition()
        {
            tickets = DataService.GetData<ArbitrageTicket>(new ArbitragePreconditionCriteria());
        }

        protected override void ExecuteRule()
        {
            XmlHelper.RegisterType<ArbitrageTicket>();
            XmlHelper.RegisterType<ArbitrageCustomer>();

            foreach (var g in tickets.GroupBy(t => t.MatchId))
            {
                GroupData(g);
            }
        }

        protected override void TakeNextAction()
        {
        }

        protected override void ClearOldResult()
        {
        }

        private static void GroupData(IEnumerable<ArbitrageTicket> tickets)
        {
            var newCustids = new HashSet<int>(tickets.Select(t => t.CustId));

            var customers = GroupCustomer(newCustids);

            var criteria = new ArbitrageRuleCriteria
            {
                Matches = tickets.ToXmlString(),
                Associations = customers.ToXmlString()
            };

            DataService.Execute(criteria);
        }

        private static IEnumerable<ArbitrageCustomer> GroupCustomer(IEnumerable<int> custids)
        {
            return ApiHelper.Post<IEnumerable<ArbitrageCustomer>>(AppSettings.MS_WEBAPI_GROUP_CUSTOMER, custids).Result;
        }
    }
}