using Fanex.Logging;
using Fanex.NAP.FS.Core;
using Fanex.NAP.FS.Library.Core.Helpers.Api;
using Fanex.NAP.FS.Library.Core.Helpers.Xml;
using Fanex.NAP.FS.Library.Core.Utilities;
using Fanex.NAP.FS.Library.DTO;
using Fanex.NAP.FS.Library.Models;
using Fanex.NAP.FS.Library.Models.FraudCaseTask;
using Fanex.NAP.FS.Service;
using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.FS.ArbitrageService
{
    public partial class Runner : MicroServiceBase
    { 
        static string serviceName = "Arbitrage Service";
        static string projectName = "Fraud Sensor Service";

        static Service.ArbitrageService arbitrageService;
        private IEnumerable<ArbitrageTicket> tickets;

        public Runner() : base(serviceName, FraudCaseId.Arbitrage)
        {
            _matches = new List<Match>();
            _threads = new List<Task>();
            arbitrageService = new Service.ArbitrageService();

            InitializeComponent();
        }

        protected override void Precondition()
        {
            tickets = arbitrageService.PreCondition();
        }

        protected override void Rule()
        {
            XmlHelper.RegisterType<ArbitrageTicket>();
            XmlHelper.RegisterType<ArbitrageCustomer>();

            foreach (var g in tickets.GroupBy(t => t.MatchId))
            {
                GroupData(g.Key, g);
            }
        }

        protected override void Report(bool status, string exception)
        {
            var client = new RestClient(AppSetting.HeartBeatUrl);
            var request = new RestRequest("/api/HealthCheck/beat", Method.POST);

            request.AddQueryParameter("serviceName", serviceName);
            request.AddQueryParameter("status", status.ToString());
            request.AddQueryParameter("exception", exception);

            var result = client.Execute(request);

            if (!result.IsSuccessful)
            {
                LogService.Error(result.ErrorMessage);
            }
        }

        private static void GroupData(int matchId, IEnumerable<ArbitrageTicket> tickets)
        {
            var tsk = tickets.Select(t => t.CustId).Distinct().Chunk(500).Select(GroupCustomer).ToArray();

            Task.WaitAll(tsk);

            var customers = tsk.SelectMany(t => t.Result);

            arbitrageService.Rule(tickets.ToXmlString(), customers.ToXmlString());            
        }

        private static async Task<IEnumerable<ArbitrageCustomer>> GroupCustomer(IEnumerable<int> custids)
        {
            try
            {
                return await ApiHelper.Post<IEnumerable<ArbitrageCustomer>>(AppSetting.GroupCustomerPath, custids);
            }
            catch (Exception ex)
            {
                LogService.Error($"{projectName} - {serviceName} - Cannot get Group Customer from Web API - Exception: {ex.Message}");

                return Enumerable.Empty<ArbitrageCustomer>();
            }
        }

    }
}
