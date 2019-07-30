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

        private readonly Service.ArbitrageService _arbitrageService;
        private readonly MicroService _baseService;
        private bool _isRunning = false;

        private DateTime _lastExecuted;

        public Runner()
        {
            _baseService = new MicroService(ServiceName, FraudCaseId.Arbitrage);
            _arbitrageService = new Service.ArbitrageService();

            _lastExecuted = DateTime.Now;

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
            var nowDate = DateTime.Now;
            try
            {
                var lastExecutedTotalSeconds = nowDate.Subtract(_lastExecuted).TotalSeconds;

                if (Convert.ToBoolean(source) || lastExecutedTotalSeconds >= _baseService.FraudCase.FraudCaseInterval)
                {
                    Report(true, exception, "Fraud Case is executing at " + DateTime.Now);

                    var isUm = _baseService.CheckUM();

                    if (!isUm)
                    {
                        if (!_isRunning)
                        {
                            _isRunning = true;

                            var tickets = Precondition();

                            Rule(tickets);

                            _isRunning = false;
                        }
                    }
                    else
                    {
                        _baseService.WriteLogForUM(nowDate);
                    }

                    _lastExecuted = nowDate;
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

        private void ExecuteGroupCustomer(int matchId, List<ArbitrageCustomer> associations, List<ArbitrageTicket> matchIds)
        {
            try
            {
                if (matchIds.Any() && associations.Any())
                {
                    var xmlMatchIds = matchIds.ToXmlString();
                    var xmlAssociations = associations.ToXmlString();

                    _arbitrageService.Rule(xmlMatchIds, xmlAssociations);
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

        private async Task<IEnumerable<ArbitrageCustomer>> GetGroupCustomerFromGraphDB(IEnumerable<int> custids)
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
#if !DEBUG
            _baseService.Report(model);
#endif
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

                List<ArbitrageCustomer> groupCustomer = GetGroupCustomerFromGraphDB(custIds).Result.ToList();

                ExecuteGroupCustomer(ticket.MatchId, groupCustomer, ticket.ArbitrageTicketList);

                Thread.Sleep(10);
            }

            _baseService.MonitoringServiceLog();
        }
    }
}