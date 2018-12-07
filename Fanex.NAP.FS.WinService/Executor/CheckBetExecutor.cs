namespace Fanex.NAP.FS.WinService.Executor
{
    using Fanex.NAP.FS.Library.Core.BaseClasses;
    using Fanex.NAP.FS.Library.Criteria.FraudSensorTask;
    using Fanex.NAP.FS.Library.DTO;

    [FraudCase(Library.Models.FraudCaseId.CheckBet)]
    public class CheckBetExecutor : FraudCaseExecutor
    {
        public CheckBetExecutor(FraudCase fraudCase) : base(fraudCase)
        {
        }

        protected override void ClearOldResult()
        {
        }

        protected override void ExecutePrecondition()
        {
        }

        protected override void ExecuteRule()
        {
            DataService.Execute(new CheckBetRuleCriteria());
        }

        protected override void TakeNextAction()
        {
        }
    }
}