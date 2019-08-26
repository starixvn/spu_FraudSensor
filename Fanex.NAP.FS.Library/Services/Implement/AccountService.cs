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
    using Fanex.NAP.FS.Library.Core.Helpers;
    using System.Threading.Tasks;
    using Fanex.NAP.FS.Library.Core.Helpers.Api;
    using Fanex.NAP.FS.Library.Core.Consts;
    using Fanex.NAP.FS.Library.ViewModels.Evidence;

    public class AccountService : IAccountService
    {
        public IEnumerable<SubscriberAssignment> GetSubscribers(long userId)
        {
            var criteria = new SubscriberGetCriteria
            {
                ip_userId = userId
            };

            var rawSubscribers = DataService.GetData<SubscriberAssignment>(criteria).OrderBy(c => c.SubscriberName).ThenBy(c => c.SubscriberID);
            
            if (CheckIsMasterSubscriber(rawSubscribers))
            {
                return rawSubscribers;
            }

            return GetSubscribers(rawSubscribers);
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

        public AccountDetail GetAccountDetail(int accountId, int custId)
        {
            var data = new AccountDetail();
            data.AccountInfo = new Account();
            data.AccountInfo.Id = accountId;
            data.AccountInfo.CustId = custId;
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
            var custid_test = 1;

            var customers = GetCustomersFromAPI(custid_test, filter.DeviceAwayOne);

            return GetAssociatedAccountByAccounts_Test(filter);
        }

        public IEnumerable<AssociatedDeviceByAccount> GetAssociatedDevicesByAccount(AssociatedDeviceByAccountFilter filter)
        {
            return GetAssociatedDevicesByAccount_Test(filter);
        }

        public IEnumerable<Evidence> GetEvidences()
        {
            return GetEvidences_Test().OrderBy(c=>c.EvidenceGroupID).ThenBy(c=>c.EvidenceTypeCode);
        }

        #region Private Methods
        private static bool CheckIsMasterSubscriber(IEnumerable<SubscriberAssignment> subscribers)
        {
            if (subscribers != null && subscribers.Any())
            {
                return subscribers.ToList()[0].IsMaster == 1;
            }

            return false;
        }

        private static IEnumerable<SubscriberAssignment> GetSubscribers(IEnumerable<SubscriberAssignment> rawSubscribers)
        {
            var subscribers = new List<SubscriberAssignment>();

            foreach (SubscriberAssignment subscriber in rawSubscribers)
            {
                if (subscriber.isAssigned == 1)
                {
                    subscribers.Add(subscriber);
                }
            }

            return subscribers;
        }

        private static IEnumerable<CustomerByGraphDB> GetCustomersFromAPI(int custId, bool deviceAwayOne)
        {
            var configUrl = deviceAwayOne ? Consts.GraphDBGetAssociationOneLevel : Consts.GraphDBGetAssociationNLevel;

            var url = AppSettings.Get(configUrl);

            var customers = ApiHelper.Post<IEnumerable<CustomerByGraphDB>>(url, custId);

            Task.WhenAll(customers);

            return customers.Result;
        }
        #endregion

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

        private IEnumerable<Evidence> GetEvidences_Test()
        {
            var evidences = new List<Evidence>();

            for (int i = 1; i < 6; i++)
            {
                evidences.Add(new Evidence
                {
                    EvidenceGroupID = 1,
                    EvidenceGroupName = "Monitoring",
                    EvidenceTypeID = i,
                    EvidenceTypeCode = "1." + i,
                    EvidenceTypeName = "VIP Smart Punter " + i
                });
            }

            for (int i = 1; i < 5; i++)
            {
                evidences.Add(new Evidence
                {
                    EvidenceGroupID = 2,
                    EvidenceGroupName = "Soccer Fraud",
                    EvidenceTypeID = i,
                    EvidenceTypeCode = "2." + i,
                    EvidenceTypeName = "Abnormal Betting " + i
                });
            }

            return evidences;
        }

        #endregion
    }
}