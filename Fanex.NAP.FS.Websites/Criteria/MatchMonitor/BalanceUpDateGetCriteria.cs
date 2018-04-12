namespace Fanex.NAP.FS.Websites.Criteria.MatchMonitor
{
    using Fanex.Data.Repository;

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