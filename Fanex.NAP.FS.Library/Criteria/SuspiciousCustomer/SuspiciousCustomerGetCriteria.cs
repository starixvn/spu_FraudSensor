namespace Fanex.NAP.FS.Library.Criteria.SuspiciousCustomer
{
    using Data.Repository;

    public class SuspiciousCustomerGetCriteria : CriteriaBase
    {
        public int CustId { get; set; }

        public bool? IsDelinquent { get; set; }

        public bool? IsHedge { get; set; }

        public bool? IsDirty { get; set; }

        public bool IsShowSpecialCurrencies { get; set; }

        public override string GetSettingKey()
        {
            return "FS_Tbl_SuspiciousCustomer_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}