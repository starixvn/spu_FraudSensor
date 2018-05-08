namespace Fanex.NAP.FS.Library.Criteria.SuspiciousCustomer
{
    using Fanex.Data.Repository;

    public class SuspiciousCustomerAssigneeGetCriteria : CriteriaBase
    {
        public int CustId { get; set; }

        public SuspiciousCustomerAssigneeGetCriteria(int custId)
        {
            CustId = custId;
        }

        public override string GetSettingKey()
        {
            return "FS_Tbl_CustomerMonitorAssignment_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}