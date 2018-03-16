namespace Fanex.NAP.FS.WinService
{
    using Library.DTO;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class BigStakeNearGoalExecutor : FraudCaseExecutor
    {
        private IEnumerable<Match> _matches;

        private readonly List<Task> _threads;

        public BigStakeNearGoalExecutor(FraudCase fraudCase) : base(fraudCase)
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
            _matches = _fraudSensorTaskService.BigStakeNearGoalExecutePrecondition();
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
                foreach (var match in _matches)
                {
                    var thread = ExecuteSingle(match);

                    _threads.Add(thread);
                }

                Task.WaitAll(_threads.ToArray());
            }
        }

        private Task ExecuteSingle(Match match)
        {
            var thread = Task.Factory.StartNew(m =>
            {
                try
                {
                    _fraudSensorTaskService.BigStakeNearGoalExecuteRule(((Match)m).MatchId.ToString());

                    _errorTimes = 0;
                }
                catch (Exception exception)
                {
                    var error = "<b>Error:</b> " + exception.Message +
                              "<br><b>StackTrace:</b> " + exception.StackTrace;
                    var message = string.Format("<b>Problem with FraudCaseId = {0}, FraudCaseName = {1}, MatchId = {2}</b><br><br>", FraudCase.FraudCaseId, FraudCase.FraudCaseName, match.MatchId) + error;

                    if (_errorTimes++ < 10)
                    {
                        SendMailInform(message);
                    }
                }
            }, match);

            return thread;
        }
        #endregion
    }
}
