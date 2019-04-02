using Fanex.Logging;
using Fanex.NAP.FS.Core;
using Fanex.NAP.FS.Library.Core.Helpers.Api;
using Fanex.NAP.FS.Library.Core.Helpers.Xml;
using Fanex.NAP.FS.Library.Core.Utilities;
using Fanex.NAP.FS.Library.DTO;
using Fanex.NAP.FS.Library.Models;
using Fanex.NAP.FS.Library.Models.FraudCase.FlaggedAccount;
using Fanex.NAP.FS.Model;
using Fanex.NAP.FS.Service;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace Fanex.NAP.FS.FlaggedAccountService
{
    public partial class Runner : ServiceBase
    {
        private const int ChunkSize = 100;
        protected readonly Service.FlaggedAccountService flaggedAccountService;
        private readonly IEnumerable<FlaggedCustomer> _flaggedCustomers;

        static string serviceName = "Flagged Account Service";
        static string projectName = "Fraud Sensor Service";

        private IEnumerable<Match> _matches;
        private readonly List<Task> _threads;
        private MicroService baseService;

        private bool isRunning = false;

        public Runner()
        {
            _matches = new List<Match>();
            _threads = new List<Task>();
            _flaggedCustomers = new List<FlaggedCustomer>();
            baseService = new MicroService(serviceName, FraudCaseId.FlaggedAccount);
            flaggedAccountService = new Service.FlaggedAccountService();

            var param = new CustomerEvidenceList
            {
                CustIdList = string.Empty,
                EvidenceCodeList = string.Empty
            };

            try
            {
                _flaggedCustomers = ApiHelper.Post<IEnumerable<FlaggedCustomer>>(AppSetting.CustomerEvidenceApi, param).Result;
            }
            catch (Exception exception)
            {
                var error = "<b>Error:</b> " + exception.Message +
                              "<br><b>StackTrace:</b> " + exception.StackTrace;
                var message = string.Format("<b>Problem with FraudCaseId = {0}, FraudCaseName = {1}</b><br><br>", FraudCaseId.FlaggedAccount, serviceName) + error;

                LogService.Error(message);
            }


            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            baseService.Start(OnTimedEvent);
        }

        protected override void OnStop()
        {
            baseService.Hold();
        }

        private void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            var exception = string.Empty;
            var status = true;
            Report(status, exception);
            try
            {
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
                  //  LogService.Info($"{projectName} - {serviceName} - Executed at {DateTime.Now}");
                }
            }
            catch (Exception ex)
            {
                exception = ex.Message;
                status = false;

                LogService.Critical($"{projectName} - {serviceName} - OnTimedEvent Exception: {ex.Message}");
            }

            Report(status, exception);
        }

        void Report(bool status, string exception)
        {
            var model = new ReportModel()
            {

                ServiceName = serviceName,
                Exception = exception,                
                Status = status,
                Timeout = baseService.FraudCase.FraudCaseInterval
            };

            baseService.Report(model);
        }

        void CleanResult()
        {
            baseService.Clean(_matches, _threads);
        }

        void Precondition()
        {
            _matches = flaggedAccountService.ExecutePrecondition();
        }

        void Rule()
        {
            ExecuteBySaparatedThreads();
        }

        private void ExecuteBySaparatedThreads()
        {
            if (_matches != null)
            {
                var matchChunks = _matches.Chunk(ChunkSize);

                foreach (var matches in matchChunks)
                {
                    var thread = ExecuteMultiple(matches.ToList());

                    _threads.Add(thread);
                }

                Task.WaitAll(_threads.ToArray());
            }
        }

        private Task ExecuteMultiple(List<Match> matches)
        {
            var thread = Task.Factory.StartNew(m =>
            {
                try
                {
                    var flaggedAccountsXml = XmlHelper.ToXmlString(_flaggedCustomers);
                    flaggedAccountService.ExecuteRule(string.Join(",", ((List<Match>)m).Select(t => t.MatchId.ToString())), flaggedAccountsXml);

                }
                catch (Exception exception)
                {
                    var error = "<b>Error:</b> " + exception.Message +
                              "<br><b>StackTrace:</b> " + exception.StackTrace;
                    var message = string.Format("<b>Problem with FraudCaseId = {0}, FraudCaseName = {1}</b><br><br>", FraudCaseId.FlaggedAccount, serviceName) + error;

                    LogService.Error(message);
                }
            }, matches);

            return thread;

        }
    }
}
