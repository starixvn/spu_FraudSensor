using System.Threading;

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

        public Runner()
        {
            _baseService = new MicroService(ServiceName, FraudCaseId.ScannedOfficeIP);
            _scannedOfficeIpService = new ScannedOfficeIpService();
            _matches = new List<Match>();

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

                var logMessage = new LogMessage(ex.Message)
                    .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                    .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error($"{ProjectName} - {ServiceName} - Execute Exeption {logMessage}");

                Report(false, exception, "Fraud Case is executed at " + DateTime.Now);
            }
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
        }

        private void Precondition() {
            _matches = _scannedOfficeIpService.PreCondition<Match>();

            LogService.Info($"{ProjectName} - {ServiceName} - Precondition is executed.");
        }

        private void Rule()
        {
            if (_matches == null || !_matches.Any()) return;
            foreach (var chunk in _matches.Chunk(100))
            {
                var ids = chunk.Select(m => m.MatchId);

                var matchIds = string.Join(",", ids);

                _scannedOfficeIpService.Rule(matchIds);
            }

            Thread.Sleep(TimeSpan.FromSeconds(2));

            LogService.Info($"{ProjectName} - {ServiceName} - Rule is executed.");
        }
    }
}
