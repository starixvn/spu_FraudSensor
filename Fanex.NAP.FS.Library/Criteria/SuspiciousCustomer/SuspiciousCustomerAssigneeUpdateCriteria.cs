namespace Fanex.NAP.FS.Library.Criteria.SuspiciousCustomer
{
    using Fanex.Data.Repository;
    using Fanex.NAP.FS.Library.Core.BaseClasses;

    public class SuspiciousCustomerAssigneeUpdateCriteria : CommandBase
    {
        public int CustId { get; set; }

        public int? AssigneeId { get; set; }

        public int UserId { get; set; }

        public override string GetSettingKey()
        {
            return "FS_SuspiciousCustomer_Assignee_Update";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}