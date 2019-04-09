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

namespace Fanex.NAP.FS.DailyDataCaptureService
{
    public partial class Runner : ServiceBase
    {
        static string serviceName = "Daily Data Capture Service";
        static string projectName = "Fraud Sensor Service";

        static Service.DailyDataCaptureService dailyDataCaptureService;
        private MicroService baseService;
        
        private bool isRunning = false;

        public Runner()
        {
            baseService = new MicroService(serviceName, FraudCaseId.DailyDataCapture);
            dailyDataCaptureService = new Service.DailyDataCaptureService();

            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            baseService.Start(Execute);
        }

        protected override void OnStop()
        {
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
                    LogService.Info($"{projectName} - {serviceName} - Rule is executed.");
                }
            }
            catch (Exception ex)
            {
                isRunning = false;
                exception = ex.Message;
                status = false;

                var logMessage = new LogMessage(ex.Message)
                    .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex)).ToString();

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

        void CleanResult()
        {

        }

        void Precondition()
        {

        }

        void Rule()
        {
            dailyDataCaptureService.Rule();
        }
    }
}
