namespace Fanex.NAP.FS.CheckBetService
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.FS.Library.Models;
    using Fanex.NAP.FS.Model;
    using Fanex.NAP.FS.Service;
    using System;
    using System.ServiceProcess;

    public partial class Runner : ServiceBase
    {
        private const string ProjectName = "Fraud Sensor Service";
        private new const string ServiceName = "Check Bet Service";
        private readonly MicroService _baseService;

        private static Service.CheckBetService _checkBetService;
        private bool _isRunning = false;

        public Runner()
        {
            _baseService = new MicroService(ServiceName, FraudCaseId.CheckBet);
            _checkBetService = new CheckBetService();

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

                    _checkBetService.Rule();

                    _baseService.MonitoringServiceLog();

                    _isRunning = false;
                }
            }
            catch (Exception ex)
            {
                exception = ex.Message;
                
                _isRunning = false;

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
                IsActive = _baseService.FraudCase.IsActive,
                Timeout = _baseService.FraudCase.FraudCaseInterval,
                Message = message
            };

            _baseService.Report(model);
        }
    }
}
