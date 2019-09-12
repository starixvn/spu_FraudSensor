namespace Fanex.NAP.FS.Library.ViewModels.Account.Evidence
{  
    using System.Collections.Generic;

    public class ChainsToEvidence
    {
        public IEnumerable<ChainNode> ChainsDirected { get; set; }

        public IEnumerable<ChainNode> ChainsAffected { get; set; }   
    }
}