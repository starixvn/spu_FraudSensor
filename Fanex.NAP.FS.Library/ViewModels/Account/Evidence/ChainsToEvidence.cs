namespace Fanex.NAP.FS.Library.ViewModels.Account.Evidence
{  
    using System.Collections.Generic;

    public class ChainsToEvidence
    {
        public IEnumerable<ChainNodeRoot> ChainsRoot { get; set; }

        public IEnumerable<ChainNode> Chains { get; set; }   
    }
}