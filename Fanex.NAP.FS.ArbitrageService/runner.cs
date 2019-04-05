using Fanex.Logging;
using Fanex.Logging.Extensions;
using Fanex.Logging.Extensions.Database;
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

            try
            {
                Report(status, exception, "Fraud Case is executing at " + DateTime.Now);

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
                }
            }
            catch (Exception ex)
            {
                isRunning = false;
                exception = ex.Message;
                status = false;

                var logMessage = new LogMessage(ex.Message)
                    .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                    .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error($"{projectName} - {serviceName} - OnTimeEvent Exeption {logMessage}");
            }

            Report(status, exception, "Fraud Case is executed at " + DateTime.Now);
        }

        private void GroupData(int matchId, IEnumerable<ArbitrageTicket> tickets)
        {
            var message = string.Empty;

            var tsk = tickets.Select(t => t.CustId).Distinct().Chunk(500).Select(GroupCustomer).ToArray();

            message = $"- Rule for match {matchId} is executing. Ticket counter: " + tsk.Count();

#if DEBUG
            Console.WriteLine(message);
#endif

            Task.WaitAll(tsk);

            var customers = tsk.SelectMany(t => t.Result);

            message = $"- Rule for match {matchId} is executing. Customers counter: " + customers.Count();

#if DEBUG
            Console.WriteLine(message);
#endif

            try
            {
                arbitrageService.Rule(tickets.ToXmlString(), customers.ToXmlString());
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                    .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                    .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error($"{projectName} - {serviceName} - MatchId: {matchId} - GroupData Exeption: {logMessage}");
            }

            message = $"=> Rule for match {matchId} is executed.";

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
            var model = new ReportModel()
            {
                ServiceName = serviceName,
                Exception = exception,
                Status = status,
                Timeout = baseService.FraudCase.FraudCaseInterval,
                IsActive = baseService.FraudCase.IsActive,
                Message = message
            };

            baseService.Report(model);
        }

        void Precondition()
        {
            var message = string.Empty;

            tickets = arbitrageService.PreCondition();

            message = "===> Pre-Condition is executing. Ticket counter: " + tickets.Count();

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

                var message = "===> Rule funtion is executing. Ticket Group By Per Match counter: " + ticketGroupBy.Count();

#if DEBUG
                Console.WriteLine(message);
#endif

                Parallel.ForEach(ticketGroupBy, new ParallelOptions
                {
                    MaxDegreeOfParallelism = 2
                }, (g) =>
{
                    message = $"===> Match Id: {g.Key} - Ticket counter: {g.Count()} - Executing";
#if DEBUG
                    Console.WriteLine(message);
#endif

                    GroupData(g.Key, g);

                    message = $"===> Match Id: {g.Key} - Ticket counter: {g.Count()} - Executed";

#if DEBUG
                    Console.WriteLine(message);
#endif
                });
            }
        }

        void CleanResult()
        {

        }
    }
}
