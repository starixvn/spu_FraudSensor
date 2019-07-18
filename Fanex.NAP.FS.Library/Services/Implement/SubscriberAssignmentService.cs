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
            var i = 1;

            var subscribers = new List<Subscriber>();
            {
                subscribers.Add(new Subscriber { Id = i++, Name = "1 bet" });
                subscribers.Add(new Subscriber { Id = i++, Name = "2 bet" });
                subscribers.Add(new Subscriber { Id = i++, Name = "3 bet" });
                subscribers.Add(new Subscriber { Id = i++, Name = "4 bet" });
                subscribers.Add(new Subscriber { Id = i++, Name = "Alligator" });
                subscribers.Add(new Subscriber { Id = i++, Name = "Boar" });
                subscribers.Add(new Subscriber { Id = i++, Name = "Bat" });
                subscribers.Add(new Subscriber { Id = i++, Name = "Crocodile" });
                subscribers.Add(new Subscriber { Id = i++, Name = "Cat" });
                subscribers.Add(new Subscriber { Id = i++, Name = "Dog 1" });
                subscribers.Add(new Subscriber { Id = i++, Name = "Dog 2" });
                subscribers.Add(new Subscriber { Id = i++, Name = "Dog 3" });
                subscribers.Add(new Subscriber { Id = i++, Name = "Elephant" });
                subscribers.Add(new Subscriber { Id = i++, Name = "Fish" });
                subscribers.Add(new Subscriber { Id = i, Name = "Koala bear" });
            }

            return subscribers;
        }
    }
}