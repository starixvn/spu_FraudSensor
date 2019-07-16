namespace Fanex.NAP.FS.Library.Services
{
    using Fanex.NAP.FS.Library.Models.Subscriber;
    using System.Collections.Generic;

    public interface ISubscriberAssignmentService
    {
        IEnumerable<Subscriber> GetSubscribers(string userId);

    }
}