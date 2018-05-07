namespace Fanex.NAP.FS.Library.Models
{
    public class SuspiciousCustomerReportModel
    {
        public int CustId { get; set; }

        public string Username { get; set; }

        public bool IsHedge { get; set; }

        public bool IsDirty { get; set; }

        public bool IsDelinquent { get; set; }

        public bool HasNote { get; set; }

        public bool IsVerified { get; set; }

        public string Evidence { get; set; }

        public int TotalViolation { get; set; }

        public int BetCount { get; set; }

        public int ViolatedCases { get; set; }

        public string Assignee { get; set; }
    }
}