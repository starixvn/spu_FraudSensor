namespace Fanex.NAP.FS.Library.ViewModels.Account.Evidence
{
    using Fanex.NAP.FS.Library.Models.Account;
    using System.Collections.Generic;

    public class ChainsToEvidence
    {
        public string NodeNameRoot { get; set; }

        public List<AccountEvidence> Evidences { get; set; }

        public List<ChainNode> Chains { get; set; }      

        public string Subscriber { get; set; }

        public string ChainsEvidenceType { get; set; }
    }
}