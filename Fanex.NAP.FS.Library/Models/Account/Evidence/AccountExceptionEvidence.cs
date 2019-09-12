namespace Fanex.NAP.FS.Library.Models.Account.Evidence
{
    public class AccountExceptionEvidence
    {
        public int MainAccountId { get; set; }

        public string MainAccountUsername { get; set; }

        public int RootAccountId { get; set; }

        public string RootAccountUsername { get; set; }

        public int TargetEvidenceID { get; set; }        

        public string EviCode { get; set; }

        public bool IsActive { get; set; }
    }
}