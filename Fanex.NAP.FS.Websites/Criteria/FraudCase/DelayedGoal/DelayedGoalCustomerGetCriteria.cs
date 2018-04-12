namespace Fanex.NAP.FS.Websites.Criteria.FraudCase.DelayedGoal
{
    using Fanex.Data.Repository;

    public class DelayedGoalCustomerGetCriteria : CriteriaBase
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