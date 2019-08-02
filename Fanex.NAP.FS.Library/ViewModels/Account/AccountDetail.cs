namespace Fanex.NAP.FS.Library.ViewModels.Account
{
    using Fanex.NAP.FS.Library.Models.Account;

    public class AccountDetail
    {
        public AccountInfo AccountInfo { get; set; }

        public int TotalChainsToEvidence { get; set; }

        public int TotalRetractedEvidence { get; set; }

        public int TotalException { get; set; }

        public int TotalAssociatedAcount { get; set; }

        public int TotalAssociatedDevice { get; set; }

        public int TotalTransactionHistory { get; set; }
    }
}