namespace Fanex.NAP.FS.DailyDataCaptureService
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.FS.Library.Models;
    using Fanex.NAP.FS.Model;
    using Fanex.NAP.FS.Service;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Diagnostics;
    using System.Linq;
    using System.ServiceProcess;
    using System.Text;
    using System.Threading.Tasks;

    public partial class Runner : ServiceBase
    {
        private const string ProjectName = "Fraud Sensor Service";

        static Service.DailyDataCaptureService _dailyDataCaptureService;
        private readonly MicroService _baseService;

        private bool _isRunning = false;

        public Runner()
        {
            _baseService = new MicroService(base.ServiceName, FraudCaseId.DailyDataCapture);
            _dailyDataCaptureService = new Service.DailyDataCaptureService();

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
                    .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex)).ToString();

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

       private static void CleanResult()
        {

        }

       private static void Precondition()
        {

        }

       private static void Rule()
        {
            _dailyDataCaptureService.Rule();
        }
    }
}
