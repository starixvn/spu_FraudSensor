namespace Fanex.NAP.FS.Library.ViewModels.SubscriberAccount
{
    using Fanex.NAP.FS.Library.Models.Subscriber;

    public class SubscriberAccountDetail
    {
        public SubscriberAccountInfo AccountInfo { get; set; }

        public int TotalChainsToEvidence { get; set; }

        public int TotalRetractedEvidence { get; set; }

        public int TotalException { get; set; }

        public int TotalAssociatedAcount { get; set; }

        public int TotalAssociatedDevice { get; set; }

        public int TotalTransactionHistory { get; set; }
    }
}