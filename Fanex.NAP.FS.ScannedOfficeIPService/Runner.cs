namespace Fanex.NAP.FS.ScannedOfficeIPService
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.FS.Library.Core.Utilities;
    using Fanex.NAP.FS.Library.DTO;
    using Fanex.NAP.FS.Library.Models;
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
        private new const string ServiceName = "Scanned Office IP Service";
        private const string ProjectName = "Fraud Sensor Service";
        private static Service.ScannedOfficeIpService _scannedOfficeIpService;

        private readonly MicroService _baseService;
        private bool _isRunning = false;
        private IEnumerable<Match> _matches;
        private readonly Queue<Task> _tasks;

        public Runner()
        {
            _baseService = new MicroService(ServiceName, FraudCaseId.ScannedOfficeIP);
            _scannedOfficeIpService = new ScannedOfficeIpService();
            _matches = new List<Match>();
            _tasks = new Queue<Task>();

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
                    LogService.Info($"{ProjectName} - {ServiceName} - Fraudcase is executed.");
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
                ServiceName = ServiceName,
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
            _matches = null;
            _tasks.Clear();
        }

        private void Precondition() {
            _matches = _scannedOfficeIpService.PreCondition<Match>();
        }

        private void Rule()
        {
            if (_matches == null || !_matches.Any()) return;
            foreach (var chunk in _matches.Chunk(100))
            {
                var ids = chunk.Select(m => m.MatchId);

                _scannedOfficeIpService.MatchIds = string.Join(",", ids);

                _tasks.Enqueue(Task.Run(() => _scannedOfficeIpService.Rule()));
            }

            Task.WaitAll(_tasks.ToArray());

            foreach (var item in _tasks.Where(t => t.IsFaulted))
            {
                LogService.Error($"{ProjectName} - {ServiceName} - Rule Exception: {item.Exception.Message}, {item.Exception}");
            }
        }
    }
}
