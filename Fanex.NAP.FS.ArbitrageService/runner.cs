using Fanex.Graph.Data;
using Fanex.Logging;
using Fanex.Logging.Extensions;
using Fanex.Logging.Extensions.Database;
using Fanex.NAP.FS.Core;
using Fanex.NAP.FS.Library.Core.Helpers.Api;
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
       // private readonly IDriver _driver;

        public Runner()
        {
            _baseService = new MicroService(ServiceName, FraudCaseId.Arbitrage);
            _arbitrageService = new Service.ArbitrageService();

            //_driver = GraphDatabase.Driver("bolt://10.18.200.70:7687", AuthTokens.Basic("neo4j", "1234aa"), new Config()
            //{
            //    EncryptionLevel = EncryptionLevel.None
            //});

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

                    var tickets = Precondition();

                    Rule(tickets);

                    _isRunning = false;
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
            //var arbitrageCustomers = custIds.Chunk2(500);
            //var customerGroup = new List<ArbitrageCustomer>();
            ////var lockMe = new object();
            //foreach (var currentCustomer in arbitrageCustomers)
            //{
            //    //var customers2 = GetGroupCustomer(currentCustomer);
            //    var customers = GetGroupCustomer(currentCustomer);

            //    if (customers != null && customers.Any())
            //    {
            //        customerGroup.AddRange(customers);
            //    }

            //    Thread.Sleep(10);
            //}

            //Parallel.ForEach(arbitrageCustomers, (currentCustomer) =>
            //{
            //    var customers = GetGroupCustomer(currentCustomer);

            //    if (customers != null && customers.Any())
            //    {
            //        lock (lockMe)
            //        {
            //            customerGroup.AddRange(customers);
            //        }
            //    }
            //});

            // return customerGroup;

            var arbitrageCustomers = custIds.Chunk(500).ToList();

            var taskCustomers = new List<Task<IEnumerable<ArbitrageCustomer>>>();

            foreach (var arbitrageCustomer in arbitrageCustomers)
            {
                var taskCustomer = GroupCustomer(arbitrageCustomer);

                taskCustomers.Add(taskCustomer);

                taskCustomer = null;

                Thread.Sleep(10);
            }

            arbitrageCustomers = null;

            Task.WaitAll(taskCustomers.ToArray());

            return taskCustomers.SelectMany(t => t.Result).ToList();
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
#if !DEBUG
            _baseService.Report(model);
#endif
        }

        private List<ArbitrageTicket> Precondition()
        {
            LogService.Info($"{ProjectName} - {ServiceName} - Precondition is executed.");

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

           // var options = new ParallelOptions() { MaxDegreeOfParallelism = 2 };

            foreach (var ticket in ticketGroupBy)
            {
                var custIds = ticket.ArbitrageTicketList
                    .Select(t => t.CustId)
                    .Distinct()
                    .ToList();

                var groupCustomer = GroupCustomer(custIds);

                ExecuteGroupCustomer(ticket.MatchId, groupCustomer, ticket.ArbitrageTicketList);

                Thread.Sleep(10);
            }

            //Parallel.ForEach(ticketGroupBy, options, ticket =>
            //{
            //    var custIds = ticket.ArbitrageTicketList
            //        .Select(t => t.CustId)
            //        .Distinct()
            //        .ToList();

            //    var groupCustomer = GroupCustomer(custIds);

            //    ExecuteGroupCustomer(ticket.MatchId, groupCustomer, ticket.ArbitrageTicketList);
            //});

            ticketGroupBy = null;

            LogService.Info($"{ProjectName} - {ServiceName} - Rule is executed.");
        }

        //private List<ArbitrageCustomer> GetCustomerGroupFromGraphDB(IEnumerable<int> custIds)
        //{
        //    using (var session = _driver.Session())
        //    {
        //        return session.ReadTransaction(tx =>
        //        {
        //            var result = tx.Run("MATCH (c:Customer)-[:BELONG_TO]->(g:Group)" +
        //                                " WHERE c.CustId IN $CustIdList" +
        //                                " RETURN DISTINCT c.CustId AS CustId, g.GroupId AS GroupId",
        //                new { CustIdList = custIds });

        //            return result.Select(record => new ArbitrageCustomer()
        //            {
        //                CustId = record.Values["CustId"].As<Int32>(),
        //                GroupId = record.Values["GroupId"].As<Int32>()
        //            }).ToList();
        //        });
        //    }
        //}
    }
}