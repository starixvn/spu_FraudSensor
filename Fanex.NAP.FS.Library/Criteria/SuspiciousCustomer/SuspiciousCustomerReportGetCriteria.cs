namespace Fanex.NAP.FS.Library.Criteria
{
    using Data.Repository;
    using System;

    public class SuspiciousCustomerReportGetCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public string Username { get; set; }

        public bool? IsVerified { get; set; }

        public int? FraudCaseId { get; set; }

        public int? Classification { get; set; }

        public int? Assignee { get; set; }

        public override string GetSettingKey()
        {
            return "FD_SuspiciousCustomerReportGet";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
