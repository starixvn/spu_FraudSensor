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
    }
}