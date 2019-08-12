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
            var criteria = new TransactionHistoryGetCriteria
            {
                ip_AccountId = filter.AccountId,
                ip_FromDate = new DateTime(2019, 6, 12),
                ip_Skip = 0,
                ip_Limit = 20
            };
        
            return DataService.GetData<Transaction>(criteria);
        }

        #region Test
        //private IEnumerable<Transaction> GetTransactionHistory_Test(TransactionHistoryFilter filter)
        //{
        //    var transactions = new List<Transaction>();

        //    for (int i = 1; i < 11; i++)
        //    {
        //        var transaction = new Transaction();
        //        transaction.TransDate = DateTime.Now;
        //        transaction.Device = new Device();
        //        transaction.Device.DeviceNo = "d6e3d7efc7ca44e78d9e8f960300083" + i;
        //        transaction.Device.OSName = "iPhone";
        //        transaction.RobotTracking = "Human";                
        //        transaction.ActionResult = "login->successfully";
        //        transaction.Browser = "Safari";
        //        transaction.Domain = "iphone.9999999.com";
        //        transaction.IPAddress = "221.197.55.154";
        //        transaction.Country = "China";
        //        transaction.Region = "Tianjin";
        //        transaction.City = "Tianjin";
        //        transaction.ServiceProvider = "China Unicorn Tianjin Province Network";

        //        if (i % 2 == 0)
        //        {
        //            transaction.Flagged = Models.DeviceFlagged.Green;
        //        }
        //        else
        //        {
        //            transaction.Flagged = Models.DeviceFlagged.Gray;
        //        }

        //        transactions.Add(transaction);
        //    }

        //    return transactions;
        //}

        #endregion
    }
}