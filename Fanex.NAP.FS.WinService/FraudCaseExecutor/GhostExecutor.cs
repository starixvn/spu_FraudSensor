namespace Fanex.NAP.FS.WinService
{
    using Library.DTO;

    public class GhostExecutor : FraudCaseExecutor
    {
        public GhostExecutor(FraudCase fraudCase) : base(fraudCase)
        {
        }

        protected override void ClearOldResult()
        {
            Stop();
        }

        protected override void ExecutePrecondition()
        {
        }

        protected override void ExecuteRule()
        {
        }

        protected override void TakeNextAction()
        {
        }
    }
}