using Fanex.Data.Repository;

namespace Fanex.NAP.FS.Websites.Criteria.MatchMonitor
{
    public class BalanceUpDateGetCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "Age_SelBalanceUpDay";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}