using Fanex.Graph.Data;
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

                var logMessage = new LogMessage(ex.Message)
                    .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                    .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error($"{ProjectName} - {ServiceName} - Execute Exeption {logMessage}");

                Report(false, exception, "Fraud Case is executed at " + DateTime.Now);
            }
        }

        private void GroupData(int matchId, IEnumerable<ArbitrageTicket> tickets)
        {
            var startTime = DateTime.Now;
            var debugMessage = new LogMessage($"{ProjectName} - {ServiceName} - GroupData(rule) for big match is executing.");
            var innerMessage = new Dictionary<string, object>();
            innerMessage.Add("MatchID: ", matchId);
            innerMessage.Add("Start time: ", DateTime.Now);

            var arbitrageTicket = tickets
                .Select(t => t.CustId)
                .Distinct();

            var arbitrageCustomers = arbitrageTicket.Chunk(500);
            var customerGroup = new List<IEnumerable<ArbitrageCustomer>>();

            foreach (var arbitrageCustomer in arbitrageCustomers)
            {
                var customers = GroupCustomerFromDB(arbitrageCustomer);

                if (!customers.Any()) continue;

                customerGroup.Add(customers);
            }
            innerMessage.Add("Get data from GraphDB successfully: ", DateTime.Now);
            innerMessage.Add("\t tickets: ", tickets.Count());
            innerMessage.Add("\t customerGroup: ", customerGroup.Count());

            try
            {
                if (tickets.Any() && customerGroup.Any())
                {
                    _arbitrageService.MatchIds = tickets.ToXmlString();
                    _arbitrageService.Associations = customerGroup.ToXmlString();
                    innerMessage.Add("Convert to XML successfuly: ", DateTime.Now);

                    _arbitrageService.Rule();
                    innerMessage.Add("Run rule successfuly: ", DateTime.Now);
                    if ((DateTime.Now - startTime).TotalSeconds > 30)
                    {
                        debugMessage.AddSection("Data Info", innerMessage);
                        LogService.Info(debugMessage.ToString());
                    }
                }
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

        private IEnumerable<ArbitrageCustomer> GroupCustomerFromDB(IEnumerable<int> custids)
        {
            IEnumerable<ArbitrageCustomer> result = null;

            try
            {
                result = DbSessionFactory.Execute<ArbitrageCustomer>("GroupCustomer", new { CustIdList = custids }).Data;

                if (!result.Any())
                {
                    LogService.Info($"{ProjectName} - {ServiceName} - Group Customer on Graph DB is executed. Result is null.");
                }
                else
                {
                    LogService.Info($"{ProjectName} - {ServiceName} - Group Customer on Graph DB is executed. Result({result.Count()})");
                }
                
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                    .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error(
                    $"{ProjectName} - {ServiceName} - - GroupCustomer Exeption: {logMessage}");
            }

            return result;
        }

        private async Task<IEnumerable<ArbitrageCustomer>> GroupCustomer(IEnumerable<int> custids)
        {
            try
            {
                return await ApiHelper.Post<IEnumerable<ArbitrageCustomer>>(AppSetting.GroupCustomerPath, custids);
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                        .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                        .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error(
                    $"{ProjectName} - {ServiceName} - GroupCustomer Exception: Cannot get Group Customer from Web API - Exception: {logMessage}");

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
            LogService.Info($"{ProjectName} - {ServiceName} - Rule is executing.");

            XmlHelper.RegisterType<ArbitrageTicket>();
            XmlHelper.RegisterType<ArbitrageCustomer>();

            if (_tickets == null || !_tickets.Any()) return;

            var ticketGroupBy = _tickets.GroupBy(t => t.MatchId);

            foreach (var ticket in ticketGroupBy)
            {
                GroupData(ticket.Key, ticket);
            }

            LogService.Info($"{ProjectName} - {ServiceName} - Rule is executed.");
        }
    }
}