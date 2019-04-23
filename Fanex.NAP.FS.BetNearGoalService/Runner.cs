namespace Fanex.NAP.FS.BetNearGoalService
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
        private new const string ServiceName = "Bet Near Goal Service";

        private static BetsNearGoalService _betNearGoalService;

        private IEnumerable<Match> _matches;
        private readonly List<Task> _threads;
        private readonly MicroService _baseService;
        private bool _isRunning = false;       

        public Runner()
        {
            _matches = new List<Match>();
            _threads = new List<Task>();
            _baseService = new MicroService(ServiceName, FraudCaseId.BetsNearGoal);
            _betNearGoalService = new BetsNearGoalService();

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
                    if (_isRunning) return;

                    _isRunning = true;

                    CleanResult();
                    Precondition();
                    Rule();

                    _isRunning = false;
                }
            }
            catch (Exception ex)
            {
                _isRunning = false;
                exception = ex.Message;
                status = false;

                var logMessage = new LogMessage(ex.Message)
                    .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                    .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error($"{ProjectName} - {ServiceName} - Execute Exeption {logMessage}");
            }

            Report(status, exception, "Fraud Case is executed at " + DateTime.Now);
        }

        private void Report(bool status, string exception, string message)
        {
            var model = new ReportModel()
            {
                ServiceName = base.ServiceName,
                Exception = exception,
                Status = status,
                Timeout = _baseService.FraudCase.FraudCaseInterval,
                IsActive = _baseService.FraudCase.IsActive,
                Message = message
            };

            _baseService.Report(model);
        }

        private void CleanResult()
        {
            if (_matches == null || !_matches.Any()) return;

            _matches = Enumerable.Empty<Match>();

            //_threads?.Clear();
        }

        private void Precondition()
        {
            LogService.Info($"{ProjectName} - {ServiceName} - Precondition is executing.");

            _matches = _betNearGoalService.PreCondition<Match>();

            LogService.Info($"{ProjectName} - {ServiceName} - Precondition is executed.");
        }

        private void Rule()
        {
            LogService.Info($"{ProjectName} - {ServiceName} - Rule is executing.");

            ExecuteBySeparatedThreads();

            LogService.Info($"{ProjectName} - {ServiceName} - Rule is executed.");
        }

        private void ExecuteBySeparatedThreads()
        {
            if (_matches == null) return;
            foreach (var match in _matches)
            {
               // var thread = ExecuteSingle(match);

               // _threads.Add(thread);
               ExecuteSingleMatch(match);
            }

            Thread.Sleep(TimeSpan.FromSeconds(2));

            // Task.WaitAll(_threads.ToArray());
        }

        private void ExecuteSingleMatch(Match match)
        {
            try
            {
                _betNearGoalService.MatchIds = match.MatchId.ToString();
                _betNearGoalService.Rule();
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                    .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                    .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error($"{ProjectName} - {ServiceName} - ExecuteSingle Exeption {logMessage}");
            }
        }

        private Task ExecuteSingle(Match match)
        {
            var thread = Task.Factory.StartNew(m =>
            {
                try
                {
                    _betNearGoalService.MatchIds = ((Match)m).MatchId.ToString();
                    _betNearGoalService.Rule();                   
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
    }
}
