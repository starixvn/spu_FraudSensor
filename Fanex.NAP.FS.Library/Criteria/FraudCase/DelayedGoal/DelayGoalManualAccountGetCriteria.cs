namespace Fanex.NAP.FS.Library.Criteria.FraudCase.DelayedGoal
{
    using Data.Repository;

    public class DelayGoalManualAccountGetCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "FS_DelayGoalAccount_GetManualAccount";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}