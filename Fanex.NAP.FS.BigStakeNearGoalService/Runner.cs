namespace Fanex.NAP.FS.BigStakeNearGoalService
{
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
    using System.Threading;
    using System.Threading.Tasks;

    public partial class Runner : ServiceBase
    {
        private const string ProjectName = "Fraud Sensor Service";

        private static Service.BigStakeNearGoalService _bigStakeNearGoalService;
        private IEnumerable<Match> _matches;
        public readonly List<Task> Threads;
        private readonly MicroService _baseService;
        private bool _isRunning = false;

        public Runner()
        {
            _matches = new List<Match>();
            Threads = new List<Task>();
            _bigStakeNearGoalService = new Service.BigStakeNearGoalService();
            _baseService = new MicroService(base.ServiceName, FraudCaseId.BSNG);

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

        private void Execute(object source, System.Timers.ElapsedEventArgs e)
        {
            var exception = string.Empty;
            var status = true;
            
            try
            {
                Report(true, exception, "Fraud Case is executing at " + DateTime.Now);

                var isUm = _baseService.CheckUM();
                if (!isUm)
                {

#if DEBUG
                    Console.WriteLine("-> Fraud Case is executing at " + DateTime.Now);
#endif
                    if (_isRunning) return;

                    _isRunning = true;

                    CleanResult();
                    Precondition();
                    Rule();

                    _isRunning = false;

#if DEBUG
                    Console.WriteLine("=> Fraud Case is executed at " + DateTime.Now);
#endif
                    LogService.Info($"{ProjectName} - {ServiceName} - Fraudcase is executed.");
                }
            }
            catch (Exception ex)
            {
                exception = ex.Message;
                status = false;
                _isRunning = false;

                var logMessage = new LogMessage(ex.Message)
                    .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                    .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error($"{ProjectName} - {ServiceName} - Execute Exeption {logMessage}");
            }

            Report(status, exception, "Fraud Case is executed at " + DateTime.Now);
        }

        private void Precondition()
        {
            _matches = _bigStakeNearGoalService.PreCondition<Match>();
        }

        private void Rule()
        {
            ExecuteBySeparatedThreads();
        }

        private void CleanResult()
        {
            if (_matches == null || !_matches.Any()) return;
            _matches = Enumerable.Empty<Match>();
            Threads.Clear();
        }

        private void Report(bool status, string exception, string message)
        {
            var model = new ReportModel()
            {
                ServiceName = ServiceName,
                Exception = exception,                
                Status = status,
                IsActive = _baseService.FraudCase.IsActive,
                Timeout = _baseService.FraudCase.FraudCaseInterval,
                Message  = message
            };

            _baseService.Report(model);
        }

        private Task ExecuteSingle(Match match)
        {
            var thread = Task.Factory.StartNew(m =>
            {
                try
                {
                    var matchIds = ((Match)m).MatchId.ToString();
                    _bigStakeNearGoalService.MatchIds = matchIds;
                    _bigStakeNearGoalService.Rule();
                }
                catch (Exception ex)
                {
                    var logMessage = new LogMessage(ex.Message)
                    .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                    .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                    LogService.Error($"{ProjectName} - {ServiceName} - ExecuteSingle Exeption {logMessage}");
                }
            }, match);

            return thread;
        }

        private void Execute(Match match)
        {
            try
            {
                var matchIds = ((Match)match).MatchId.ToString();
                _bigStakeNearGoalService.MatchIds = matchIds;
                _bigStakeNearGoalService.Rule();
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error($"{ProjectName} - {ServiceName} - ExecuteSingle Exeption {logMessage}");
            }

        }

        private void ExecuteBySeparatedThreads()
        {
            if (_matches == null) return;
            foreach (var match in _matches)
            {
                // var thread = ExecuteSingle(match);

                // _threads.Add(thread);

                Execute(match);
            }

            //Task.WaitAll(_threads.ToArray());

            Thread.Sleep(TimeSpan.FromSeconds(2));
        }
    }
}
