namespace Fanex.NAP.FS.Library.Criteria.SuspiciousCustomer
{
    using Fanex.Data.Repository;

    public class SuspiciousCustomerDetailsLogNoteGetCriteria : CriteriaBase
    {
        public int CustId { get; set; }

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