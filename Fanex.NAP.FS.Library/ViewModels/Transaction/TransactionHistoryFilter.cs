namespace Fanex.NAP.FS.Library.ViewModels.Transaction
{
    using Fanex.NAP.FS.Library.Models;

    public class TransactionHistoryFilter
    {
        public int AccountId { get; set; }

        public int ip_Skip { get; set; }

        public int ip_Limit { get; set; }

        public TransactionHistoryLength ReportLength { get; set; }
    }
}