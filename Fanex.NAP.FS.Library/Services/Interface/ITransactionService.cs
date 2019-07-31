namespace Fanex.NAP.FS.Library.Services
{
    using Fanex.NAP.FS.Library.Models.Transaction;
    using System.Collections.Generic;

    public interface ITransactionService
    {
        IEnumerable<Transaction> GetTransactionHistory(int accountId);
    }
}