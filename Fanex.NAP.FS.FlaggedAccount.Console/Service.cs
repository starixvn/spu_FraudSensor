namespace Fanex.NAP.FS.FlaggedAccount.Console
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.FS.Core;
    using Fanex.NAP.FS.Library.Core.Helpers.Api;
    using Fanex.NAP.FS.Library.Core.Helpers.Xml;
    using Fanex.NAP.FS.Library.Core.Utilities;
    using Fanex.NAP.FS.Library.DTO;
    using Fanex.NAP.FS.Library.Models.FraudCase.FlaggedAccount;
    using Fanex.NAP.FS.Service;

    public class Service : BaseService
    {
        private IEnumerable<Match> _matches;
        private IEnumerable<FlaggedCustomer> _flaggedCustomers;
        protected readonly FS.Service.FlaggedAccountService FlaggedAccountService;
        private const int ChunkSize = 100;
        private const int SleepSize = 2;

        public Service() : base(Library.Models.FraudCaseId.FlaggedAccount)
        {
            _matches = new List<Match>();

            _flaggedCustomers = new List<FlaggedCustomer>();

            FlaggedAccountService = new FS.Service.FlaggedAccountService();
        }

        private IEnumerable<FlaggedCustomer> GetFlaggedCustomer()
        {
            var param = new CustomerEvidenceList
            {
                CustIdList = string.Empty,
                EvidenceCodeList = string.Empty
            };

            return ApiHelper.Post<IEnumerable<FlaggedCustomer>>(AppSetting.CustomerEvidenceApi, param).Result;
        }

        public override BaseService CleanResult()
        {
            if (_matches == null || !_matches.Any())
            {
                return this;
            }

            _matches = Enumerable.Empty<Match>();

            return base.CleanResult();
        }

        public override BaseService Precondition()
        {
            _matches = FlaggedAccountService.PreCondition<Match>();

            return base.Precondition();
        }

        public override BaseService Rule()
        {
            _flaggedCustomers = GetFlaggedCustomer();

            ExecuteBySeparatedThreads();

            return base.Rule();
        }

        private void ExecuteBySeparatedThreads()
        {
            if (_matches == null || !_matches.Any())
            {
                return;
            }
            var matchChunks = _matches.Chunk(ChunkSize);

            foreach (var matches in matchChunks)
            {
                ExecuteMatches(matches.ToList());
            }

            Thread.Sleep(TimeSpan.FromSeconds(SleepSize));
        }

        private void ExecuteMatches(List<Match> matches)
        {
            try
            {
                var matchIds = string.Join(",", (matches).Select(t => t.MatchId.ToString()));
                var flaggedAccounts = XmlHelper.ToXmlString(_flaggedCustomers);

                FlaggedAccountService.Rule(matchIds, flaggedAccounts);

            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error($"Flagged Account service - ExecuteMatches Exeption {logMessage}");
            }
        }        
    }
}