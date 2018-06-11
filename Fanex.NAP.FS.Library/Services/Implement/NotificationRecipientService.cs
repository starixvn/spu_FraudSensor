namespace Fanex.NAP.FS.Library.Services.Implement
{
    using Fanex.NAP.FS.Library.Core.BaseClasses;
    using Fanex.NAP.FS.Library.Criteria.Notification;
    using Fanex.NAP.FS.Library.Models;
    using Fanex.NAP.FS.Library.Models.FraudCase;
    using Fanex.NAP.FS.Library.Services.Interface;
    using System.Collections.Generic;

    public class NotificationRecipientService : INotificationRecipientService
    {
        public DBResult AddRecipient(string recipients, int userId, FraudCaseId fraudCase)
        {
            var criteria = new RecipientAddCriteria
            {
                RecipientIds = recipients,
                UserId = userId,
                FraudCaseId = (int)fraudCase
            };

            return DataService.UpdateData(criteria, "Notification");
        }

        public IEnumerable<NotificationRecipient> GetRecipients(FraudCaseId fraudCase)
        {
            return DataService.GetData<NotificationRecipient>(new RecipientGetCriteria { FraudCaseId = (int)fraudCase });
        }

        public DBResult RemoveRecipient(int recipientId, int userId, FraudCaseId fraudCase)
        {
            var criteria = new RecipientRemoveCriteria
            {
                RecipientId = recipientId,
                UserId = userId,
                FraudCaseId = (int)fraudCase
            };

            return DataService.UpdateData(criteria, "Notification");
        }
    }
}