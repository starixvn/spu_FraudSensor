namespace Fanex.NAP.FS.Library.ViewModels.Account.Evidence
{
    public class ChainNode
    {
        public int AffectedAccountId { get; set; }

        public string AffectedUserName { get; set; }

        public int DirectedAccountId { get; set; }

        public string DirectedUserName { get; set; }

        public string EvidenceCode { get; set; }
    }
}