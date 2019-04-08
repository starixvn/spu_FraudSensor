using Fanex.Logging;
using Fanex.Logging.Extensions;
using Fanex.Logging.Extensions.Database;
using Fanex.NAP.FS.Library.DTO;
using Fanex.NAP.FS.Library.Models;
using Fanex.NAP.FS.Model;
using Fanex.NAP.FS.Service;
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
        private bool isRunning = false;

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
            baseService.Start(Execute);
        }

        protected override void OnStop()
        {
            baseService.Hold();
        }

        private void Execute(Object source, System.Timers.ElapsedEventArgs e)
        {
            var exception = string.Empty;
            var status = true;
            
            try
            {
                Report(status, exception, "Fraud Case is executing at " + DateTime.Now);

                var isUM = baseService.CheckUM();
                if (!isUM)
                {

#if DEBUG
                    Console.WriteLine("-> Fraud Case is executing at " + DateTime.Now);
#endif
                    if (isRunning) return;

                    isRunning = true;

                    CleanResult();
                    Precondition();
                    Rule();

                    isRunning = false;

#if DEBUG
                    Console.WriteLine("=> Fraud Case is executed at " + DateTime.Now);
#endif
                }
            }
            catch (Exception ex)
            {
                exception = ex.Message;
                status = false;
                isRunning = false;

                var logMessage = new LogMessage(ex.Message)
                    .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                    .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error($"{projectName} - {serviceName} - OnTimeEvent Exeption {logMessage}");
            }

            Report(status, exception, "Fraud Case is executed at " + DateTime.Now);
        }

        void Precondition()
        {
            _matches = bigStakeNearGoalService.PreCondition<Match>();
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

        void Report(bool status, string exception, string message)
        {
            var model = new ReportModel()
            {
                ServiceName = serviceName,
                Exception = exception,                
                Status = status,
                IsActive = baseService.FraudCase.IsActive,
                Timeout = baseService.FraudCase.FraudCaseInterval,
                Message  = message
            };

            baseService.Report(model);
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
                catch (Exception ex)
                {
                    var logMessage = new LogMessage(ex.Message)
                    .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                    .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                    LogService.Error($"{projectName} - {serviceName} - ExecuteSingle Exeption {logMessage}");
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
