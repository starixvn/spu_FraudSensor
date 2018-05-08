namespace Fanex.NAP.FS.Library.Models.SuspiciousInfo
{
    using System;


    public class SuspiciousCustomerFraudCaseModel
    {
        public int FraudCaseId { get; set; }

        public string FraudCaseName { get; set; }

        public int Violations { get; set; }

        public int Verified { get; set; }

        public int Unverified { get; set; }

        public DateTime LastVerifiedDate { get; set; }

        public string LastVerifiedBy { get; set; }
    }
}