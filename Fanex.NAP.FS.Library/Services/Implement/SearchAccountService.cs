namespace Fanex.NAP.FS.Library.Services
{
    using Fanex.NAP.FS.Library.Models.Subscriber;
    using System.Collections.Generic;

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

        private List<Subscriber> GetSubscribers_Test()
        {
            var i = 1;

            var subscribers = new List<Subscriber>();
            {
                subscribers.Add(new Subscriber { SubscriberID = i++, SubscriberName = "11bet" });
                subscribers.Add(new Subscriber { SubscriberID = i++, SubscriberName = "12bet" });
                subscribers.Add(new Subscriber { SubscriberID = i++, SubscriberName = "18Luck" });
                subscribers.Add(new Subscriber { SubscriberID = i++, SubscriberName = "Alpha" });
                subscribers.Add(new Subscriber { SubscriberID = i++, SubscriberName = "ALOG" });
                subscribers.Add(new Subscriber { SubscriberID = i++, SubscriberName = "BetDeal" });
                subscribers.Add(new Subscriber { SubscriberID = i++, SubscriberName = "Boying" });
                subscribers.Add(new Subscriber { SubscriberID = i++, SubscriberName = "CN88" });
                subscribers.Add(new Subscriber { SubscriberID = i++, SubscriberName = "CGame" });
                subscribers.Add(new Subscriber { SubscriberID = i++, SubscriberName = "Dade" });
                subscribers.Add(new Subscriber { SubscriberID = i++, SubscriberName = "DF88" });
                subscribers.Add(new Subscriber { SubscriberID = i++, SubscriberName = "Hong88" });
                subscribers.Add(new Subscriber { SubscriberID = i++, SubscriberName = "K8VN" });
                subscribers.Add(new Subscriber { SubscriberID = i++, SubscriberName = "Mbet" });
                subscribers.Add(new Subscriber { SubscriberID = i, SubscriberName = "Monaco" });
            }

            return subscribers;
        }

        private List<SubscriberAccount> GetSubscriberAccounts_Test(string key, string subscribers)
        {
            var i = 1;

            var accounts = new List<SubscriberAccount>();
            {
                accounts.Add(new SubscriberAccount { SubscriberID = i++, SubscriberName = "11bet", UserName1 = "abc", UserName2 ="xyz" });
                accounts.Add(new SubscriberAccount { SubscriberID = i++, SubscriberName = "11bet", UserName1 = "abc", UserName2 = "xyz" });
                accounts.Add(new SubscriberAccount { SubscriberID = i++, SubscriberName = "11bet", UserName1 = "abc", UserName2 = "xyz" });
                accounts.Add(new SubscriberAccount { SubscriberID = i++, SubscriberName = "11bet", UserName1 = "abc", UserName2 = "xyz" });
                accounts.Add(new SubscriberAccount { SubscriberID = i++, SubscriberName = "11bet", UserName1 = "abc", UserName2 = "xyz" });
                accounts.Add(new SubscriberAccount { SubscriberID = i++, SubscriberName = "11bet", UserName1 = "abc", UserName2 = "xyz" });
                accounts.Add(new SubscriberAccount { SubscriberID = i++, SubscriberName = "11bet", UserName1 = "abc", UserName2 = "xyz" });
                accounts.Add(new SubscriberAccount { SubscriberID = i++, SubscriberName = "11bet", UserName1 = "abc", UserName2 = "xyz" });
                accounts.Add(new SubscriberAccount { SubscriberID = i++, SubscriberName = "11bet", UserName1 = "abc", UserName2 = "xyz" });
                accounts.Add(new SubscriberAccount { SubscriberID = i++, SubscriberName = "11bet", UserName1 = "abc", UserName2 = "xyz" });
                accounts.Add(new SubscriberAccount { SubscriberID = i++, SubscriberName = "12bet", UserName1 = "abc", UserName2 = "xyz" });
                accounts.Add(new SubscriberAccount { SubscriberID = i++, SubscriberName = "12bet", UserName1 = $"cn88@abc", UserName2 = "xyz" });
                accounts.Add(new SubscriberAccount { SubscriberID = i++, SubscriberName = "12bet", UserName1 = $"cn88@abc", UserName2 = "xyz" });
                accounts.Add(new SubscriberAccount { SubscriberID = i++, SubscriberName = "12bet", UserName1 = $"cn88@abc", UserName2 = "xyz" });
                accounts.Add(new SubscriberAccount { SubscriberID = i, SubscriberName = "12bet", UserName1 = $"cn88@abc", UserName2 = "xyz" });
            }

            return accounts;
        }
    }
}