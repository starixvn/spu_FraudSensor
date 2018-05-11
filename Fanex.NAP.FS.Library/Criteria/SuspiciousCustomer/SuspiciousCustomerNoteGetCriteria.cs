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
            return "FS_Rpt_SuspiciousCustomer_GetNotes";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}