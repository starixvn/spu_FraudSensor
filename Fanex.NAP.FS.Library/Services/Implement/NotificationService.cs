namespace Fanex.NAP.FS.Library.Services
{
    using Core.BaseClasses;
    using Criteria.Notification;
    using DTO;
    using System.Collections.Generic;

    public class NotificationService : INotificationService
    {
        public int GetUnreadMessageCounter(int custId)
        {
            var criteria = new UserMessageUnreadCounterGetCriteria(custId);

            var result = DataService.GetSingle<dynamic>(criteria);

            return result.UnreadMessageCounter;
        }

        public IEnumerable<Message> Get(int custId)
        {
            return DataService.GetData<Message>(new UserMessageGetCriteria(custId));
        }
    }
}