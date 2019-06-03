namespace Fanex.NAP.FS.Library.DTO
{
    public class FraudCase
    {
        public int FraudCaseId { get; set; }

        public string FraudCaseName { get; set; }

        public string FraudCaseDesc { get; set; }
        /// <summary>
        /// Expressed in seconds.
        /// </summary>
        public int FraudCaseInterval { get; set; }

        public bool IsActive { get; set; }

        public bool IsTest { get; set; }
    }
}