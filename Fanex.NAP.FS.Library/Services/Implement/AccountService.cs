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
    using Fanex.NAP.FS.Library.Criteria.Account.Associated;
    using Fanex.Data;
    using Fanex.NAP.FS.Library.ViewModels.Account.GeneralInformation;
    using Fanex.NAP.FS.Library.Criteria.Account.GeneralInformation;
    using System.Text;
    using Fanex.NAP.FS.Library.Core.Resources;

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

        public AccountDetail GetAccountDetailGeneralInformation(int accountId, int custId)
        {
            var accountCollected = new GeneralAccountCollected();

            accountCollected.CustomerInformation = GetGeneralCustomerInformation(accountId);

            accountCollected.AssociatedByAccounts = GetAssociatedByAccounts(accountId);

            accountCollected.CustomerFraudSuspicious = GetCustomerFraudSuspicious(custId.ToString());

            return GetAccountDetailGeneralInformation(accountCollected, accountId, custId);
        }

        public IEnumerable<AssociatedAccountByAccount> GetAssociatedAccountByAccounts(AssociatedAccountByAccountFilter filter)
        {
            var accountCollected = new AssociatedAccountsCollected();

            accountCollected.CustomersFromAPI = GetCustomersFromAPI(filter.CustId, filter.DeviceAwayOne);

            accountCollected.AccountsByCustomers = GetAssociatedAccountByCustomers(filter.AccountId, accountCollected.CustomersFromAPI);

            accountCollected.CustomerFraudSuspicious = GetCustomerFraud(accountCollected.CustomersFromAPI);
           
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

        #region General information
        private static IEnumerable<CustomerInformation> GetGeneralCustomerInformation(int accountId)
        {
            var criteria = new GetCustomerGeneralInformationCriteria
            {
                 ip_accountId = accountId
            };

            return DataService.GetData<CustomerInformation>(criteria);
        }

        private static IEnumerable<AssociatedByAccount> GetAssociatedByAccounts(int accountId)
        {
            var criteria = new GetAssociationByAccountCriteria
            {
                accountId = accountId
            };

            return DataService.GetData<AssociatedByAccount>(criteria);
        }

        private CustomerFraudSuspicious GetCustomerFraudSuspicious(string customerId)
        {
            var criteria = new GetCustomerFraudInfoCriteria
            {
                custIds = customerId
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

        private static AccountDetail GetAccountDetailGeneralInformation(GeneralAccountCollected accountsCollected, int accountId, int custId)
        {
            var accountDetail = new AccountDetail();
            var generalInfo = new AccountGeneralInformation();

            generalInfo.AccountId = accountId;
            generalInfo.CustId = custId;

            if (accountsCollected.CustomerInformation.Any())
            {
                var customerInformation = accountsCollected.CustomerInformation.ToList()[0];

                generalInfo = GetLoginInformation(generalInfo, customerInformation);
            }

            if (accountsCollected.AssociatedByAccounts.Any())
            {
                var associated = accountsCollected.AssociatedByAccounts.ToList()[0];

                generalInfo = GetAssociatedInformation(generalInfo, associated);
            }

            generalInfo = GetFraudInformation(generalInfo, accountsCollected.CustomerFraudSuspicious);

            accountDetail.GeneralInfo = generalInfo;

            return accountDetail;
        }

        private static AccountGeneralInformation GetLoginInformation(AccountGeneralInformation generalInfo, CustomerInformation customerInformation)
        {
            generalInfo.FirstLogin = customerInformation.CreatedDate;
            generalInfo.LastLogin = customerInformation.LastLoginDate;
            generalInfo.FrequencyLogin = customerInformation.LoginCount;
            generalInfo.Username = customerInformation.Username;
            generalInfo.Username2 = customerInformation.Username2;
            generalInfo.AccountStatus = customerInformation.AccountStatus;
            generalInfo.AccountAge = customerInformation.AccountAge;

            return generalInfo;
        }

        private static AccountGeneralInformation GetAssociatedInformation(AccountGeneralInformation generalInfo, AssociatedByAccount associated)
        {
            generalInfo.AssociatedAccounts = associated.AssociatedAccount;
            generalInfo.AssociatedDevices = associated.AssociatedDevice;
            generalInfo.SharedDevices = associated.SharedDevices;
            generalInfo.NoSharedDevices = associated.NonShareDevices;
            generalInfo.AccountExceptions = associated.AccountException;

            return generalInfo;
        }

        private static AccountGeneralInformation GetFraudInformation(AccountGeneralInformation generalInfo, CustomerFraudSuspicious customerFraudSuspicious)
        {
            generalInfo.Evidence = "-";
            generalInfo.ProblemAccount = "-";
            generalInfo.AutoSuspend = ResourcesManager.GetValue(ConstsResources.NONE);

            if (customerFraudSuspicious.CustomerFrauds.Any())
            {
                var fraud = customerFraudSuspicious.CustomerFrauds.ToList()[0];
                generalInfo.Abnormal = fraud.Abnormal;
                generalInfo.DangerousOriginal = fraud.danger;
                generalInfo.DangerousNonLive = fraud.danger2;
                generalInfo.DangerousOtherSports = fraud.danger3;
                if (fraud.autosuspend)
                {
                    generalInfo.AutoSuspend = ResourcesManager.GetValue(ConstsResources.AUTO_SUSPEND_SOCCER);
                }
                else if (fraud.autosuspend2)
                {
                    generalInfo.AutoSuspend = ResourcesManager.GetValue(ConstsResources.AUTO_SUSPEND_OTHER_SPORTS);
                }
                else
                {
                    generalInfo.AutoSuspend = ResourcesManager.GetValue(ConstsResources.NONE);
                }
            }

            var fraudCase = new StringBuilder();
            foreach (var item in customerFraudSuspicious.CustomerSuspiciouses)
            {
                fraudCase.Append(item.FraudCaseName + ",");
            }
            generalInfo.CommitedFraudCase = fraudCase.ToString().TrimEnd(',');

            return generalInfo;
        }
        #endregion

        #region Associated account by account
        private static IEnumerable<CustomerByGraphDB> GetCustomersFromAPI(int custId, bool deviceAwayOne)
        {
            var configUrl = deviceAwayOne ? Consts.GraphDBGetAssociationOneLevel : Consts.GraphDBGetAssociationNLevel;

            var url = AppSettings.Get(configUrl);

            var customers = ApiHelper.Post<IEnumerable<CustomerByGraphDB>>(url, custId);

            Task.WhenAll(customers);

            return customers.Result;
        }

        private static IEnumerable<AssociatedAccountByCustomer> GetAssociatedAccountByCustomers(int accountId, IEnumerable<CustomerByGraphDB> customersFromAPI)
        {
            if (customersFromAPI.Any())
            {
                var customerIds = customersFromAPI.Select(c => c.CustId).Join();
                
                return GetAssociatedAccountByCustomers(accountId, customerIds);
            }

            return new List<AssociatedAccountByCustomer>();            
        }

        private static IEnumerable<AssociatedAccountByCustomer> GetAssociatedAccountByCustomers(int accountId, string customerIds)
        {
            var criteria = new GetAssociatedAccountByAccountCriteria
            {
                custIds = customerIds,
                rootAccountId = accountId
            };

            return DataService.GetData<AssociatedAccountByCustomer>(criteria);
        }

        private CustomerFraudSuspicious GetCustomerFraud(IEnumerable<CustomerByGraphDB> customersFromAPI)
        {
            if (customersFromAPI.Any())
            {
                var customerIds = customersFromAPI.Select(c => c.CustId).Join();

                var criteria = new GetCustomerFraudInfoCriteria
                {
                    custIds = customerIds
                };

                using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
                {
                    using (var query = db.QueryMultiple(criteria))
                    {
                        var customerFrauds = query.Read<CustomerFraud>();

                        return new CustomerFraudSuspicious
                        {
                            CustomerFrauds = customerFrauds
                        };
                    }
                }
            }

            return new CustomerFraudSuspicious();
        }

        private static IEnumerable<AssociatedAccountByAccount> GetAssociatedAccountByAccount(AssociatedAccountsCollected accountsCollected, AssociatedAccountByAccountFilter filter)
        {
            return new List<AssociatedAccountByAccount>();
            //return accountsCollected.AccountsByCustomers
            //                .Where(customer=>customer.CompareFraudStatus(filter.FraudStatus)).AsEnumerable()
            //                .Join(accountsCollected.CustomersFromAPI,
            //                    cusA => cusA.CustID,
            //                    cusB => cusB.CustId,
            //                    (cusA, cusB) => GetAssociatedAccount(cusA, cusB))
            //                .Join(accountsCollected.CustomerFraudSuspicious.CustomerFrauds
            //                    .Where(customer=>customer.CompareStatus(filter.Status)),
            //                    cusA => cusA.CustID,
            //                    cusB => cusB.custid,
            //                    (cusA, cusB) => GetAssociatedAccount(cusA, cusB));
        }

        private static AssociatedAccountByAccount GetAssociatedAccount(AssociatedAccountByCustomer customerA, CustomerByGraphDB customerB)
        {
            return new AssociatedAccountByAccount
            {
                AccountID = customerA.AccountID,
                CustID = customerA.CustID,
                Username = customerB.Username,
                Username2 = customerB.Username2,
                LastLogin = customerA.LastLogin,
                FirstLogin = customerA.FirstLogin,
                AssociatedWithRootAccountOn = customerA.AssociatedWithRootAccountOn,
                TotalLoginWithRootAccount = customerA.TotalLoginWithRootAccount,
                EvidenceLevel = customerA.EvidenceLevel
            };
        }

        private static AssociatedAccountByAccount GetAssociatedAccount(AssociatedAccountByAccount customerA, CustomerFraud customerB)
        {
            return new AssociatedAccountByAccount
            {
                AccountID = customerA.AccountID,
                CustID = customerA.CustID,
                Username = customerA.Username,
                Username2 = customerA.Username2,
                LastLogin = customerA.LastLogin,
                FirstLogin = customerA.FirstLogin,
                AssociatedWithRootAccountOn = customerA.AssociatedWithRootAccountOn,
                TotalLoginWithRootAccount = customerA.TotalLoginWithRootAccount,
                EvidenceLevel = customerA.EvidenceLevel,
                Status = customerB.GetStatusName()
            };
        }

        #endregion

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