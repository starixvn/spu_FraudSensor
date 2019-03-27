using Fanex.Logging;
using Fanex.NAP.FS.Core;
using Fanex.NAP.FS.Library.DTO;
using Fanex.NAP.FS.Library.Models;
using Fanex.NAP.FS.Service;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace Fanex.NAP.FS.BigStakeNearGoalService
{
    public partial class Runner : ServiceBase
    {
        static string serviceName = "Big Stake Near Goal Service";
        static string projectName = "Fraud Sensor Service";

        static Service.BigStakeNearGoalService bigStakeNearGoalService;
        private IEnumerable<Match> _matches;
        private readonly List<Task> _threads;
        private MicroService baseService;

        public Runner()
        {
            _matches = new List<Match>();
            _threads = new List<Task>();
            bigStakeNearGoalService = new Service.BigStakeNearGoalService();
            baseService = new MicroService(serviceName, FraudCaseId.BSNG);

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
#if DEBUG
                            Console.WriteLine("-> Fraud Case is executing at " + DateTime.Now);
#endif
                CleanResult();
                Precondition();
                Rule();

#if DEBUG
                            Console.WriteLine("=> Fraud Case is executed at " + DateTime.Now);
#endif
                LogService.Info($"{projectName} - {serviceName} - Executed at {DateTime.Now}");
            }
            catch (Exception ex)
            {
                exception = ex.Message;
                status = false;

                LogService.Critical($"{projectName} - {serviceName} - OnTimedEvent Exception: {ex.Message}");
            }

            Report(status, exception);
        }

        void Precondition()
        {
            _matches = bigStakeNearGoalService.PreCondition();            
        }

        void Rule()
        {
            ExecuteBySaparatedThreads();            
        }

        void CleanResult()
        {           
            if (_matches != null && _matches.Any())
            {
                _matches = Enumerable.Empty<Match>();
                _threads.Clear();
            }
        }

        void Report(bool status, string exception)
        {
            baseService.Report(status, exception);
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
                    var message = string.Format("<b>Problem with FraudCaseId = {0}, FraudCaseName = {1}, MatchId = {2}</b><br><br>", baseService.FraudCase.FraudCaseId, baseService.FraudCase.FraudCaseName, match.MatchId) + error;

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
