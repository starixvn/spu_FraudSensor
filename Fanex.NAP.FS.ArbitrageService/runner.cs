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
using System.Collections.Specialized;
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

        private List<ArbitrageCustomer> GroupCustomer(List<int> custIds)
        {
            var arbitrageCustomers = custIds.Chunk(500);
            var customerGroup = new List<ArbitrageCustomer>();
            var lockMe = new object();

            Parallel.ForEach(arbitrageCustomers, (currentCustomer) =>
            {
                var customers = GetGroupCustomer(currentCustomer);

                if (customers.Any())
                {
                    lock (lockMe)
                    {
                        customerGroup.AddRange(customers);
                    }
                }

                Thread.Sleep(10);
            });

            return customerGroup;
        }

        private void ExecuteGroupCustomer(int matchId, List<ArbitrageCustomer> associations, List<ArbitrageTicket> matchIds)
        {
            try
            {
                if (matchIds.Any() && associations.Any())
                {
                    _arbitrageService.MatchIds = Ultilities.XMLConvert(matchIds);
                    _arbitrageService.Associations = Ultilities.XMLConvert(associations);
                    
                    _arbitrageService.Rule();
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

        private IEnumerable<ArbitrageCustomer> GetGroupCustomer(IEnumerable<int> custIds)
        {
            IEnumerable<ArbitrageCustomer> result = null;

            try
            {
                result = DbSessionFactory.Execute<ArbitrageCustomer>("GroupCustomer", new { CustIdList = custIds }).Data;
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

            var lockMe = new object();

            if (_tickets == null || !_tickets.Any()) return;

            var ticketGroupBy = _tickets
                .GroupBy(t => t.MatchId)
                .Select(x => new
                {
                    MatchId = x.Key,
                    ArbitrageTicketList = x.ToList()
                }).ToList();

            var options = new ParallelOptions() { MaxDegreeOfParallelism = 2 };

            Parallel.ForEach(ticketGroupBy, options, ticket =>
            {
                var custIds = ticket.ArbitrageTicketList
                    .Select(t => t.CustId)
                    .Distinct()
                    .ToList();

                var groupCustomer = GroupCustomer(custIds);

                lock (lockMe)
                {
                    ExecuteGroupCustomer(ticket.MatchId, groupCustomer, ticket.ArbitrageTicketList);
                }
            });

            LogService.Info($"{ProjectName} - {ServiceName} - Rule is executed.");
        }
    }
}