namespace Fanex.NAP.FS.Websites.Criteria.FraudCase.DelayedGoal
{
    using Fanex.Data.Repository;

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