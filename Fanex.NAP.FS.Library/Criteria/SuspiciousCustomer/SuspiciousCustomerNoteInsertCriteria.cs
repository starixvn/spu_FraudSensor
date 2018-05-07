namespace Fanex.NAP.FS.Library.Criteria.SuspiciousCustomer
{
    using Fanex.Data.Repository;

    public class SuspiciousCustomerNoteInsertCriteria : CriteriaBase
    {
        public int CustId { get; set; }

        public string Note { get; set; }

        public int UserId { get; set; }

        public override string GetSettingKey()
        {
            return "FS_Tbl_CustomerLog_Insert";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}