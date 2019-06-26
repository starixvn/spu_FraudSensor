namespace Fanex.NAP.FS.FlaggedAccount.Console
{
    using Fanex.Data;
    using Fanex.Data.ConnectionStringProvider20;
    using Fanex.NAP.FS.Core;
    using Fanex.NAP.FS.Library.Core.Helpers.Api;
    using Fanex.NAP.FS.Library.DTO;
    using Fanex.NAP.FS.Library.Models;
    using Fanex.NAP.FS.Library.Models.FraudCase.FlaggedAccount;
    using Fanex.NAP.FS.Service;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class Run
    {
        private const int ChunkSize = 100;
        protected readonly Service.FlaggedAccountService FlaggedAccountService;
        private readonly IEnumerable<FlaggedCustomer> _flaggedCustomers;

        private new const string ServiceName = "Flagged Account Service";
        private const string ProjectName = "Fraud Sensor Service";

        private IEnumerable<Match> _matches;
        private readonly List<Task> _threads;
        private readonly MicroService _baseService;

        private bool _isRunning = false;
        private DateTime _lastExecuted;
        public Run()
        {
            ConfigManager();

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

                //LogService.Error(message);
            }

            _lastExecuted = DateTime.Now;
        }

        public void ConfigManager()
        {
            DbSettingProviderManager
                  .StartNewSession()
                  .Use(new ConnectionStringProvider20())
                  .UseDefaultDbSettingProvider(AppSetting.AppData, false)
                  .Run();

            //LogManager.SetDefaultLogCategory(AppSetting.FanexLogCategory)
            //         .Use(new List<ILogEngine>
            //         {
            //             new SentryLogging(new SentryEngineOptions
            //             {
            //                 Dsn = new Dsn(AppSetting.SentryLogging)
            //             })
            //         });
        }

        public void Start()
        {
            _baseService.Start(Execute);
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

                        //CleanResult();
                        //Precondition();
                        //Rule();

                        _isRunning = false;
                    }

                    _lastExecuted = nowDate;
                }
            }
            catch (Exception ex)
            {
                _isRunning = false;
                exception = ex.Message;

                //var logMessage = new LogMessage(ex.Message)
                //    .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                //    .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                //LogService.Error($"{ProjectName} - {ServiceName} - Execute Exeption {logMessage}");

                Report(false, exception, "Fraud Case is executed at " + DateTime.Now);
            }
        }

        private void Report(bool status, string exception, string message)
        {
            //var model = new ReportModel()
            //{

            //    ServiceName = ServiceName,
            //    Exception = exception,
            //    Status = status,
            //    Timeout = _baseService.FraudCase.FraudCaseInterval,
            //    IsActive = _baseService.FraudCase.IsActive,
            //    Message = message
            //};

            //_baseService.Report(model);
        }
    }
}
