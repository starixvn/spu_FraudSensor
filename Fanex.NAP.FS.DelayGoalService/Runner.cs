namespace Fanex.NAP.FS.DelayGoalService
{
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

    public partial class Runner : ServiceBase
    {
        private new const string ServiceName = "Delay Goal Service";
        private const string ProjectName = "Fraud Sensor Service";
        private static Service.DelayGoalService _delayGoalService;
        private readonly MicroService _baseService;
        private bool _isRunning = false;

        private IEnumerable<Match> _matches;
        private readonly List<Task> _threads;
        private const int ChunkSize = 100;
        private DateTime _lastExecuted;

        public Runner()
        {
            _matches = new List<Match>();
            _threads = new List<Task>();
            _delayGoalService = new Service.DelayGoalService();
            _baseService = new MicroService(ServiceName, Library.Models.FraudCaseId.DelayedGoal);

            _lastExecuted = DateTime.Now;

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
            var nowDate = DateTime.Now;

            try
            {
                if (Convert.ToBoolean(source) || nowDate.Subtract(_lastExecuted).TotalSeconds >= _baseService.FraudCase.FraudCaseInterval)
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
                    _lastExecuted = nowDate;
                }
            }
            catch (Exception ex)
            {
                exception = ex.Message;

                _isRunning = false;

                var logMessage = new LogMessage(ex.Message)
                    .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                    .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error($"{ProjectName} - {ServiceName} - OnTimeEvent Exeption {logMessage}");

                Report(false, exception, "Fraud Case is executed at " + DateTime.Now);
            }


        }

        private void Precondition()
        {
            _matches = _delayGoalService.PreCondition<Match>();
        }

        private void Rule()
        {
            ExecuteBySeparatedThreads();

            _baseService.MonitoringServiceLog();
        }

        private void CleanResult()
        {
            if (_matches == null || !_matches.Any()) return;
            _matches = Enumerable.Empty<Match>();

            _threads?.Clear();
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
                Message = message
            };

            _baseService.Report(model);
        }

        private void ExecuteBySeparatedThreads()
        {
            if (_matches == null) return;
            var matchChunks = _matches.Chunk(ChunkSize);

            foreach (var matches in matchChunks)
            {
                var thread = ExecuteMultiple(matches.ToList());

                _threads.Add(thread);
            }

            Task.WaitAll(_threads.ToArray());
        }

        private static Task ExecuteMultiple(List<Match> matches)
        {
            var thread = Task.Factory.StartNew(m =>
            {
                try
                {
                    var matchIds = string.Join(",", ((List<Match>)m).Select(t => t.MatchId.ToString()));
                    _delayGoalService.Rule(matchIds);
                }
                catch (Exception ex)
                {
                    var logMessage = new LogMessage(ex.Message)
                    .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                    .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                    LogService.Error($"{ProjectName} - {ServiceName} - Execute Exeption {logMessage}");
                }
            }, matches);

            return thread;
        }
    }
}
