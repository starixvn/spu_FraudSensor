namespace Fanex.NAP.FS.WinService
{
    using Fanex.Logging;
    using Library.DTO;
    using Library.Services;
    using Sunplus.UMControllerLibrary;
    using System;
    using System.Configuration;
    using System.Diagnostics;
    using System.Timers;

    public abstract class FraudCaseExecutor : IFraudCaseExecutor, IDisposable
    {
        private Timer _timer;

        private bool _isRunning;

        private bool _isStoppedManually;

        protected readonly FraudSensorTaskService _fraudSensorTaskService;

        protected FraudCaseExecutor(FraudCase fraudCase)
        {
            FraudCase = fraudCase;
            _fraudSensorTaskService = new FraudSensorTaskService();
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
        }

        public virtual void Execute()
        {
            try
            {
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
            }
            catch (Exception exception)
            {
                WriteLog(exception);
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
        }

        protected abstract void ClearOldResult();

        protected abstract void ExecutePrecondition();

        protected abstract void ExecuteRule();

        protected abstract void TakeNextAction();

        protected static void WriteLog(Exception exception)
        {
            var message = "\r\nError: " + exception.Message +
                          "\r\nStackTrace: " + exception.StackTrace;

            Logger.Log.Error(message);
        }

        protected virtual void Dispose(bool disposing)
        {
            _timer.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
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
        #endregion
    }
}
