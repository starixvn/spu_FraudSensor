namespace Fanex.NAP.FS.Service
{
    using Fanex.Data.Repository;

    public class DelayGoalRuleCommand: NonQueryCommand
    {
        public string MatchIds { get; set; }

        public override string GetSettingKey()
        {
            return "FS_FraudCase_DelayGoal_Rule";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
