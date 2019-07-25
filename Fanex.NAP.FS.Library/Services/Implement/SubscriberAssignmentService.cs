namespace Fanex.NAP.FS.Library.Services
{
    using Fanex.NAP.FS.Library.Core.BaseClasses;
    using Fanex.NAP.FS.Library.Criteria.SubscriberAssignment;
    using Fanex.NAP.FS.Library.Models.Subscriber;
    using System.Linq;
    using System.Collections.Generic;
    using System;
    using Fanex.NAP.FS.Library.Models;

    public class SubscriberAssignmentService : ISubscriberAssignmentService
    {
        public IEnumerable<SubscriberAssignment> GetSubscribers(long userId)
        {
            var criteria = new SubscriberGetCriteria
            {
                ip_userId = userId
            };

            return DataService.GetData<SubscriberAssignment>(criteria).OrderBy(c => c.SubscriberName).ThenBy(c=>c.SubscriberID);
        }

        public DBResult UpdateSubscribers(bool isMasterSub, int userId, string subscriberIds, int createdBy)
        {
            var criteria = new SubscribersUpdateCriteria
            {
                ip_isMasterSub = isMasterSub,
                ip_subscriberIds = subscriberIds,
                ip_userId = userId,
                ip_createdBy = createdBy
            };

            try
            {
                return DataService.UpdateData(criteria, "SubscriberAssignment");
            }
            catch (Exception ex)
            {
                return new DBResult
                {
                    ErrorCode = 1,
                    ErrorMsg = ex.Message
                };
            }
        }
    }
}