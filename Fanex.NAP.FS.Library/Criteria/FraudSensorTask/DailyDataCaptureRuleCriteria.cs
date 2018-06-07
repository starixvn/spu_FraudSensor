namespace Fanex.NAP.FS.Library.Criteria.FraudSensorTask
{
    using Fanex.Data.Repository;

    public class DailyDataCaptureRuleCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "FS_FraudCase_DailyDataMonitoring_Rule";

        public override bool IsValid() => true;
    }
}