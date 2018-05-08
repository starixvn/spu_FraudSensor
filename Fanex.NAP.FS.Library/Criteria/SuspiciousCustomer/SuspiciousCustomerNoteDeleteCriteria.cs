namespace Fanex.NAP.FS.Library.Criteria.SuspiciousCustomer
{
    using Fanex.Data.Repository;

    public class SuspiciousCustomerNoteDeleteCriteria : CriteriaBase
    {
        public int CustomerLogId { get; set; }

        public int UserId { get; set; }

        public override string GetSettingKey()
        {
            return "FS_Tbl_CustomerLog_Delete";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}