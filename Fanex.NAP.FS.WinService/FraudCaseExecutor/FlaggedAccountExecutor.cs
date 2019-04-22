namespace Fanex.NAP.FS.WinService
{
    using Fanex.NAP.FS.Library.Core.BaseClasses;
    using Library.Core.Helpers.Api;
    using Library.Core.Helpers.Xml;
    using Library.Core.Utilities;
    using Library.DTO;
    using Library.Models.FraudCase.FlaggedAccount;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Threading.Tasks;

    [FraudCase(Library.Models.FraudCaseId.FlaggedAccount)]
    public class FlaggedAccountExecutor : FraudCaseExecutor
    {
        private IEnumerable<Match> _matches;

        private readonly IEnumerable<FlaggedCustomer> _flaggedCustomers;

        private readonly List<Task> _threads;

        private const int ChunkSize = 100;

        private readonly string FPS_WEBAPI_GET_CUSTOMER_EVIDENCE = ConfigurationManager.AppSettings["FPS_WEBAPI_GET_CUSTOMER_EVIDENCE"];

        public FlaggedAccountExecutor(FraudCase fraudCase) : base(fraudCase)
        {
            _matches = new List<Match>();
            _threads = new List<Task>();
            _flaggedCustomers = new List<FlaggedCustomer>();

            var param = new CustomerEvidenceList
            {
                CustIdList = string.Empty,
                EvidenceCodeList = string.Empty
            };

            try
            {
                _flaggedCustomers = ApiHelper.Post<IEnumerable<FlaggedCustomer>>(FPS_WEBAPI_GET_CUSTOMER_EVIDENCE, param).Result;
            }
            catch (Exception e)
            {
                var message = "Flagged Accounts initializing gets error: " + e.Message;
                SendMailInform(message, message);
            }
        }

        protected override void ClearOldResult()
        {
            if (_matches != null && _matches.Any())
            {
                _matches = Enumerable.Empty<Match>();
                _threads.Clear();
            }
        }

        protected override void ExecutePrecondition()
        {
            _matches = _fraudSensorTaskService.FlaggedAccountExecutePrecondition();
        }

        protected override void ExecuteRule()
        {
            ExecuteBySaparatedThreads();
        }

        protected override void TakeNextAction()
        {
        }

        #region private method

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
                    _fraudSensorTaskService.FlaggedAccountExecuteRule(string.Join(",", ((List<Match>)m).Select(t => t.MatchId.ToString())), flaggedAccountsXml);

                    _errorTimes = 0;
                }
                catch (Exception exception)
                {
                    var error = "<b>Error:</b> " + exception.Message +
                              "<br><b>StackTrace:</b> " + exception.StackTrace;
                    var message = string.Format("<b>Problem with FraudCaseId = {0}, FraudCaseName = {1}</b><br><br>", FraudCase.FraudCaseId, FraudCase.FraudCaseName) + error;

                    if (_errorTimes++ < 10)
                    {
                        SendMailInform(message);
                    }
                }
            }, matches);

            return thread;
        }

        #endregion private method
    }
}