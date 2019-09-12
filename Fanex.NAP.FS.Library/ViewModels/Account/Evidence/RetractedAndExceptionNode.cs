namespace Fanex.NAP.FS.Library.ViewModels.Account.Evidence
{
    public class RetractedAndExceptionNode
    {
        public int AffectedAccountId { get; set; }

        public string AffectedUserName { get; set; }

        public int DirectedAccountId { get; set; }

        public string DirectedUserName { get; set; }

        public string EvidenceCode { get; set; }

        public bool IsActive { get; set; }
    }
}