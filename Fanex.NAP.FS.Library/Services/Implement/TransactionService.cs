namespace Fanex.NAP.FS.Library.Services
{
    using System.Collections.Generic;
    using Fanex.NAP.FS.Library.Models.Transaction;
    using Fanex.NAP.FS.Library.ViewModels.Transaction;
    using Fanex.NAP.FS.Library.Criteria.Transaction.History;
    using Fanex.NAP.FS.Library.Core.BaseClasses;
    using Fanex.NAP.FS.Library.Criteria.Transaction;
    using System.Linq;

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

        public IPInformation GetIPInformation(int ipRangeLocationId)
        {
            var criteria = new IPInformationGetByRangeLocationCriteria
            {
                ip_IPRangeLocationID = ipRangeLocationId
            };

            var data = DataService.GetData<IPInformation>(criteria);

            if (data.Any())
            {
                return data.ToList()[0];
            }

            return new IPInformation();
        }
    }
}