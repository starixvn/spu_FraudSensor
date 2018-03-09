using Fanex.Data.Repository;

namespace Fanex.NAP.FS.Websites.Criteria.FraudCase.DelayedGoal
{
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