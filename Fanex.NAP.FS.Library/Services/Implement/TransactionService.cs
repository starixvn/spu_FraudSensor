using System.Collections.Generic;
using Fanex.NAP.FS.Library.Models.Transaction;

namespace Fanex.NAP.FS.Library.Services
{
    public class TransactionService : ITransactionService
    {
        public IEnumerable<Transaction> GetTransactionHistory(int accountId)
        {
            return new List<Transaction>();
        }
    }
}
