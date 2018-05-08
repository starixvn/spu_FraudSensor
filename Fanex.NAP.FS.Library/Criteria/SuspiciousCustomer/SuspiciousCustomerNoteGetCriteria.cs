namespace Fanex.NAP.FS.Library.Criteria.SuspiciousCustomer
{
    using Fanex.Data.Repository;

    public class SuspiciousCustomerNoteGetCriteria : CriteriaBase
    {
        public int CustId { get; set; }

        public SuspiciousCustomerNoteGetCriteria(int custId)
        {
            CustId = custId;
        }

        public override string GetSettingKey()
        {
            return "FS_Tbl_CustomerLog_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}