using Fanex.Logging;
using Fanex.Logging.Extensions;
using Fanex.Logging.Extensions.Database;
using Fanex.NAP.FS.Core;
using Fanex.NAP.FS.Library.Core.Helpers.Api;
using Fanex.NAP.FS.Library.Core.Utilities;
using Fanex.NAP.FS.Library.Models.FraudCaseTask;
using Fanex.NAP.FS.Service;
using Quartz;
using Quartz.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.FS.ArbitrageJob
{
    class ArbitrageJob : IJob
    {
        private readonly ArbitrageService _arbitrageService;
        private const string projectName = "Fraud Sensor Service";
        private new const string serviceName = "Arbitrage Service";
        bool _isRunning = false;

        public ArbitrageJob()
        {
            _arbitrageService = new ArbitrageService();
        }

        public async Task Execute(IJobExecutionContext context)
        {
            //var exception = string.Empty;
            //await Task.Run(() =>
            //{
            //    try
            //    {
            //        if (!_isRunning)
            //        {
            //            _isRunning = true;

            //            var tickets = Precondition();
            //            Rule(tickets);

            //            _isRunning = false;
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        _isRunning = false;
            //        exception = ex.Message;

            //        var logMessage = new LogMessage(ex.Message)
            //        .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
            //        .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

            //        LogService.Error($"{projectName} - {serviceName} - Execute Exeption {logMessage}");
            //    }
            //});
        }

        private List<ArbitrageTicket> Precondition()
        {
            return _arbitrageService.PreCondition<ArbitrageTicket>().ToList();
        }

        private void Rule(IReadOnlyCollection<ArbitrageTicket> tickets)
        {
            if (tickets == null || !tickets.Any()) return;

            var ticketGroupBy = tickets
                .GroupBy(t => t.MatchId)
                .Select(x => new
                {
                    MatchId = x.Key,
                    ArbitrageTicketList = x.ToList()
                }).ToList();

            foreach (var ticket in ticketGroupBy)
            {
                var custIds = ticket.ArbitrageTicketList
                    .Select(t => t.CustId)
                    .Distinct()
                    .ToList();

                var groupCustomer = GroupCustomer(custIds);

                ExecuteGroupCustomer(ticket.MatchId, groupCustomer, ticket.ArbitrageTicketList);

            }

            ticketGroupBy = null;
        }

        private List<ArbitrageCustomer> GroupCustomer(List<int> custIds)
        {
            var arbitrageCustomers = custIds.Chunk(500).ToList();

            var taskCustomers = new List<Task<IEnumerable<ArbitrageCustomer>>>();

            foreach (var arbitrageCustomer in arbitrageCustomers)
            {
                var taskCustomer = GetGroupCustomerFromGraph(arbitrageCustomer);

                taskCustomers.Add(taskCustomer);
                taskCustomer = null;
            }

            arbitrageCustomers = null;

            Task.WaitAll(taskCustomers.ToArray());

            return taskCustomers.SelectMany(t => t.Result).ToList();
        }

        private async Task<IEnumerable<ArbitrageCustomer>> GetGroupCustomerFromGraph(IEnumerable<int> custids)
        {
            try
            {
                return await ApiHelper.Post<IEnumerable<ArbitrageCustomer>>(AppSetting.GroupCustomerPath, custids);
            }
            catch (Exception ex)
            {
                return Enumerable.Empty<ArbitrageCustomer>();
            }
        }

        private void ExecuteGroupCustomer(int matchId, List<ArbitrageCustomer> associations, List<ArbitrageTicket> matchIds)
        {
            try
            {
                if (matchIds.Any() && associations.Any())
                {
                    var xmlMatchIds = Ultilities.XmlConvertByIsolatedStorage(matchIds, $"{matchId}.xml");
                    var xmlAssociations = Ultilities.XmlConvertByIsolatedStorage(associations, $"association{matchId}.xml");

                    _arbitrageService.Rule(xmlMatchIds, xmlAssociations);
                }
            }
            catch (Exception ex)
            {
            }
        }

    }
}
