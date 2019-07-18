namespace Fanex.NAP.FS.Library.Services
{
    using Fanex.NAP.FS.Library.Models;
    using Fanex.NAP.FS.Library.Models.Subscriber;
    using System.Collections.Generic;

    public interface ISubscriberAssignmentService
    {
        IEnumerable<Subscriber> GetSubscribers(long userId);

        DBResult UpdateSubscribers(bool isMasterSub, int userId, string subscriberIds, int createdBy);

    }
}