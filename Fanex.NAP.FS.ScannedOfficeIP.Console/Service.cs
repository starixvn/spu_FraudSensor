namespace Fanex.NAP.FS.ScannedOfficeIP.Console
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

    public class Service : BaseService
    {
        private readonly FS.Service.ScannedOfficeIpService _service;
        private IEnumerable<Match> _matches;
        private const int ChunkSize = 100;

        public Service() : base(Library.Models.FraudCaseId.DailyDataCapture)
        {
            _service = new ScannedOfficeIpService();
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
            Execute();

            return base.Rule();
        }

        private void Execute()
        {
            try
            {
                if (_matches == null || !_matches.Any())
                {
                    return;
                }

                foreach (var chunk in _matches.Chunk(ChunkSize))
                {
                    var ids = chunk.Select(m => m.MatchId);

                    var matchIds = string.Join(",", ids);

                    _service.Rule(matchIds);
                }
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                   .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                   .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error($"Scanned Office IP service - Execute Exeption {logMessage}");
            }
        }
    }
}