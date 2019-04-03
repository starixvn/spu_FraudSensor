using Fanex.Logging;
using Fanex.NAP.FS.Core;
using Fanex.NAP.FS.Library.Core.Helpers.Api;
using Fanex.NAP.FS.Library.Core.Helpers.Xml;
using Fanex.NAP.FS.Library.Core.Utilities;
using Fanex.NAP.FS.Library.Models;
using Fanex.NAP.FS.Library.Models.FraudCaseTask;
using Fanex.NAP.FS.Model;
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
            Report(status, exception, "Fraud Case start execution at " + DateTime.Now);

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
                   // LogService.Info($"{projectName} - {serviceName} - Executed at {DateTime.Now}");
                }
            }
            catch (Exception ex)
            {
                exception = ex.Message;
                status = false;

                LogService.Error($"{projectName} - {serviceName} - OnTimeEvent Exeption", ex);
            }

            Report(status, exception, "Fraud Case is executed at " + DateTime.Now);
        }

        private void GroupData(int matchId, IEnumerable<ArbitrageTicket> tickets)
        {
            var message = string.Empty;

            var tsk = tickets.Select(t => t.CustId).Distinct().Chunk(500).Select(GroupCustomer).ToArray();

            message = "=> GroupData is executing at " + DateTime.Now + ". Ticket counter: " + tsk.Count();
            LogService.Info(message);
#if DEBUG
            Console.WriteLine(message);
#endif

            Task.WaitAll(tsk);

            var customers = tsk.SelectMany(t => t.Result);

            message = "=> GroupData is executing at " + DateTime.Now + ". Customers counter: " + customers.Count();
            LogService.Info(message);
#if DEBUG
            Console.WriteLine(message);
#endif

            arbitrageService.Rule(tickets.ToXmlString(), customers.ToXmlString());

            message = "===> GroupData is executed at " + DateTime.Now;
            LogService.Info(message);

#if DEBUG
            Console.WriteLine(message);
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

        void Report(bool status, string exception, string message)
        {
            var model = new ReportModel() {

                ServiceName = serviceName,
                Exception = exception,                
                Status = status,
                Timeout = baseService.FraudCase.FraudCaseInterval,
                Message = message
            };

            baseService.Report(model);
        }

        void Precondition()
        {
            var message = string.Empty;

            tickets = arbitrageService.PreCondition();

            message = "=> Pre-Condition is executing at " + DateTime.Now + ". Ticket counter: " + tickets.Count();
            LogService.Info(message);

#if DEBUG
            Console.WriteLine(message);
#endif
        }

        void Rule()
        {
            XmlHelper.RegisterType<ArbitrageTicket>();
            XmlHelper.RegisterType<ArbitrageCustomer>();

            if (tickets != null && tickets.Any())
            {
                var ticketGroupBy = tickets.GroupBy(t => t.MatchId);

                var message = "=> Rule is executing at " + DateTime.Now + ". Ticket Group By counter: " + ticketGroupBy.Count();
                LogService.Info(message);
#if DEBUG
                Console.WriteLine(message);
#endif
                foreach (var g in ticketGroupBy)
                {
                    message = $"=> Match Id: {g.Key} - Ticket counter: {g.Count()} - Executing at {DateTime.Now} ";
                    LogService.Info(message);

                    GroupData(g.Key, g);

                    message = $"=> Match Id: {g.Key} - Ticket counter: {g.Count()} - Executed at {DateTime.Now} ";
                    LogService.Info(message);

#if DEBUG
                    Console.WriteLine(message);
#endif
                }
            }
        }

        void CleanResult()
        {

        }
    }
}
