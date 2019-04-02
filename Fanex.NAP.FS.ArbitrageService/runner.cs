using Fanex.Logging;
using Fanex.NAP.FS.Core;
using Fanex.NAP.FS.Library.Core.Helpers.Api;
using Fanex.NAP.FS.Library.Core.Helpers.Xml;
using Fanex.NAP.FS.Library.Core.Utilities;
using Fanex.NAP.FS.Library.Models;
using Fanex.NAP.FS.Library.Models.FraudCaseTask;
using Fanex.NAP.FS.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace Fanex.NAP.FS.ArbitrageService
{
    public partial class Runner : ServiceBase
    {
        static string serviceName = "Arbitrage Service";
        static string projectName = "Fraud Sensor Service";

        static Service.ArbitrageService arbitrageService;
        private MicroService baseService;
        private IEnumerable<ArbitrageTicket> tickets;
        private bool isRunning = false;

        public Runner()
        {

            baseService = new MicroService(serviceName, FraudCaseId.Arbitrage);
            arbitrageService = new Service.ArbitrageService();

            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            baseService.Start(OnTimedEvent);
        }

        protected override void OnStop()
        {
            baseService.Hold();
        }

        private void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            var exception = string.Empty;
            var status = true;

            try
            {
                var isUM = baseService.CheckUM();

                if (!isUM)
                {
                    if (isRunning) return;
#if DEBUG
                    Console.WriteLine("-> Fraud Case is executing at " + DateTime.Now);
#endif
                    isRunning = true;

                    CleanResult();
                    Precondition();
                    Rule();

                    isRunning = false;
#if DEBUG
                    Console.WriteLine("=> Fraud Case is executed at " + DateTime.Now);
#endif
                    LogService.Info($"{projectName} - {serviceName} - Executed at {DateTime.Now}");
                }
            }
            catch (Exception ex)
            {
                exception = ex.Message;
                status = false;

                LogService.Error($"{projectName} - {serviceName} - OnTimedEvent Exception: {ex.Message}");
            }

            Report(status, exception);
        }

        private void GroupData(int matchId, IEnumerable<ArbitrageTicket> tickets)
        {
            var tsk = tickets.Select(t => t.CustId).Distinct().Chunk(500).Select(GroupCustomer).ToArray();

#if DEBUG
            Console.WriteLine("=> GroupData is executing at " + DateTime.Now + ". Ticket By counter: " + tsk.Count());
#endif

            Task.WaitAll(tsk);

            var customers = tsk.SelectMany(t => t.Result);

#if DEBUG
            Console.WriteLine("=> GroupData is executing at " + DateTime.Now + ". Customers counter: " + customers.Count());
#endif

            arbitrageService.Rule(tickets.ToXmlString(), customers.ToXmlString());

#if DEBUG
            Console.WriteLine("===> GroupData is executed at " + DateTime.Now);
#endif
        }

        private async Task<IEnumerable<ArbitrageCustomer>> GroupCustomer(IEnumerable<int> custids)
        {
            try
            {
                return await ApiHelper.Post<IEnumerable<ArbitrageCustomer>>(AppSetting.GroupCustomerPath, custids);
            }
            catch (Exception ex)
            {
                LogService.Error($"{projectName} - {serviceName} - GroupCustomer Exception: Cannot get Group Customer from Web API - Exception: {ex.Message}");

                return Enumerable.Empty<ArbitrageCustomer>();
            }
        }

        void Report(bool status, string exception)
        {
            baseService.Report(status, exception);
        }

        void Precondition()
        {
            tickets = arbitrageService.PreCondition();

#if DEBUG
            Console.WriteLine("=> Pre-Condition is executing at " + DateTime.Now + ". Ticket counter: " + tickets.Count());
#endif
        }

        void Rule()
        {
            XmlHelper.RegisterType<ArbitrageTicket>();
            XmlHelper.RegisterType<ArbitrageCustomer>();

            if (tickets != null && tickets.Any())
            {
                var ticketGroupBy = tickets.GroupBy(t => t.MatchId);
#if DEBUG
                Console.WriteLine("=> Rule is executing at " + DateTime.Now + ". Ticket Group By counter: " + ticketGroupBy.Count());
#endif
                foreach (var g in ticketGroupBy)
                {
                    GroupData(g.Key, g);
                }
            }
        }

        void CleanResult()
        {

        }
    }
}
