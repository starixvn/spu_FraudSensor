namespace Fanex.NAP.FS.Library.Services
{
    using System;
    using System.Collections.Generic;
    using Fanex.NAP.FS.Library.Models.Transaction;
    using Fanex.NAP.FS.Library.ViewModels.Transaction;
    using Fanex.NAP.FS.Library.Models.Device;
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

        #region Test
        private IEnumerable<Transaction> GetTransactionHistory_Test(TransactionHistoryFilter filter)
        {
            var transactions = new List<Transaction>();

            for (int i = 1; i < 11; i++)
            {
                var transaction = new Transaction();
                transaction.TransDate = DateTime.Now;
                transaction.DeviceID = 123456;
                transaction.DeviceCode = "d6e3d7efc7ca44e78d9e8f960300083" + i;
                //transaction.OSName = "iPhone";
                if (i % 2 == 0)
                {
                    transaction.RobotTracking = "Human";
                }
                else
                {
                    transaction.RobotTracking = "Robot (83%)";
                }
                transaction.ActionResult = "login->successfully";
                transaction.Browser = "Safari";
                transaction.Domain = "iphone.9999999.com";
                transaction.IP = "221.197.55.154";
                transaction.Country = "China";
                transaction.Region = "Tianjin";
                transaction.City = "Tianjin";
                transaction.ServiceProvider = "China Unicorn Tianjin Province Network";

                if (i % 2 == 0)
                {
                    transaction.Flagged = Models.DeviceFlagged.Green;
                }
                else
                {
                    transaction.Flagged = Models.DeviceFlagged.Gray;
                }

                transactions.Add(transaction);
            }

            return transactions;
        }

        #endregion
    }
}