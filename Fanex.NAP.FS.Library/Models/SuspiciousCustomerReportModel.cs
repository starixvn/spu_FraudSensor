namespace Fanex.NAP.FS.Library.Models
{
    public class SuspiciousCustomerReportModel
    {
        public string Username { get; set; }

        public bool IsHedge { get; set; }

        public bool IsDirty { get; set; }

        public bool IsDelinquency { get; set; }

        public bool HasNote { get; set; }

        public bool IsVerified { get; set; }

        public string Evidence { get; set; }

        public int TotalViolations { get; set; }

        public int BetCount { get; set; }

        public int ViolatedCases { get; set; }

        public string Assignees { get; set; }
    }
}
