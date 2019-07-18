namespace Fanex.NAP.FS.Library.Criteria.FraudCase.DelayedGoal
{
    using Fanex.Data.Repository;
    using Fanex.NAP.FS.Library.Core.BaseClasses;

    public class DelayedGoalAccountDeleteCriteria : CommandBase
    {
        public int CustId { get; set; }

        public int UserId { get; set; }

        public override string GetSettingKey()
        {
            return "FS_DelayGoalAccount_Delete";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}