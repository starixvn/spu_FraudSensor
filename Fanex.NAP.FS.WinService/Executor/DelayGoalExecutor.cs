namespace Fanex.NAP.FS.WinService.Executor
{
    using Fanex.NAP.FS.Library.Core.BaseClasses;
    using Fanex.NAP.FS.Library.Core.Extensions;
    using Library.Core.Utilities;
    using Library.Criteria.FraudSensorTask;
    using Library.DTO;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    [FraudCase(Library.Models.FraudCaseId.DelayedGoal)]
    public class DelayGoalExecutor : FraudCaseExecutor
    {
        private IEnumerable<Match> _matches;

        private readonly List<Task> _threads;

        private const int ChunkSize = 100;

        private int _errorTimes;

        public DelayGoalExecutor(FraudCase fraudCase) : base(fraudCase)
        {
            _matches = new List<Match>();
            _threads = new List<Task>();
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
            _matches = DataService.GetData<Match>(new DelayGoalPreconditionCriteria());
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
                    DataService.Execute(new DelayGoalRuleCriteria { MatchIds = ((List<Match>)m).Join(a => a.MatchId) });

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