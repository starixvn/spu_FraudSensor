namespace Fanex.NAP.FS.WinService
{
    using Library.DTO;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public class FlaggedAccountExecutor : FraudCaseExecutor
    {
        private IEnumerable<Match> _matches;

        private readonly List<Task> _threads;

        public FlaggedAccountExecutor(FraudCase fraudCase) : base(fraudCase)
        {
            _matches = new List<Match>();
            _threads = new List<Task>();
        }

        protected override void ClearOldResult()
        {
            if (_matches != null && _matches.Any())
            {
                _matches = Enumerable.Empty<Match>();
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
            var coreCount = Environment.ProcessorCount;
            var matchCount = _matches.Count();
            var count = 0;

            for (; count < matchCount; count++)
            {
                if (count >= coreCount)
                {
                    break;
                }

                var match = _matches.ElementAt(count);

                var thread = ExecuteSingle(match);

                _threads.Add(thread);

                Thread.Sleep(10);
            }

            var threadsHaveDone = 0;

            while (threadsHaveDone < matchCount)
            {
                var index = Task.WaitAny(_threads.ToArray());

                threadsHaveDone++;
                _threads.RemoveAt(index);

                if (count >= matchCount)
                {
                    continue;
                }

                var match = _matches.ElementAt(count);
                var thread = ExecuteSingle(match);

                _threads.Add(thread);

                count++;

                Thread.Sleep(10);
            }
        }

        private Task ExecuteSingle(Match match)
        {
            var thread = Task.Factory.StartNew(m =>
            {
                try
                {
                    _fraudSensorTaskService.FlaggedAccountExecuteRule(((Match)m).MatchId.ToString());

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
            }, match);

            return thread;
        }
        #endregion
    }
}
