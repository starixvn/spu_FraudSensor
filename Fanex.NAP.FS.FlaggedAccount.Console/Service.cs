using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Fanex.NAP.FS.Library.Core.Helpers.Xml;
using Fanex.NAP.FS.Library.Core.Utilities;
using Fanex.NAP.FS.Library.DTO;
using Fanex.NAP.FS.Library.Models.FraudCase.FlaggedAccount;
using Fanex.NAP.FS.Service;

namespace Fanex.NAP.FS.FlaggedAccount.Console
{
    public class Service : BaseService
    {
        private IEnumerable<Match> _matches;
        private readonly IEnumerable<FlaggedCustomer> _flaggedCustomers;
        protected readonly FS.Service.FlaggedAccountService FlaggedAccountService;
        private const int ChunkSize = 100;

        public Service() : base(Library.Models.FraudCaseId.FlaggedAccount)
        {
            _matches = new List<Match>();
            _flaggedCustomers = new List<FlaggedCustomer>();
            FlaggedAccountService = new FS.Service.FlaggedAccountService();
        }

        private void ExecuteBySeparatedThreads()
        {
            if (_matches == null) return;
            var matchChunks = _matches.Chunk(ChunkSize);

            foreach (var matches in matchChunks)
            {
                ExecuteMatches(matches.ToList());
            }

            Thread.Sleep(TimeSpan.FromSeconds(2));
        }

        private void ExecuteMatches(List<Match> matches)
        {
            try
            {
                var matchIds = string.Join(",", ((List<Match>)matches).Select(t => t.MatchId.ToString()));
                var flaggedAccounts = XmlHelper.ToXmlString(_flaggedCustomers);

                FlaggedAccountService.Rule(matchIds, flaggedAccounts);

            }
            catch (Exception ex)
            {
                //var logMessage = new LogMessage(ex.Message)
                //.AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                //.AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                //if (!logMessage.Contains("(17507649)"))
                //{
                //    LogService.Error($"{ProjectName} - {ServiceName} - ExecuteMultiple Exeption {logMessage}");
                //}
            }
        }

        public override BaseService CleanResult()
        {
            if (_matches == null || !_matches.Any()) return this;
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
            ExecuteBySeparatedThreads();

            return base.Rule();
        }
    }
}
