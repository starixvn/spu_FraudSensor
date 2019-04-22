using Fanex.Logging;
using Fanex.Logging.Extensions;
using Fanex.Logging.Extensions.Database;
using Fanex.NAP.FS.Library.DTO;
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
using Fanex.NAP.FS.Library.Core.Utilities;

namespace Fanex.NAP.FS.ScannedOfficeIPService
{
    public partial class Runner : ServiceBase
    {
        static string serviceName = "Scanned Office IP Service";
        static string projectName = "Fraud Sensor Service";
        static Service.ScannedOfficeIpService scannedOfficeIpService;

        private MicroService baseService;
        private bool isRunning = false;
        private IEnumerable<Match> _matches;
        private readonly Queue<Task> _tasks;

        public Runner()
        {
            baseService = new MicroService(serviceName, FraudCaseId.ScannedOfficeIP);
            scannedOfficeIpService = new ScannedOfficeIpService();
            _matches = new List<Match>();
            _tasks = new Queue<Task>();

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
                    LogService.Info($"{projectName} - {serviceName} - Fraudcase is executed.");
                }
            }
            catch (Exception ex)
            {
                isRunning = false;
                exception = ex.Message;
                status = false;

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
                Timeout = baseService.FraudCase.FraudCaseInterval,
                IsActive = baseService.FraudCase.IsActive,
                Message = message
            };

            baseService.Report(model);
        }

        void CleanResult()
        {
            _matches = null;
            _tasks.Clear();
        }

        void Precondition() {
            _matches = scannedOfficeIpService.PreCondition<Match>();
        }

        void Rule()
        {
            if (_matches != null && _matches.Any())
            {
                foreach (var chunk in _matches.Chunk(100))
                {
                    var ids = chunk.Select(m => m.MatchId);

                    scannedOfficeIpService.MatchIds = string.Join(",", ids);

                    _tasks.Enqueue(Task.Run(() => scannedOfficeIpService.Rule()));
                }

                Task.WaitAll(_tasks.ToArray());

                foreach (var item in _tasks.Where(t => t.IsFaulted))
                {
                    LogService.Error($"{projectName} - {serviceName} - Rule Exception: {item.Exception.Message}, {item.Exception}");
                }
            }
        }
    }
}
