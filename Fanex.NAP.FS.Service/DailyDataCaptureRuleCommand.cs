namespace Fanex.NAP.FS.Service
{
    using Fanex.Data.Repository;

    public  class DailyDataCaptureRuleCommand: NonQueryCommand
    {
        public override string GetSettingKey() => "FS_FraudCase_DailyDataMonitoring_Rule";

        public override bool IsValid() => true;
    }
}
