namespace Fanex.NAP.FS.Library.Services.Implement
{
    using System.Collections.Generic;
    using Fanex.NAP.FS.Library.Core.BaseClasses;
    using Fanex.NAP.FS.Library.Core.Helpers;
    using Fanex.NAP.FS.Library.Criteria.Notification;
    using Fanex.NAP.FS.Library.Models;
    using Fanex.NAP.FS.Library.Models.FraudCase;
    using Fanex.NAP.FS.Library.Services.Interface;

    public class NotificationRecipientService : INotificationRecipientService
    {
        public DBResult AddRecipient(string recipients, int userId)
        {
            var criteria = new RecipientAddCriteria
            {
                RecipientIds = recipients,
                UserId = userId
            };


            return DataService.UpdateData(criteria, "Notification");
        }

        public IEnumerable<NotificationRecipient> GetRecipients()
        {
            for (int i = 0; i < 10; i++)
            {
                yield return new NotificationRecipient
                {
                    RecipientId = i,
                    Recipient = "Simon"
                };
            }
        }

        public DBResult RemoveRecipient(int recipientId, int userId)
        {
            var criteria = new RecipientRemoveCriteria
            {
                RecipientId = recipientId,
                UserId = userId
            };

            return DataService.UpdateData(criteria, "Notification");
        }
    }
}