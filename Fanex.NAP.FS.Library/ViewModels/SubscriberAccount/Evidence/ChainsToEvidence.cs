namespace Fanex.NAP.FS.Library.ViewModels.SubscriberAccount.Evidence
{
    using Fanex.NAP.FS.Library.Models.Subscriber;
    using System.Collections.Generic;

    public class ChainsToEvidence
    {
        public string CentralPoint { get; set; }

        public List<EvidenceAccount> Evidences { get; set; }

        public List<ChainLevel> Level1 { get; set; }

        public List<ChainLevel> Level2 { get; set; }

        public string Subscriber { get; set; }

        public string ChainsEvidenceType { get; set; }

        public string GetLabel(int level)
        {
            switch (level)
            {
                case 1:
                    return (ChainsEvidenceType == "Account") ? "Device" : "Account";
                case 2:
                    return ChainsEvidenceType;
                default:
                    return string.Empty;
            }
        }
    }
}