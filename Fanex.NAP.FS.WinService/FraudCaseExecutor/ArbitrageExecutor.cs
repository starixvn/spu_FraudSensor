namespace Fanex.NAP.FS.WinService
{
    using Fanex.Logging;
    using Fanex.NAP.FS.Library.Core.BaseClasses;
    using Fanex.NAP.FS.Library.Core.Helpers;
    using Fanex.NAP.FS.Library.Core.Helpers.Api;
    using Fanex.NAP.FS.Library.Core.Helpers.Xml;
    using Fanex.NAP.FS.Library.Core.Utilities;
    using Fanex.NAP.FS.Library.Criteria.FraudSensorTask;
    using Fanex.NAP.FS.Library.DTO;
    using Fanex.NAP.FS.Library.Models.FraudCaseTask;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    [FraudCase(Library.Models.FraudCaseId.Arbitrage)]
    public class ArbitrageExecutor : FraudCaseExecutor
    {
        private IEnumerable<ArbitrageTicket> tickets;

        public ArbitrageExecutor(FraudCase fraudCase) : base(fraudCase)
        {
            var message = "Arbitrage is activating";
            SendMailInform(message, message);
        }

        protected override void ExecutePrecondition()
        {
            var message = "Arbitrage is processing precondition";
            SendMailInform(message, message);

            tickets = DataService.GetData<ArbitrageTicket>(new ArbitragePreconditionCriteria());
        }

        protected override void ExecuteRule()
        {
            XmlHelper.RegisterType<ArbitrageTicket>();
            XmlHelper.RegisterType<ArbitrageCustomer>();

            foreach (var g in tickets.GroupBy(t => t.MatchId))
            {
                GroupData(g.Key, g);
            }
        }

        protected override void TakeNextAction()
        {
        }

        protected override void ClearOldResult()
        {
        }

        private static void GroupData(int matchId, IEnumerable<ArbitrageTicket> tickets)
        {
            var tsk = tickets.Select(t => t.CustId).Distinct().Chunk(500).Select(GroupCustomer).ToArray();

            Task.WaitAll(tsk);

            var customers = tsk.SelectMany(t => t.Result);

            var criteria = new ArbitrageRuleCriteria
            {
                Matches = tickets.ToXmlString(),
                Associations = customers.ToXmlString()
            };

            try
            {
                DataService.Execute(criteria);
            }
            catch (System.Exception ex)
            {
                LogService.Error($"{ex.Message}\nmatch: {matchId}\ntickets: {criteria.Matches}\nassociations: {criteria.Associations}");
            }
        }

        private static async Task<IEnumerable<ArbitrageCustomer>> GroupCustomer(IEnumerable<int> custids)
        {
            try
            {
                return await ApiHelper.Post<IEnumerable<ArbitrageCustomer>>(AppSettings.MS_WEBAPI_GROUP_CUSTOMER, custids);
            }
            catch (Exception ex)
            {
                LogService.Error($"{ex.GetType().FullName}: {ex.Message}\n\tids:{custids.Join()}", ex);

                return Enumerable.Empty<ArbitrageCustomer>();
            }
        }
    }
}