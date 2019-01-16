namespace Fanex.NAP.FS.WinService
{
    using Fanex.NAP.FS.Library.Core.BaseClasses;
    using Fanex.NAP.FS.Library.Criteria.FraudSensorTask;
    using Fanex.NAP.FS.Library.DTO;

    [FraudCase(Library.Models.FraudCaseId.DailyDataCapture)]
    public class DailyDataCaptureExecutor : FraudCaseExecutor
    {
        public DailyDataCaptureExecutor(FraudCase fraudCase) : base(fraudCase)
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
            DataService.Execute(new DailyDataCaptureRuleCriteria());
        }

        protected override void TakeNextAction()
        {
        }
    }
}