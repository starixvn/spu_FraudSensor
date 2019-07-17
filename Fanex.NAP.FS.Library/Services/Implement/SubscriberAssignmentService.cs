namespace Fanex.NAP.FS.Library.Services
{
    using Fanex.NAP.FS.Library.Models.Subscriber;
    using System;
    using System.Collections.Generic;

    public class SubscriberAssignmentService : ISubscriberAssignmentService
    {
        public IEnumerable<Subscriber> GetSubscribers(string userId)
        {
            return GetSubscribers_Test();
        }

        private List<Subscriber> GetSubscribers_Test()
        {
            var subscribers = new List<Subscriber>();
            {
                for (int i = 1; i <= 3; i++)
                {
                    subscribers.Add(new Subscriber { Id = i, Name = i + " bet"});
                }

                subscribers.Add(new Subscriber { Id = 4, Name = "Alligator" });
                subscribers.Add(new Subscriber { Id = 5, Name = "Boar" });
                subscribers.Add(new Subscriber { Id = 6, Name = "Bat" });
                subscribers.Add(new Subscriber { Id = 7, Name = "Crocodile" });
                subscribers.Add(new Subscriber { Id = 8, Name = "Dog" });
            }

            return subscribers;
        }
    }
}