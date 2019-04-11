namespace Fanex.NAP.FS.BetNearGoalService
{
    using Fanex.NAP.FS.Service;
    using Fanex.NAP.FS.Library.Models;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Diagnostics;
    using System.Linq;
    using System.ServiceProcess;
    using System.Text;
    using System.Threading.Tasks;
    using Fanex.NAP.FS.Model;
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.FS.Library.DTO;

    public partial class Runner : ServiceBase
    {
        static string serviceName = "Bets Near Goal Service";
        static string projectName = "Fraud Sensor Service";
        static BetsNearGoalService betNearGoalService;

        private IEnumerable<Match> _matches;
        private readonly List<Task> _threads;
        private MicroService baseService;
        private bool isRunning = false;       

        public Runner()
        {
            _matches = new List<Match>();
            _threads = new List<Task>();
            baseService = new MicroService(serviceName, FraudCaseId.BetsNearGoal);
            betNearGoalService = new BetsNearGoalService();

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
                    if (isRunning) return;
#if DEBUG
                    Console.WriteLine("-> Fraud Case is executing at " + DateTime.Now);
#endif
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
                isRunning = false;
                exception = ex.Message;
                status = false;

                var logMessage = new LogMessage(ex.Message)
                    .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                    .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error($"{projectName} - {serviceName} - Execute Exeption {logMessage}");
            }

            Report(status, exception, "Fraud Case is executed at " + DateTime.Now);
        }

        void Report(bool status, string exception, string message)
        {
            var model = new ReportModel()
            {
                ServiceName = serviceName,
                Exception = exception,
                Status = status,
                Timeout = baseService.FraudCase.FraudCaseInterval,
                IsActive = baseService.FraudCase.IsActive,
                Message = message
            };

            baseService.Report(model);
        }

        private void CleanResult()
        {
            baseService.Clean(_matches, _threads);
        }

        private void Precondition()
        {
            _matches = betNearGoalService.PreCondition<Match>();
        }

        private void Rule()
        {
            ExecuteBySaparatedThreads();
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

        private Task ExecuteSingle(Match match)
        {
            var thread = Task.Factory.StartNew(m =>
            {
                try
                {
                    betNearGoalService.MatchIds = ((Match)m).MatchId.ToString();
                    betNearGoalService.Rule();                   
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
    }
}
