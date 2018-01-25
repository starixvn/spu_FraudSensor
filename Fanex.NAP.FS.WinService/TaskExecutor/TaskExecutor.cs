namespace Fanex.NAP.FS.WinService
{
    using Fanex.Logging;
    using Sunplus.UMControllerLibrary;
    using System;
    using System.Configuration;
    using System.Diagnostics;
    using System.Timers;

    public abstract class TaskExecutor : ITaskExecutor
    {
        private Timer _timer;

        private bool _isRunning;

        private bool _isStoppedManually;

        protected TaskExecutor()
        {
        }

        public void Start()
        {
            _timer = new Timer
            {
                Interval = 10,
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
                var underMaintenanceBusiness = new Sunplus.UMControllerLibrary.UMBusiness();
                var isUnderMaintenance = underMaintenanceBusiness.isUnderMaintainence(Sites.AccountSite, ConfigurationManager.AppSettings["FSUrl"]);

                if (!isUnderMaintenance)
                {
                    ClearOldResult();

#if DEBUG
                    //Console.WriteLine("{0} - {1} - {2} is running", Task.CaseId, Task.TaskId, Task.TaskName);
#endif
                    ExecutePrecondition();

                    ExecuteRule();

                    TakeNextAction();

                    TriggerOtherTasks();
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
            _timer.Dispose();
        }

        protected abstract void ClearOldResult();

        protected abstract void ExecutePrecondition();

        protected abstract void ExecuteRule();

        protected abstract void TakeNextAction();

        protected void WriteLog(Exception exception)
        {
            var message = "\r\nError: " + exception.Message +
                          "\r\nStackTrace: " + exception.StackTrace;

            Logger.Log.Error(message);
        }

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
            //Console.WriteLine("{0} - {1} - {2} spent {3} running", Task.CaseId, Task.TaskId, Task.TaskName, stopwatch.Elapsed.ToString());
#endif
        }

        private void TriggerOtherTasks()
        {

        }
    }
}
