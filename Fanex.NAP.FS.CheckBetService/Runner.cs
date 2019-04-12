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
        static string serviceName = "Check Bet";
        static string projectName = "Fraud Sensor Service";
        private MicroService baseService;

        static Service.CheckBetService checkBetService;
        private bool isRunning = false;

        public Runner()
        {
            baseService = new MicroService(serviceName, FraudCaseId.CheckBet);
            checkBetService = new CheckBetService();

            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            baseService.Start(Execute);
        }

        protected override void OnStop()
        {
            baseService.Hold();
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
#if DEBUG
                    Console.WriteLine("-> Fraud Case is executing at " + DateTime.Now);
#endif
                    if (isRunning) return;

                    isRunning = true;

                    checkBetService.Rule();

                    isRunning = false;
#if DEBUG
                    Console.WriteLine("=> Fraud Case is executed at " + DateTime.Now);
#endif
                    LogService.Info($"{projectName} - {serviceName} - Fraudcase is executed.");
                }
            }
            catch (Exception ex)
            {
                exception = ex.Message;
                status = false;
                isRunning = false;

                var logMessage = new LogMessage(ex.Message)
                    .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                    .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

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
                IsActive = baseService.FraudCase.IsActive,
                Timeout = baseService.FraudCase.FraudCaseInterval,
                Message = message
            };

            baseService.Report(model);
        }
    }
}
