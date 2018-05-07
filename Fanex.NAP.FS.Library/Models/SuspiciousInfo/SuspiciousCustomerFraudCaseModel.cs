namespace Fanex.NAP.FS.Library.Models.SuspiciousInfo
{
    using System;


    public class SuspiciousCustomerFraudCaseModel
    {
        public int FraudCaseId { get; set; }

        public string FraudCaseName { get; set; }

        public int VerifyCounter { get; set; }

        public int UnverifyCounter { get; set; }

        public DateTime LastVerifyDate { get; set; }

        public int LastVerifyById { get; set; }

        public string LastVerifyByName { get; set; }
    }
}