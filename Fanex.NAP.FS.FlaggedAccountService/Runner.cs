using Fanex.Logging;
using Fanex.NAP.FS.Core;
using Fanex.NAP.FS.Library.Core.Helpers.Api;
using Fanex.NAP.FS.Library.Core.Helpers.Xml;
using Fanex.NAP.FS.Library.Core.Utilities;
using Fanex.NAP.FS.Library.DTO;
using Fanex.NAP.FS.Library.Models;
using Fanex.NAP.FS.Library.Models.FraudCase.FlaggedAccount;
using Fanex.NAP.FS.Service;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Fanex.NAP.FS.FlaggedAccountService
{
    public partial class Runner : MicroServiceBase
    {
        private const int ChunkSize = 100;
        protected readonly Service.FlaggedAccountService flaggedAccountService;
        private readonly IEnumerable<FlaggedCustomer> _flaggedCustomers;        
        private readonly string FPS_WEBAPI_GET_CUSTOMER_EVIDENCE = AppSetting.CustomerEvidenceApi;

        static string serviceName = "Flagged Account Service";

        public Runner() : base(serviceName, FraudCaseId.FlaggedAccount)
        {
            _matches = new List<Match>();
            _threads = new List<Task>();
            _flaggedCustomers = new List<FlaggedCustomer>();

            var param = new CustomerEvidenceList
            {
                CustIdList = string.Empty,
                EvidenceCodeList = string.Empty
            };

            try
            {
                _flaggedCustomers = ApiHelper.Post<CustomerEvidenceList, IEnumerable<FlaggedCustomer>>(FPS_WEBAPI_GET_CUSTOMER_EVIDENCE, param, 2, 82).Result;
            }
            catch (Exception exception)
            {
                var error = "<b>Error:</b> " + exception.Message +
                              "<br><b>StackTrace:</b> " + exception.StackTrace;
                var message = string.Format("<b>Problem with FraudCaseId = {0}, FraudCaseName = {1}</b><br><br>", FraudCaseId.FlaggedAccount, serviceName) + error;

                LogService.Error(message);
            }
            

            InitializeComponent();
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

        protected override void CleanResult()
        {
            if (_matches != null && _matches.Any())
            {
                _matches = Enumerable.Empty<Match>();
                _threads.Clear();
            }
        }

        protected override void Precondition()
        {
            _matches = flaggedAccountService.ExecutePrecondition();
        }

        protected override void Rule()
        {
            ExecuteBySaparatedThreads();
        }

        private void ExecuteBySaparatedThreads()
        {
            if (_matches != null)
            {
                var matchChunks = _matches.Chunk(ChunkSize);

                foreach (var matches in matchChunks)
                {
                    var thread = ExecuteMultiple(matches.ToList());

                    _threads.Add(thread);
                }

                Task.WaitAll(_threads.ToArray());
            }
        }

        private Task ExecuteMultiple(List<Match> matches)
        {
            var thread = Task.Factory.StartNew(m =>
            {
                try
                {
                    var flaggedAccountsXml = XmlHelper.ToXmlString(_flaggedCustomers);
                    flaggedAccountService.ExecuteRule(string.Join(",", ((List<Match>)m).Select(t => t.MatchId.ToString())), flaggedAccountsXml);
                                       
                }
                catch (Exception exception)
                {
                    var error = "<b>Error:</b> " + exception.Message +
                              "<br><b>StackTrace:</b> " + exception.StackTrace;
                    var message = string.Format("<b>Problem with FraudCaseId = {0}, FraudCaseName = {1}</b><br><br>", FraudCaseId.FlaggedAccount, serviceName) + error;

                    LogService.Error(message);
                }
            }, matches);

            return thread;

        }
    }
}
