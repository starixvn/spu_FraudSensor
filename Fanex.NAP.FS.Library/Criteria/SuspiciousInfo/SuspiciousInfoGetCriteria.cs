namespace Fanex.NAP.FS.Library.Criteria.SuspiciousInfo
{
    using Fanex.Data.Repository;

    public class SuspiciousInfoGetCriteria : CriteriaBase
    {
        public int CustId { get; set; }

        public SuspiciousInfoGetCriteria(int custId)
        {
            CustId = custId;
        }

        public override string GetSettingKey()
        {
            return "FS_SuspiciousCustomer_FraudCases_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}