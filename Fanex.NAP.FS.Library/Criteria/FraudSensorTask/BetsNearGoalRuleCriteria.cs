namespace Fanex.NAP.FS.Library.Criteria.FraudSensorTask
{
    using Data.Repository;

    public class BetsNearGoalRuleCriteria : CriteriaBase
    {
        public string MatchIds { get; set; }

        public override string GetSettingKey()
        {
            return "FS_FraudCase_BetsNearGoal_Rule";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}