namespace Fanex.NAP.FS.Library.Services
{
    using Fanex.NAP.FS.Library.Models.Subscriber;
    using Fanex.NAP.FS.Library.ViewModels.SubscriberAccount;
    using System.Linq;
    using System.Collections.Generic;
    using System;

    public class SearchAccountService : ISearchAccountService
    {
        public IEnumerable<Subscriber> GetSubscribers()
        {
            return GetSubscribers_Test();
        }

        public IEnumerable<SubscriberAccount> SearchSubscriberAccounts(string key, string subscribers)
        {
            return GetSubscriberAccounts_Test(key, subscribers);
        }

        public SubscriberAccountDetail GetSubscriberAccountDetail(int accountID)
        {
            var data = new SubscriberAccountDetail();
            {
                data.AccountInfo = new SubscriberAccountInfo();
                {
                    data.AccountInfo.AccountID = accountID;
                    data.AccountInfo.FirstLogin = DateTime.Now;
                    data.AccountInfo.LastLogin = DateTime.Now;
                    data.AccountInfo.FrequencyLogin = 1;
                }
            }

            return data;
        }

        private List<Subscriber> GetSubscribers_Test()
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

        private List<SubscriberAccount> GetSubscriberAccounts_Test(string key, string subscribers)
        {
            var i = 1;

            var arrSubs = subscribers.Split(',');

            var data = new List<SubscriberAccount>();
            {
                data.Add(new SubscriberAccount { SubscriberID = i++, SubscriberName = "11bet", AccountID = i, UserName1 = "abc1", UserName2 = "xyz" });
                data.Add(new SubscriberAccount { SubscriberID = i++, SubscriberName = "11bet", AccountID = i, UserName1 = "abc12", UserName2 = "xyz" });
                data.Add(new SubscriberAccount { SubscriberID = i++, SubscriberName = "11bet", AccountID = i, UserName1 = "abc5", UserName2 = "xyz" });
                data.Add(new SubscriberAccount { SubscriberID = i++, SubscriberName = "11bet", AccountID = i, UserName1 = "abc", UserName2 = "xyz" });                
                data.Add(new SubscriberAccount { SubscriberID = i++, SubscriberName = "11bet", AccountID = i, UserName1 = "cn88@abc", UserName2 = "xyz" });
                data.Add(new SubscriberAccount { SubscriberID = i++, SubscriberName = "11bet", AccountID = i, UserName1 = "abc66", UserName2 = "xyz" });
                data.Add(new SubscriberAccount { SubscriberID = i++, SubscriberName = "11bet", AccountID = i, UserName1 = "abc44", UserName2 = "xyz" });
                data.Add(new SubscriberAccount { SubscriberID = i++, SubscriberName = "11bet", AccountID = i, UserName1 = "abc", UserName2 = "xyz" });
                data.Add(new SubscriberAccount { SubscriberID = i++, SubscriberName = "11bet", AccountID = i, UserName1 = "cn@abc", UserName2 = "xyz" });
            }
            {
                data.Add(new SubscriberAccount { SubscriberID = i++, SubscriberName = "TGO", AccountID = i, UserName1 = "bebong@abc", UserName2 = "xyz" });
                data.Add(new SubscriberAccount { SubscriberID = i++, SubscriberName = "TGO", AccountID = i, UserName1 = "sieunhangao@abc", UserName2 = "xyz" });
                data.Add(new SubscriberAccount { SubscriberID = i++, SubscriberName = "TGO", AccountID = i, UserName1 = "iloveyou@abc", UserName2 = "xyz" });
                data.Add(new SubscriberAccount { SubscriberID = i++, SubscriberName = "TGO", AccountID = i, UserName1 = "abc@abc", UserName2 = "xyz" });
            }
            {
                data.Add(new SubscriberAccount { SubscriberID = i++, SubscriberName = "CN88", AccountID = i, UserName1 = $"cn88@abc", UserName2 = "xyz" });
                data.Add(new SubscriberAccount { SubscriberID = i++, SubscriberName = "CN88", AccountID = i, UserName1 = "heocon@abc", UserName2 = "xyz" });
                data.Add(new SubscriberAccount { SubscriberID = i++, SubscriberName = "CN88", AccountID = i, UserName1 = "abc", UserName2 = "xyz" });
                data.Add(new SubscriberAccount { SubscriberID = i++, SubscriberName = "CN88", AccountID = i, UserName1 = "ruangoc@abc", UserName2 = "xyz" });
                data.Add(new SubscriberAccount { SubscriberID = i, SubscriberName = "CN88", AccountID = i, UserName1 = "ruangoc2@abc", UserName2 = "xyz" });
            }

            if (arrSubs.Length > 1)
            {
                return data.Where(a => a.UserName1.Contains(key) && arrSubs.Contains(a.SubscriberID.ToString()))
                        .OrderBy(a => a.SubscriberName)
                        .ThenBy(a => a.UserName1)                       
                        .ToList();
            }

            return data.Where(a => a.UserName1.Contains(key))
                        .OrderBy(a => a.SubscriberName)
                        .ThenBy(a => a.UserName1)                       
                        .ToList();
        }
    }
}