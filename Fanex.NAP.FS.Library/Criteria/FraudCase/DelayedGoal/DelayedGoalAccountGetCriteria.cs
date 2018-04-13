namespace Fanex.NAP.FS.Library.Criteria.FraudCase.DelayedGoal
{
    using Fanex.Data.Repository;

    public class DelayedGoalAccountGetCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "FS_DelayGoalAccount_GetAll";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}