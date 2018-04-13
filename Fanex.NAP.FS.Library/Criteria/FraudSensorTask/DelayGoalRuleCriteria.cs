namespace Fanex.NAP.FS.Library.Criteria.FraudSensorTask
{
    using Data.Repository;

    public class DelayGoalRuleCriteria : CriteriaBase
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