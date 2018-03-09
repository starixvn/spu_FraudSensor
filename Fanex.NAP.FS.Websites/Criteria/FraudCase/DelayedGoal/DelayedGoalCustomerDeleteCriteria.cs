using Fanex.Data.Repository;
using System;

namespace Fanex.NAP.FS.Websites.Criteria.FraudCase.DelayedGoal
{
    public class DelayedGoalCustomerDeleteCriteria : CriteriaBase
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