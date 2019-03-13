using Fanex.Logging;
using Fanex.NAP.FS.Core;
using Fanex.NAP.FS.Library.DTO;
using Fanex.NAP.FS.Library.Models;
using Fanex.NAP.FS.Service;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fanex.NAP.FS.BigStakeNearGoalService
{
    public partial class Runner : MicroServiceBase
    {
        static string serviceName = "Big Stake Near Goal Service";

        static Service.BigStakeNearGoalService bigStakeNearGoalService;

        public Runner() : base(serviceName, FraudCaseId.BSNG)
        {
            _matches = new List<Match>();
            _threads = new List<Task>();
            bigStakeNearGoalService = new Service.BigStakeNearGoalService();

            InitializeComponent();
        }

        protected override void Precondition()
        {
            _matches = bigStakeNearGoalService.PreCondition();            
        }

        protected override void Rule()
        {
            ExecuteBySaparatedThreads();            
        }

        protected override void CleanResult()
        {           
            if (_matches != null && _matches.Any())
            {
                _matches = Enumerable.Empty<Match>();
                _threads.Clear();
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

        private Task ExecuteSingle(Match match)
        {
            var thread = Task.Factory.StartNew(m =>
            {
                try
                {
                    var matchIds = ((Match)m).MatchId.ToString();

                    bigStakeNearGoalService.Rule(matchIds);
                }
                catch (Exception exception)
                {
                    var error = "<b>Error:</b> " + exception.Message +
                              "<br><b>StackTrace:</b> " + exception.StackTrace;
                    var message = string.Format("<b>Problem with FraudCaseId = {0}, FraudCaseName = {1}, MatchId = {2}</b><br><br>", _fraudCase.FraudCaseId, _fraudCase.FraudCaseName, match.MatchId) + error;

                    LogService.Error(message);
                }
            }, match);

            return thread;
        }

        private void ExecuteBySaparatedThreads()
        {
            if (_matches != null)
            {
                foreach (var match in _matches)
                {
                    var thread = ExecuteSingle(match);

                    _threads.Add(thread);
                }

                Task.WaitAll(_threads.ToArray());
            }
        }
    }
}
