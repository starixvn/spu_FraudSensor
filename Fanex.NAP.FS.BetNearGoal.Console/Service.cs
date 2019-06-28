namespace Fanex.NAP.FS.BetNearGoal.Console
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.FS.Library.DTO;
    using Fanex.NAP.FS.Service;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Service : BaseService
    {
        private readonly FS.Service.BetsNearGoalService _service;
        private IEnumerable<Match> _matches;

        public Service() : base(Library.Models.FraudCaseId.BetsNearGoal)
        {
            _service = new FS.Service.BetsNearGoalService();
            _matches = new List<Match>();
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

            foreach (var match in _matches)
            {
                ExecuteSingleMatch(match);
            }
        }

        private void ExecuteSingleMatch(Match match)
        {
            try
            {
                _service.Rule(match.MatchId.ToString());
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                    .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                    .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error($"Bet Near Goal service - ExecuteSingleMatch Exeption {logMessage}");
            }
        }
    }
}