namespace Fanex.NAP.FS.WinService
{
    using Fanex.Logging;
    using Library.DTO;
    using Library.Services;
    using MailDelivery.Client;
    using Sunplus.UMControllerLibrary;
    using System;
    using System.Configuration;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using System.Timers;

    public abstract class FraudCaseExecutor : IFraudCaseExecutor, IDisposable
    {
        private Timer _timer;

        private bool _isRunning;

        private bool _isStoppedManually;

        protected readonly FraudSensorTaskService _fraudSensorTaskService;

        protected int _errorTimes;

        protected FraudCaseExecutor(FraudCase fraudCase)
        {
            FraudCase = fraudCase;
            _fraudSensorTaskService = new FraudSensorTaskService();
            _errorTimes = 0;
        }

        public FraudCase FraudCase { get; private set; }

        public void Start()
        {
            _timer = new Timer
            {
                Interval = FraudCase.FraudCaseInterval * 1000,
                AutoReset = true,
                Enabled = true
            };

            _timer.Elapsed += PreExecute;

            _timer.Start();                        

            PreExecute(null, null);                      

            var message = string.Format("{1} (ID = {0}) {2}", FraudCase.FraudCaseId, FraudCase.FraudCaseName, "ACTIVE");

            SendMailInform(message, message);
            
        }

        public virtual void Execute()
        {
            try
            {
                if (AppSettings.Environment == "2" && FraudCase.FraudCaseId != 10)
                {
                    return;
                }                                

                var underMaintenanceBusiness = new UMBusiness();
                var isUnderMaintenance = underMaintenanceBusiness.isUnderMaintainence(Sites.AccountSite, ConfigurationManager.AppSettings["FSUrl"]);

                if (!isUnderMaintenance)
                {
                    ClearOldResult();

#if DEBUG
                    Console.WriteLine("{0} - {1} is running", FraudCase.FraudCaseId, FraudCase.FraudCaseName);
#endif
                    ExecutePrecondition();

                    ExecuteRule();

                    TakeNextAction();
                }

                Logger.Log.Info("SPU Fraud Sensor Service - " + FraudCase.FraudCaseName + " is executed.");
            }
            catch (Exception exception)
            {
                WriteLog(exception);

#if DEBUG
                throw;
#endif
            }
        }

        public void Stop()
        {
            _isStoppedManually = true;
            _timer.Elapsed -= PreExecute;
            _timer.Stop();
            _timer.Close();

#if DEBUG
            Console.WriteLine("{0} - {1} stopped manually!", FraudCase.FraudCaseId, FraudCase.FraudCaseName);
#endif
            var message = string.Format("{1} (ID = {0}) {2}", FraudCase.FraudCaseId, FraudCase.FraudCaseName, "INACTIVE");
            SendMailInform(message, message);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected abstract void ClearOldResult();

        protected abstract void ExecutePrecondition();

        protected abstract void ExecuteRule();

        protected abstract void TakeNextAction();

        protected static void WriteLog(Exception exception)
        {
            var message = "\r\nError: " + exception.Message +
                          "\r\nStackTrace: " + exception.StackTrace;

            Logger.Log.Error(message, exception);
        }

        protected virtual void Dispose(bool disposing)
        {
            _timer.Dispose();
        }

        protected void SendMailInform(string message, string subject = null)
        {
            try
            {
                var environment = GetEnvironment();
                var newMail = new MailDelivery.Core.Models.Mail
                {
                    Subject = string.Format("[({0}) SPU FS] ", environment) + (string.IsNullOrEmpty(subject) ? " Error" : subject),
                    Title = "SPU Fraud Sensor",
                    Body = message,
                    MailingListId = int.Parse(ConfigurationManager.AppSettings["MailingListId"])
                };

                MailClientHandler.SendFunctionMail(newMail);
            }
            catch (Exception exception)
            {
                WriteLog(exception);
            }
        }

        #region private method

        private void PreExecute(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            if (_isRunning || _isStoppedManually)
            {
                return;
            }

#if DEBUG
            var stopwatch = new Stopwatch();
            stopwatch.Start();
#endif

            _isRunning = true;

            Execute();

            _isRunning = false;

#if DEBUG
            stopwatch.Stop();

            Console.WriteLine("{0} - {1} spent {2} running", FraudCase.FraudCaseId, FraudCase.FraudCaseName, stopwatch.Elapsed.ToString());
#endif
        }

        private static string GetEnvironment()
        {
            var environment = string.Empty;
            switch (int.Parse(ConfigurationManager.AppSettings["Environment"]))
            {
                case 1:
                    environment = "LOCAL";
                    break;

                case 2:
                    environment = "UAT";
                    break;

                case 3:
                    environment = "PRO";
                    break;

                default:
                    environment = "UNKNOWN";
                    break;
            }

            return environment;
        }

        #endregion private method
    }
}