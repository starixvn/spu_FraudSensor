namespace Fanex.NAP.FS.Library.Criteria.SuspiciousCustomer
{
    using Data.Repository;
    using Fanex.NAP.FS.Library.Core.BaseClasses;

    public class SuspiciousCustomerUpdateCriteria : CommandBase
    {
        public int CustId { get; set; }

        public int UserId { get; set; }

        public bool? IsDelinquent { get; set; }

        public bool? IsHedge { get; set; }

        public bool? IsDirty { get; set; }

        public override string GetSettingKey()
        {
            return "FS_Tbl_SuspiciousCustomer_Update";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}