using Fanex.Data.Repository;

namespace Fanex.NAP.FS.Websites.Criteria.FraudCase.DelayedGoal
{
    public class DelayedGoalCustomerAddCriteria : CriteriaBase
    {
        public string Username { get; set; }

        public int UserId { get; set; }

        public override string GetSettingKey()
        {
            return "FS_DelayGoalAccount_Insert";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}