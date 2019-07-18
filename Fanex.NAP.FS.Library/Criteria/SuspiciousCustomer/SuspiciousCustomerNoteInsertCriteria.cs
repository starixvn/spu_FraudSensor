namespace Fanex.NAP.FS.Library.Criteria.SuspiciousCustomer
{
    using Fanex.Data.Repository;
    using Fanex.NAP.FS.Library.Core.BaseClasses;

    public class SuspiciousCustomerNoteInsertCriteria : CommandBase
    {
        public int CustId { get; set; }

        public string LogDesc { get; set; }

        public int UserId { get; set; }

        public int CustLogType { get; set; }

        public int? CaseId { get; set; }

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