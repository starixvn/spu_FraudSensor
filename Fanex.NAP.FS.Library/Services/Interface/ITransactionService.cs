namespace Fanex.NAP.FS.Library.Services
{
    using Fanex.NAP.FS.Library.Models.Transaction;
    using Fanex.NAP.FS.Library.ViewModels.Transaction;
    using System.Collections.Generic;

    public interface ITransactionService
    {
        IEnumerable<Transaction> GetTransactionHistory(TransactionHistoryFilter filter);

        IPInformation GetIPInformation(string ipAddress);
    }
}