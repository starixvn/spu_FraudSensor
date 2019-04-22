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
    using System.Threading.Tasks;

    public partial class Runner : ServiceBase
    {
        private const string ProjectName = "Fraud Sensor Service";
        private static BetsNearGoalService _betNearGoalService;

        private IEnumerable<Match> _matches;
        private readonly List<Task> _threads;
        private readonly MicroService _baseService;
        private bool _isRunning = false;       

        public Runner()
        {
            _matches = new List<Match>();
            _threads = new List<Task>();
            _baseService = new MicroService(base.ServiceName, FraudCaseId.BetsNearGoal);
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
#if DEBUG
                    Console.WriteLine("-> Fraud Case is executing at " + DateTime.Now);
#endif
                    _isRunning = true;

                    CleanResult();
                    Precondition();
                    Rule();

                    _isRunning = false;
#if DEBUG
                    Console.WriteLine("=> Fraud Case is executed at " + DateTime.Now);
#endif                   
                    LogService.Info($"{ProjectName} - {base.ServiceName} - Fraudcase is executed.");
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

                LogService.Error($"{ProjectName} - {base.ServiceName} - Execute Exeption {logMessage}");
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

            _threads?.Clear();
        }

        private void Precondition()
        {
            _matches = _betNearGoalService.PreCondition<Match>();
        }

        private void Rule()
        {
            ExecuteBySeparatedThreads();
        }

        private void ExecuteBySeparatedThreads()
        {
            if (_matches == null) return;
            foreach (var match in _matches)
            {
                var thread = ExecuteSingle(match);

                _threads.Add(thread);
            }

            Task.WaitAll(_threads.ToArray());
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

                    LogService.Error($"{ProjectName} - {base.ServiceName} - ExecuteSingle Exeption {logMessage}");
                }
            }, match);

            return thread;
        }
    }
}
