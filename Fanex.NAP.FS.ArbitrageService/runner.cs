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
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Timers;

namespace Fanex.NAP.FS.ArbitrageService
{
    public partial class Runner : ServiceBase
    {
        private const string ProjectName = "Fraud Sensor Service";

        private static Service.ArbitrageService _arbitrageService;
        private readonly MicroService _baseService;
        private bool _isRunning;
        private IEnumerable<ArbitrageTicket> _tickets;

        public Runner()
        {
            _baseService = new MicroService(base.ServiceName, FraudCaseId.Arbitrage);
            _arbitrageService = new Service.ArbitrageService();

            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _baseService.Start(Execute);
        }

        protected override void OnStop()
        {
            _baseService.Hold();
        }

        private void Execute(object source, ElapsedEventArgs e)
        {
            var exception = string.Empty;
            var status = true;

            try
            {
                Report(true, exception, "Fraud Case is executing at " + DateTime.Now);

                var isUm = _baseService.CheckUM();

                if (!isUm)
                {
                    if (_isRunning) return;
#if DEBUG
                    Console.WriteLine("-> Fraud Case is executing at " + DateTime.Now);
#endif
                    _isRunning = true;

                    CleanResult();
                    Precondition();
                    Rule();

                    _isRunning = false;
#if DEBUG
                    Console.WriteLine("=> Fraud Case is executed at " + DateTime.Now);
#endif
                    LogService.Info($"{ProjectName} - {base.ServiceName} - Fraudcase is executed.");
                }
            }
            catch (Exception ex)
            {
                _isRunning = false;
                exception = ex.Message;
                status = false;

                var logMessage = new LogMessage(ex.Message)
                    .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                    .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error($"{ProjectName} - {base.ServiceName} - Execute Exeption {logMessage}");
            }

            Report(status, exception, "Fraud Case is executed at " + DateTime.Now);
        }

        private void GroupData(int matchId, IEnumerable<ArbitrageTicket> tickets)
        {
            var message = string.Empty;

            try
            {
                // ReSharper disable once PossibleMultipleEnumeration
                if (tickets != null && tickets.Any())
                {
                    // ReSharper disable once PossibleMultipleEnumeration
                    var tsk = tickets.Select(t => t.CustId).Distinct().Chunk(500).Select(GroupCustomer).ToArray();

#if DEBUG
                    message = $"- Rule for match {matchId} is executing. Ticket counter: " + tsk.Count();
                    Console.WriteLine(message);
#endif
                    Task.WaitAll(tsk);

                    if (tsk.Any())
                    {
                        var customers = tsk.SelectMany(t => t.Result);

                        if (customers != Enumerable.Empty<ArbitrageCustomer>())
                        {
#if DEBUG
                            message =
                                $"- Rule for match {matchId} is executing. Customers counter: {customers.Count()}.";
                            Console.WriteLine(message);
#endif

                            _arbitrageService.MatchIds = tickets.ToXmlString();
                            _arbitrageService.Associations = customers.ToXmlString();

                            _arbitrageService.Rule();
                        }
                        else
                        {
                            LogService.Error(
                                $"{ProjectName} - {base.ServiceName} - MatchId: {matchId} - Exeption: Associations({customers.Count()}) is null.");
                        }
                    }
                }
                else
                {
                    LogService.Error(
                        $"{ProjectName} - {base.ServiceName} - MatchId: {matchId} - Exeption: MatchIds({tickets.Count()}) is null.");
                }
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                    .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                    .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error(
                    $"{ProjectName} - {base.ServiceName} - MatchId: {matchId} - GroupData Exeption: {logMessage}");
            }

#if DEBUG
            Console.WriteLine(message);
#endif
        }

        private void WriteLog(string filename, IEnumerable<string> body)
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename);

            File.WriteAllLines(filePath, body);
        }

        private async Task<IEnumerable<ArbitrageCustomer>> GroupCustomer(IEnumerable<int> custids)
        {
            try
            {
                return await ApiHelper.Post<IEnumerable<ArbitrageCustomer>>(AppSetting.GroupCustomerPath, custids);
            }
            catch (Exception ex)
            {
                LogService.Error(
                    $"{ProjectName} - {base.ServiceName} - GroupCustomer Exception: Cannot get Group Customer from Web API - Exception: {ex.Message}");

                return Enumerable.Empty<ArbitrageCustomer>();
            }
        }

        private void Report(bool status, string exception, string message)
        {
            var model = new ReportModel
            {
                ServiceName = base.ServiceName,
                Exception = exception,
                Status = status,
                Timeout = _baseService.FraudCase.FraudCaseInterval,
                IsActive = _baseService.FraudCase.IsActive,
                Message = message
            };

            _baseService.Report(model);
        }

        private void Precondition()
        {
            var message = string.Empty;

            _tickets = _arbitrageService.PreCondition<ArbitrageTicket>();

            message = "===> Pre-Condition is executing. Ticket counter: " + _tickets.Count();
#if DEBUG
            Console.WriteLine(message);
#endif
        }

        private void Rule()
        {
            XmlHelper.RegisterType<ArbitrageTicket>();
            XmlHelper.RegisterType<ArbitrageCustomer>();

            if (_tickets == null || !_tickets.Any()) return;
            var ticketGroupBy = _tickets.GroupBy(t => t.MatchId);

            var message = "===> Rule funtion is executing. Ticket Group By Per Match counter: " +
                          ticketGroupBy.Count();

#if DEBUG
            Console.WriteLine(message);
#endif
            // LogService.Info(message);

            Parallel.ForEach(ticketGroupBy, new ParallelOptions
            {
                MaxDegreeOfParallelism = 1
            }, g =>
            {
#if DEBUG
                message = $"===> Match Id: {g.Key} - Ticket counter: {g.Count()} - Executing";
                Console.WriteLine(message);
#endif

                GroupData(g.Key, g);

                message = $"===> Match Id: {g.Key} - Ticket counter: {g.Count()} - Executed";
#if DEBUG
                Console.WriteLine(message);
#endif
                // LogService.Info(message);
            });
        }

        private void CleanResult()
        {
        }
    }
}