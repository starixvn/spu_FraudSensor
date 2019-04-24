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
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

// ReSharper disable once IdentifierTypo
namespace Fanex.NAP.FS.ArbitrageService
{
    public partial class Runner : ServiceBase
    {
        private const string ProjectName = "Fraud Sensor Service";
        private new const string ServiceName = "Arbitrage Service";

        private static Service.ArbitrageService _arbitrageService;
        private readonly MicroService _baseService;
        private bool _isRunning = false;
        private IEnumerable<ArbitrageTicket> _tickets;

        public Runner()
        {
            _baseService = new MicroService(ServiceName, FraudCaseId.Arbitrage);
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

                    _isRunning = true;
                    
                    Precondition();
                    Rule();

                    _isRunning = false;

                    Thread.Sleep(TimeSpan.FromSeconds(2));
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

                LogService.Error($"{ProjectName} - {ServiceName} - Execute Exeption {logMessage}");
            }

            Report(status, exception, "Fraud Case is executed at " + DateTime.Now);
        }

        private void GroupData(int matchId, IEnumerable<ArbitrageTicket> tickets)
        {
            // ReSharper disable once PossibleMultipleEnumeration
            if (tickets != null && tickets.Any())
            {
                var arbitrageTicket = tickets
                    .Select(t => t.CustId)
                    .Distinct();

                var arbitrageCustomers = arbitrageTicket.Chunk(500);

                var taskCustomers = new List<Task<IEnumerable<ArbitrageCustomer>>>();

                foreach (var arbitrageCustomer in arbitrageCustomers)
                {
                   var taskCustomer = GroupCustomer(arbitrageCustomer);

                   taskCustomers.Add(taskCustomer);

                   Thread.Sleep(TimeSpan.FromSeconds(5));
                }

                Task.WaitAll(taskCustomers.ToArray());

                if (!taskCustomers.Any()) return;
                {
                    try
                    {
                        LogService.Info($"{ProjectName} - {ServiceName} - Rule is executing.");

                        var customers = taskCustomers.SelectMany(t => t.Result);

                        _arbitrageService.MatchIds = tickets.ToXmlString();
                        _arbitrageService.Associations = customers.ToXmlString();

                        _arbitrageService.Rule();

                        LogService.Info($"{ProjectName} - {ServiceName} - Rule is executed.");
                    }
                    catch (Exception ex)
                    {
                        var logMessage = new LogMessage(ex.Message)
                            .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                            .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                        LogService.Error(
                            $"{ProjectName} - {ServiceName} - MatchId: {matchId} - GroupData Exeption: {logMessage}");
                    }
                }
            }
            else
            {
                LogService.Error(
                    $"{ProjectName} - {ServiceName} - MatchId: {matchId} - Exeption: MatchIds({tickets.Count()}) is null.");
            }
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
                    $"{ProjectName} - {ServiceName} - GroupCustomer Exception: Cannot get Group Customer from Web API - Exception: {ex.Message}");

                return Enumerable.Empty<ArbitrageCustomer>();
            }
        }

        private void Report(bool status, string exception, string message)
        {
            var model = new ReportModel
            {
                ServiceName = ServiceName,
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
            LogService.Info($"{ProjectName} - {ServiceName} - Precondition is executing.");

            _tickets = _arbitrageService.PreCondition<ArbitrageTicket>();

            LogService.Info($"{ProjectName} - {ServiceName} - Precondition is executed.");
        }

        private void Rule()
        {

            XmlHelper.RegisterType<ArbitrageTicket>();
            XmlHelper.RegisterType<ArbitrageCustomer>();

            if (_tickets == null || !_tickets.Any()) return;

            var ticketGroupBy = _tickets.GroupBy(t => t.MatchId);

            foreach (var ticket in ticketGroupBy)
            {
                GroupData(ticket.Key, ticket);
            }
        }
    }
}