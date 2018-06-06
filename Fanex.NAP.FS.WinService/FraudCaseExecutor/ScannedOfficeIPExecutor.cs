namespace Fanex.NAP.FS.WinService
{
    using Fanex.Logging;
    using Fanex.NAP.FS.Library.Core.BaseClasses;
    using Fanex.NAP.FS.Library.Core.Utilities;
    using Fanex.NAP.FS.Library.Criteria.FraudSensorTask;
    using Fanex.NAP.FS.Library.DTO;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class ScannedOfficeIPExecutor : FraudCaseExecutor
    {
        private IEnumerable<Match> _matches;

        private readonly Queue<Task> _tasks = new Queue<Task>();

        public ScannedOfficeIPExecutor(FraudCase fraudCase) : base(fraudCase)
        {
        }

        protected override void ClearOldResult()
        {
            _matches = null;
            _tasks.Clear();
        }

        protected override void ExecutePrecondition()
        {
            _matches = DataService.GetData<Match>(new OfficeIPPreconditionCriteria());
        }

        protected override void ExecuteRule()
        {
            if (_matches != null && _matches.Any())
            {
                foreach (var chunk in _matches.Chunk(100))
                {
                    var ids = chunk.Select(m => m.MatchId);

                    _tasks.Enqueue(Task.Run(() => DataService.Execute(new OfficeIPRuleCriteria { MatchIds = string.Join(",", ids) })));
                }

                Task.WaitAll(_tasks.ToArray());

                foreach (var item in _tasks.Where(t => t.IsFaulted))
                {
                    Logger.Log.Error(item.Exception.Message, item.Exception);
                }
            }
        }

        protected override void TakeNextAction()
        {
        }
    }
}