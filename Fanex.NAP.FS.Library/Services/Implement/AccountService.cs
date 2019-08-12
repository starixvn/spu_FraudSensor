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

        public IEnumerable<SubscriberAccount> SearchAccounts(string userName, string subscribers)
        {
            var criteria = new SearchAccountCriteria
            {
                ip_SubscriberIDs = subscribers,
                ip_Username = userName
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
        private IEnumerable<Subscriber> GetSubscribers_Test()
        {
            var i = 1;

            var data = new List<Subscriber>();
            {
                data.Add(new Subscriber { SubscriberID = i++, SubscriberName = "11bet" });
                data.Add(new Subscriber { SubscriberID = i++, SubscriberName = "12bet" });
                data.Add(new Subscriber { SubscriberID = i++, SubscriberName = "18Luck" });
                data.Add(new Subscriber { SubscriberID = i++, SubscriberName = "Alpha" });
                data.Add(new Subscriber { SubscriberID = i++, SubscriberName = "ALOG" });
                data.Add(new Subscriber { SubscriberID = i++, SubscriberName = "BetDeal" });
                data.Add(new Subscriber { SubscriberID = i++, SubscriberName = "Boying" });
                data.Add(new Subscriber { SubscriberID = i++, SubscriberName = "CN88" });
                data.Add(new Subscriber { SubscriberID = i++, SubscriberName = "CGame" });
                data.Add(new Subscriber { SubscriberID = i++, SubscriberName = "Dade" });
                data.Add(new Subscriber { SubscriberID = i++, SubscriberName = "DF88" });
                data.Add(new Subscriber { SubscriberID = i++, SubscriberName = "Hong88" });
                data.Add(new Subscriber { SubscriberID = i++, SubscriberName = "K8VN" });
                data.Add(new Subscriber { SubscriberID = i++, SubscriberName = "Mbet" });
                data.Add(new Subscriber { SubscriberID = i, SubscriberName = "Monaco" });
            }

            return data.OrderBy(s => s.SubscriberName).ToList();
        }

        private IEnumerable<SubscriberAccount> GetSubscriberAccounts_Test(string key, string subscribers)
        {
            var i = 1;

            var arrSubs = subscribers.Split(',');

            var data = new List<SubscriberAccount>();
            {
                data.Add(new SubscriberAccount { SubscriberID = i++, SubscriberName = "11bet", AccountID = i, UserName = "abc1", UserName2 = "xyz" });
                data.Add(new SubscriberAccount { SubscriberID = i++, SubscriberName = "11bet", AccountID = i, UserName = "abc12", UserName2 = "xyz" });
                data.Add(new SubscriberAccount { SubscriberID = i++, SubscriberName = "11bet", AccountID = i, UserName = "abc5", UserName2 = "xyz" });
                data.Add(new SubscriberAccount { SubscriberID = i++, SubscriberName = "11bet", AccountID = i, UserName = "abc", UserName2 = "xyz" });                
                data.Add(new SubscriberAccount { SubscriberID = i++, SubscriberName = "11bet", AccountID = i, UserName = "cn88@abc", UserName2 = "xyz" });
                data.Add(new SubscriberAccount { SubscriberID = i++, SubscriberName = "11bet", AccountID = i, UserName = "abc66", UserName2 = "xyz" });
                data.Add(new SubscriberAccount { SubscriberID = i++, SubscriberName = "11bet", AccountID = i, UserName = "abc44", UserName2 = "xyz" });
                data.Add(new SubscriberAccount { SubscriberID = i++, SubscriberName = "11bet", AccountID = i, UserName = "abcd", UserName2 = "xyz" });
                data.Add(new SubscriberAccount { SubscriberID = i++, SubscriberName = "11bet", AccountID = i, UserName = "cn@abc", UserName2 = "xyz" });
            }
            {
                data.Add(new SubscriberAccount { SubscriberID = i++, SubscriberName = "TGO", AccountID = i, UserName = "bebong@abc", UserName2 = "xyz" });
                data.Add(new SubscriberAccount { SubscriberID = i++, SubscriberName = "TGO", AccountID = i, UserName = "sieunhangao@abc", UserName2 = "xyz" });
                data.Add(new SubscriberAccount { SubscriberID = i++, SubscriberName = "TGO", AccountID = i, UserName = "iloveyou@abc", UserName2 = "xyz" });
                data.Add(new SubscriberAccount { SubscriberID = i++, SubscriberName = "TGO", AccountID = i, UserName = "abc@abc", UserName2 = "xyz" });
            }
            {
                data.Add(new SubscriberAccount { SubscriberID = i++, SubscriberName = "CN88", AccountID = i, UserName = $"cn88@abc", UserName2 = "xyz" });
                data.Add(new SubscriberAccount { SubscriberID = i++, SubscriberName = "CN88", AccountID = i, UserName = "heocon@abc", UserName2 = "xyz" });
                data.Add(new SubscriberAccount { SubscriberID = i++, SubscriberName = "CN88", AccountID = i, UserName = "abc", UserName2 = "xyz" });
                data.Add(new SubscriberAccount { SubscriberID = i++, SubscriberName = "CN88", AccountID = i, UserName = "ruangoc@abc", UserName2 = "xyz" });
                data.Add(new SubscriberAccount { SubscriberID = i, SubscriberName = "CN88", AccountID = i, UserName = "ruangoc2@abc", UserName2 = "xyz" });
            }

            if (arrSubs.Length > 1)
            {
                return data.Where(a => a.UserName.Contains(key) && arrSubs.Contains(a.SubscriberID.ToString()))
                        .OrderBy(a => a.SubscriberName)
                        .ThenBy(a => a.UserName)                       
                        .ToList();
            }

            return data.Where(a => a.UserName.Contains(key))
                        .OrderBy(a => a.SubscriberName)
                        .ThenBy(a => a.UserName)                       
                        .ToList();
        }       

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