namespace Fanex.NAP.FS.FlaggedAccountService
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
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
    using System.Threading;
    using System.Threading.Tasks;

    public partial class Runner : ServiceBase
    {
        private const int ChunkSize = 100;
        protected readonly Service.FlaggedAccountService FlaggedAccountService;
        private readonly IEnumerable<FlaggedCustomer> _flaggedCustomers;

        private new const string ServiceName = "Flagged Account Service";
        static string _projectName = "Fraud Sensor Service";

        private IEnumerable<Match> _matches;
        private readonly List<Task> _threads;
        private readonly MicroService _baseService;

        private bool _isRunning = false;

        public Runner()
        {
            _matches = new List<Match>();
            _threads = new List<Task>();
            _flaggedCustomers = new List<FlaggedCustomer>();
            _baseService = new MicroService(ServiceName, FraudCaseId.FlaggedAccount);
            FlaggedAccountService = new Service.FlaggedAccountService();

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
                var message = string.Format("<b>Problem with FraudCaseId = {0}, FraudCaseName = {1}</b><br><br>", FraudCaseId.FlaggedAccount, ServiceName) + error;

                LogService.Error(message);
            }


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
                    LogService.Info($"{_projectName} - {ServiceName} - Fraudcase is begining for executing.");

                    _isRunning = true;

                    CleanResult();
                    Precondition();
                    Rule();

                    _isRunning = false;
#if DEBUG
                    Console.WriteLine("=> Fraud Case is executed at " + DateTime.Now);
#endif
                    LogService.Info($"{_projectName} - {ServiceName} - Fraudcase is executed.");
                }
            }
            catch (Exception ex)
            {
                _isRunning = false;
                exception = ex.Message;
                status = false;

                var logMessage = new LogMessage(ex.Message)
                    .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                    .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error($"{_projectName} - {ServiceName} - Execute Exeption {logMessage}");
            }

            Report(status, exception, "Fraud Case is executed at " + DateTime.Now);
        }

        private void Report(bool status, string exception, string message)
        {
            var model = new ReportModel()
            {

                ServiceName = ServiceName,
                Exception = exception,                
                Status = status,
                Timeout = _baseService.FraudCase.FraudCaseInterval,
                IsActive = _baseService.FraudCase.IsActive,
                Message = message
            };

            _baseService.Report(model);
        }

        private void CleanResult()
        {
            if (_matches == null || !_matches.Any()) return;
            _matches = Enumerable.Empty<Match>();

            _threads?.Clear();
        }

        private void Precondition()
        {
            _matches = FlaggedAccountService.PreCondition<Match>();
        }

        private void Rule()
        {
            ExecuteBySeparatedThreads();
        }

        private void ExecuteBySeparatedThreads()
        {
            if (_matches == null) return;
            var matchChunks = _matches.Chunk(ChunkSize);

            foreach (var matches in matchChunks)
            {
                //var thread = ExecuteMultiple(matches.ToList());

                //_threads.Add(thread);
                ExecuteMatches(matches.ToList());
            }

            Thread.Sleep(TimeSpan.FromSeconds(2));

            //Task.WaitAll(_threads.ToArray());
        }

        private void ExecuteMatches(List<Match> matches)
        {
            try
            {

                FlaggedAccountService.MatchIds = string.Join(",", ((List<Match>)matches).Select(t => t.MatchId.ToString()));
                FlaggedAccountService.FlaggedAccounts = XmlHelper.ToXmlString(_flaggedCustomers);

                FlaggedAccountService.Rule();

            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error($"{_projectName} - {ServiceName} - ExecuteMultiple Exeption {logMessage}");               
            }
        }

        private Task ExecuteMultiple(List<Match> matches)
        {
            var thread = Task.Factory.StartNew(m =>
            {               
                try
                {
                  
                    FlaggedAccountService.MatchIds = string.Join(",", ((List<Match>)m).Select(t => t.MatchId.ToString()));
                    FlaggedAccountService.FlaggedAccounts = XmlHelper.ToXmlString(_flaggedCustomers);

                    FlaggedAccountService.Rule();

                }
                catch (Exception ex)
                {
                    var logMessage = new LogMessage(ex.Message)
                    .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                    .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                    LogService.Error($"{_projectName} - {ServiceName} - ExecuteMultiple Exeption {logMessage}");

                    Ultilities.WriteLog("log-MatchId-" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".log", matches.Select(x=>x.MatchId.ToString()));
                }
            }, matches);

            return thread;

        }
    }
}
