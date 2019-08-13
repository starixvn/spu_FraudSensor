namespace Fanex.NAP.FS.Library.Services
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Fanex.NAP.FS.Library.Models.Account;
    using Fanex.NAP.FS.Library.ViewModels.Account;
    using Fanex.NAP.FS.Library.ViewModels.Account.Associated;
    using Fanex.NAP.FS.Library.Models.Subscriber;
    using Fanex.NAP.FS.Library.Models;
    using Fanex.NAP.FS.Library.Criteria.SubscriberAssignment;
    using Fanex.NAP.FS.Library.Core.BaseClasses;
    using Fanex.NAP.FS.Library.Criteria.Account;
    using Fanex.NAP.FS.Library.ViewModels.Account.Search;

    public class AccountService : IAccountService
    {
        public IEnumerable<Subscriber> GetSubscribers(long userId)
        {
            var criteria = new SubscriberGetCriteria
            {
                ip_userId = userId
            };

            return DataService.GetData<Subscriber>(criteria).OrderBy(c => c.SubscriberName).ThenBy(c => c.SubscriberID);
        }

        public IEnumerable<SubscriberAccount> SearchAccounts(SearchAccountFilter filter)
        {
            var criteria = new SearchAccountCriteria
            {
                ip_SubscriberIDs = filter.Subscribers,
                ip_Username = filter.Key,
                ip_UserId = filter.UserId
            };

            return DataService.GetData<SubscriberAccount>(criteria)
                            .OrderBy(a => a.SubscriberName)
                            .ThenBy(a => a.UserName);
        }

        public AccountDetail GetAccountDetail(int accountId)
        {
            var data = new AccountDetail();
            data.AccountInfo = new Account();
            data.AccountInfo.Id = accountId;
            data.AccountInfo.UserName = "fpsa_pgmthai";
            data.AccountInfo.UserName2 = "fpsa2_pgmthai";
            data.AccountInfo.Status = AccountStatus.Active.ToString();
            data.AccountInfo.FirstLogin = DateTime.Now;
            data.AccountInfo.LastLogin = DateTime.Now;
            data.AccountInfo.FrequencyLogin = 1;
            data.TotalChainsToEvidence = 5;
            data.TotalRetractedEvidence = 3;
            data.TotalException = 5;
            data.TotalAssociatedAcount = 10;
            data.TotalAssociatedDevice = 20;
            data.TotalTransactionHistory = 100;

            return data;
        }

        public IEnumerable<AssociatedAccountByAccount> GetAssociatedAccountByAccounts(AssociatedAccountByAccountFilter filter)
        {
            return GetAssociatedAccountByAccounts_Test(filter);
        }

        public IEnumerable<AssociatedDeviceByAccount> GetAssociatedDevicesByAccount(AssociatedDeviceByAccountFilter filter)
        {
            return GetAssociatedDevicesByAccount_Test(filter);
        }

        #region Test

        private IEnumerable<AssociatedAccountByAccount> GetAssociatedAccountByAccounts_Test(AssociatedAccountByAccountFilter filter)
        {
            var lstAssociated = new List<AssociatedAccountByAccount>();

            for (int i = 1; i < 6; i++)
            {
                var accountAssociated = new AssociatedAccountByAccount();
                accountAssociated.AccountAssociated = new Account();
                accountAssociated.AccountAssociated.Id = i;
                accountAssociated.AccountAssociated.UserName = "fpsa_pmgthai_" + i;
                accountAssociated.AccountAssociated.UserName2 = "fpsa_pmgthai2_" + i;
                if (i % 2 == 0)
                {
                    accountAssociated.AccountAssociated.FraudStatus = FraudStatus.Flagged.ToString();
                }
                else
                {
                    accountAssociated.AccountAssociated.FraudStatus = FraudStatus.Clean.ToString();
                }
                accountAssociated.AccountAssociated.Status = AccountStatus.Active.ToString();
                
                accountAssociated.AccountAssociated.FirstLogin = DateTime.Now;
                accountAssociated.AccountAssociated.LastLogin = DateTime.Now;
                accountAssociated.TotalSharedDeviceLogin = 20;
                accountAssociated.AssociatedDate = DateTime.Now;
                accountAssociated.GraphURL = "Graph URL";

                lstAssociated.Add(accountAssociated);
            }

            return lstAssociated;
        }

        private IEnumerable<AssociatedDeviceByAccount> GetAssociatedDevicesByAccount_Test(AssociatedDeviceByAccountFilter filter)
        {
            var lstAssociated = new List<AssociatedDeviceByAccount>();

            for (int i = 1; i < 6; i++)
            {
                var deviceAssociated = new AssociatedDeviceByAccount();
                deviceAssociated.DeviceNo = "08d99c65629142b9903ab56b2f3511ba";                
                deviceAssociated.FirstUsed = DateTime.Now;
                deviceAssociated.LastUsed = DateTime.Now;
                deviceAssociated.AssociatedDate = DateTime.Now;
                deviceAssociated.TotalUses = 100 + i;
                deviceAssociated.GraphURL = "url 123456";
                if (i % 2 == 0)
                {
                    deviceAssociated.FraudStatus = FraudStatus.Flagged.ToString();
                }
                else
                {
                    deviceAssociated.FraudStatus = FraudStatus.Clean.ToString();
                }

                lstAssociated.Add(deviceAssociated);
            }

            return lstAssociated;
        }

        #endregion
    }
}