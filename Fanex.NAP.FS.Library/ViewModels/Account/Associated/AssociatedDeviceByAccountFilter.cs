namespace Fanex.NAP.FS.Library.ViewModels.Account.Associated
{
    using Fanex.NAP.FS.Library.Models;

    public class AssociatedDeviceByAccountFilter
    {
        public int AccountId { get; set; }

        public bool UsedByAccountCurrent { get; set; }

        public bool UsedByAccountAssociated { get; set; }

        public EvidenceStatus EvidenceStatus { get; set; }        
    }
}