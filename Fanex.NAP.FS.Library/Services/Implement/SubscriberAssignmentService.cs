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
                for (int i = 1; i <= 10; i++)
                {
                    subscribers.Add(new Subscriber { Id = i, Name = "subcriber name " + i });
                }
            }

            return subscribers;
        }
    }
}