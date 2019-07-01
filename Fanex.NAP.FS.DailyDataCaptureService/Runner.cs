namespace Fanex.NAP.FS.DailyDataCaptureService
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
        private new const string ServiceName = "Daily Data Capture Service";

        static Service.DailyDataCaptureService _dailyDataCaptureService;
        private readonly MicroService _baseService;
        private DateTime _lastExecuted;

        private bool _isRunning = false;

        public Runner()
        {
            _baseService = new MicroService(ServiceName, FraudCaseId.DailyDataCapture);
            _dailyDataCaptureService = new Service.DailyDataCaptureService();

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

                        Rule();

                        _isRunning = false;
                    }
                    _lastExecuted = nowDate;
                }               
            }
            catch (Exception ex)
            {
                _isRunning = false;
                exception = ex.Message;

                var logMessage = new LogMessage(ex.Message)
                    .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex)).ToString();

                LogService.Error($"{ProjectName} - {ServiceName} - Execute Exeption {logMessage}");

                Report(false, exception, "Fraud Case is executed at " + DateTime.Now);
            }
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

        private void Rule()
        {
            _dailyDataCaptureService.Rule();

            _baseService.MonitoringServiceLog(5);
        }
    }
}
