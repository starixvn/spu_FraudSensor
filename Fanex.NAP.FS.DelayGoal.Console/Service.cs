namespace Fanex.NAP.FS.DelayGoal.Console
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.FS.Library.Core.Utilities;
    using Fanex.NAP.FS.Library.DTO;
    using Fanex.NAP.FS.Service;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class Service : BaseService
    {
        private static FS.Service.DelayGoalService _service = new DelayGoalService();
        private IEnumerable<Match> _matches;
        private const int ChunkSize = 100;
        private readonly List<Task> _threads;

        public Service() : base(Library.Models.FraudCaseId.DailyDataCapture)
        {
            _matches = new List<Match>();
            _threads = new List<Task>();
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
            _matches = _service.PreCondition<Match>();

            return base.Precondition();
        }

        public override BaseService Rule()
        {
            ExecuteBySeparatedThreads();

            return base.Rule();
        }

        private void ExecuteBySeparatedThreads()
        {
            if (_matches == null)
            {
                return;
            }

            var matchChunks = _matches.Chunk(ChunkSize);

            foreach (var matches in matchChunks)
            {
                var thread = ExecuteMultiple(matches.ToList());

                _threads.Add(thread);
            }

            Task.WhenAll(_threads.ToArray());
        }

        private static Task ExecuteMultiple(List<Match> matches)
        {
            var thread = Task.Factory.StartNew(m =>
            {
                try
                {
                    var matchIds = string.Join(",", ((List<Match>)m).Select(t => t.MatchId.ToString()));
                    _service.Rule(matchIds);
                }
                catch (Exception ex)
                {
                    var logMessage = new LogMessage(ex.Message)
                    .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                    .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                    LogService.Error($"Delay Goal service - ExecuteMultiple Exeption {logMessage}");
                }
            }, matches);

            return thread;
        }
    }
}