namespace Fanex.NAP.FS.Library.Criteria.SuspiciousInfo
{
    using Fanex.Data.Repository;

    public class SuspicousCustomerCurrencyGetCriteria : CriteriaBase
    {
        public int CustId { get; set; }

        public override string GetSettingKey()
        {
            return "FS_CustomerInfo_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}