namespace Fanex.NAP.FS.Library.Services
{
    using System.Collections.Generic;
    using Fanex.NAP.FS.Library.Models.Transaction;
    using Fanex.NAP.FS.Library.ViewModels.Transaction;
    using Fanex.NAP.FS.Library.Criteria.Transaction.History;
    using Fanex.NAP.FS.Library.Core.BaseClasses;    

    public class TransactionService : ITransactionService
    {
        public IEnumerable<Transaction> GetTransactionHistory(TransactionHistoryFilter filter)
        {
            var criteria = new TransactionHistoryGetByCustIdCriteria
            {
                ip_accountId = filter.AccountId,
                ip_historyRange = (int)filter.ReportLength
            };

            return DataService.GetData<Transaction>(criteria);
        }
    }
}