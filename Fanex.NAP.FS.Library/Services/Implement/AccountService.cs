namespace Fanex.NAP.FS.Library.Services
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Threading.Tasks;
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
    using Fanex.NAP.FS.Library.Core.Helpers.Api;
    using Fanex.NAP.FS.Library.Core.Consts;
    using Fanex.NAP.FS.Library.ViewModels.Evidence;
    using Fanex.NAP.FS.Library.Criteria.Account.Associated;
    using Fanex.Data;

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

            var customersFromAPI = GetCustomersFromAPI(custid_test, filter.DeviceAwayOne);

            var customersIds = customersFromAPI.Select(c => c.CustId).Join();

            var accountsByCustomers = GetAssociatedAccountByCustomers(filter.AccountId, customersIds);

            var customerFraudSuspicious = GetCustomerFraudSuspicious(customersIds);

            var accountCollected = new AccountsCollected
            {
                CustomersFromAPI = customersFromAPI,
                AccountsByCustomers = accountsByCustomers,
                CustomerFraudSuspicious = customerFraudSuspicious
            };

            return GetAssociatedAccountByAccount(accountCollected, filter);
        }

        public IEnumerable<AssociatedDeviceByAccount> GetAssociatedDevicesByAccount(AssociatedDeviceByAccountFilter filter)
        {
            return GetAssociatedDevicesByAccount_Test(filter);
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
            //var configUrl = deviceAwayOne ? Consts.GraphDBGetAssociationOneLevel : Consts.GraphDBGetAssociationNLevel;

            //var url = AppSettings.Get(configUrl);

            //var customers = ApiHelper.Post<IEnumerable<CustomerByGraphDB>>(url, custId);

            //Task.WhenAll(customers);

            //return customers.Result;

            var listCustomers = new List<CustomerByGraphDB>();
            listCustomers.Add(new CustomerByGraphDB { CustId = 28625904, Username = "A1", Username2 = "B1" });
            listCustomers.Add(new CustomerByGraphDB { CustId = 28674088, Username = "A2", Username2 = "B2" });
            listCustomers.Add(new CustomerByGraphDB { CustId = 29103223, Username = "A3", Username2 = "B3" });
            listCustomers.Add(new CustomerByGraphDB { CustId = 29144584, Username = "A4", Username2 = "B4" });
            listCustomers.Add(new CustomerByGraphDB { CustId = 29179473, Username = "A5", Username2 = "B5" });
            listCustomers.Add(new CustomerByGraphDB { CustId = 29250278, Username = "A6", Username2 = "B6" });
            listCustomers.Add(new CustomerByGraphDB { CustId = 29963945, Username = "A7", Username2 = "B7" });
            listCustomers.Add(new CustomerByGraphDB { CustId = 29980403, Username = "A8", Username2 = "B8" });
            listCustomers.Add(new CustomerByGraphDB { CustId = 29983429, Username = "A9", Username2 = "B9" });
            listCustomers.Add(new CustomerByGraphDB { CustId = 29986217, Username = "A10", Username2 = "B10" });

            return listCustomers;
        }

        private static IEnumerable<AssociatedAccountByCustomer> GetAssociatedAccountByCustomers(int accountId, string customerIds)
        {
            //var criteria = new GetAssociatedAccountByAccountCriteria
            //{
            //     custIds = customerIds,
            //     rootAccountId = 2602
            //};

            //return DataService.GetData<AssociatedAccountByCustomer>(criteria);

            var idx = accountId;
            var now = DateTime.Now;
            var listCustomers = new List<AssociatedAccountByCustomer>();
            listCustomers.Add(new AssociatedAccountByCustomer { AccountID = idx++, CustID = 28625904, FirstLogin = now, LastLogin = now, AssociatedWithRootAccountOn = now, TotalLoginWithRootAccount = 1, EvidenceLevel = 0 });
            listCustomers.Add(new AssociatedAccountByCustomer { AccountID = idx++, CustID = 28674088, FirstLogin = now, LastLogin = now, AssociatedWithRootAccountOn = now, TotalLoginWithRootAccount = 2, EvidenceLevel = 0 });
            listCustomers.Add(new AssociatedAccountByCustomer { AccountID = idx++, CustID = 29103223, FirstLogin = now, LastLogin = now, AssociatedWithRootAccountOn = now, TotalLoginWithRootAccount = 3, EvidenceLevel = 0 });
            listCustomers.Add(new AssociatedAccountByCustomer { AccountID = idx++, CustID = 29144584, FirstLogin = now, LastLogin = now, AssociatedWithRootAccountOn = now, TotalLoginWithRootAccount = 4, EvidenceLevel = 1 });
            listCustomers.Add(new AssociatedAccountByCustomer { AccountID = idx++, CustID = 29179473, FirstLogin = now, LastLogin = now, AssociatedWithRootAccountOn = now, TotalLoginWithRootAccount = 5, EvidenceLevel = 3 });
            listCustomers.Add(new AssociatedAccountByCustomer { AccountID = idx, CustID = 29250278, FirstLogin = now, LastLogin = now, TotalLoginWithRootAccount = 6, EvidenceLevel = 2 });

            return listCustomers;
        }

        private CustomerFraudSuspicious GetCustomerFraudSuspicious(string customerIds)
        {
            var criteria = new GetCustomerFraudInfoCriteria
            {
                custIds = "28625904,28674088,29103223,29144584,29179473,29250278,29963945,29980403,29983429,29986217,30125119,30243383,30244853"
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (var query = db.QueryMultiple(criteria))
                {
                    var customerFrauds = query.Read<CustomerFraud>();

                    var customerSuspiciouses = query.Read<CustomerSuspicious>();
                   
                    return new CustomerFraudSuspicious
                    {
                        CustomerFrauds = customerFrauds,
                        CustomerSuspiciouses = customerSuspiciouses
                    };
                }
            }
        }

        private static IEnumerable<AssociatedAccountByAccount> GetAssociatedAccountByAccount(AccountsCollected accountsCollected, AssociatedAccountByAccountFilter filter)
        {
            return accountsCollected.AccountsByCustomers
                            .Where(customer=>customer.CompareFraudStatus(filter.FraudStatus)).AsEnumerable()
                            .Join(accountsCollected.CustomersFromAPI,
                                cusA => cusA.CustID,
                                cusB => cusB.CustId,
                                (cusA, cusB) => GetAssociatedAccount(cusA, cusB))
                            .Join(accountsCollected.CustomerFraudSuspicious.CustomerFrauds
                                .Where(customer=>customer.CompareStatus(filter.Status)),
                                cusA => cusA.CustID,
                                cusB => cusB.custid,
                                (cusA, cusB) => GetAssociatedAccount(cusA, cusB));
        }

        private static AssociatedAccountByAccount GetAssociatedAccount(AssociatedAccountByCustomer cusA, CustomerByGraphDB cusB)
        {
            return new AssociatedAccountByAccount
            {
                AccountID = cusA.AccountID,
                CustID = cusA.CustID,
                Username = cusB.Username,
                Username2 = cusB.Username2,
                LastLogin = cusA.LastLogin,
                FirstLogin = cusA.FirstLogin,
                AssociatedWithRootAccountOn = cusA.AssociatedWithRootAccountOn,
                TotalLoginWithRootAccount = cusA.TotalLoginWithRootAccount,
                EvidenceLevel = cusA.EvidenceLevel
            };
        }

        private static AssociatedAccountByAccount GetAssociatedAccount(AssociatedAccountByAccount cusA, CustomerFraud cusB)
        {
            return new AssociatedAccountByAccount
            {
                AccountID = cusA.AccountID,
                CustID = cusA.CustID,
                Username = cusA.Username,
                Username2 = cusA.Username2,
                LastLogin = cusA.LastLogin,
                FirstLogin = cusA.FirstLogin,
                AssociatedWithRootAccountOn = cusA.AssociatedWithRootAccountOn,
                TotalLoginWithRootAccount = cusA.TotalLoginWithRootAccount,
                EvidenceLevel = cusA.EvidenceLevel,
                Status = cusB.GetStatusName()
            };
        }
        #endregion

        #region Test       

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
                    deviceAssociated.FraudStatus = FraudStatus.Fraud.ToString();
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