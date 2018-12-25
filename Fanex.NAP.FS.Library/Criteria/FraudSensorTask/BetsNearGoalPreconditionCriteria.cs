namespace Fanex.NAP.FS.Library.Criteria.FraudSensorTask
{
    using Data.Repository;

    public class BetsNearGoalPreconditionCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "FS_FraudCase_BetsNearGoal_PreCondition";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}