using Fanex.Logging;
using Fanex.Logging.Extensions;
using Fanex.Logging.Extensions.Database;
using Fanex.NAP.FS.Library.Core.Utilities;
using Fanex.NAP.FS.Library.DTO;
using Fanex.NAP.FS.Model;
using Fanex.NAP.FS.Service;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace Fanex.NAP.FS.DelayGoalService
{
    public partial class Runner : ServiceBase
    {
        static string serviceName = "Delay Goal Service";
        static string projectName = "Fraud Sensor Service";
        static Service.DelayGoalService delayGoalService;
        private MicroService baseService;
        private bool isRunning = false;

        private IEnumerable<Match> _matches;
        private readonly List<Task> _threads;
        private const int ChunkSize = 100;

        public Runner()
        {
            _matches = new List<Match>();
            _threads = new List<Task>();
            delayGoalService = new Service.DelayGoalService();
            baseService = new MicroService(serviceName, Library.Models.FraudCaseId.DelayedGoal);

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
                    LogService.Info($"{projectName} - {serviceName} - Fraudcase is executed.");
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
            _matches = delayGoalService.PreCondition<Match>();
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

                if (_threads != null)
                {
                    _threads.Clear();
                }
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
                Message = message
            };

            baseService.Report(model);
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
                    delayGoalService.MatchIds = string.Join(",", ((List<Match>)m).Select(t => t.MatchId.ToString()));
                    delayGoalService.Rule();
                }
                catch (Exception ex)
                {
                    var logMessage = new LogMessage(ex.Message)
                    .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                    .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                    LogService.Error($"{projectName} - {serviceName} - Execute Exeption {logMessage}");
                }
            }, matches);

            return thread;
        }
    }
}
