namespace Fanex.NAP.FS.Library.Services.Implement
{
    using System.Collections.Generic;
    using Fanex.NAP.FS.Library.Models;
    using Fanex.NAP.FS.Library.Models.FraudCase;
    using Fanex.NAP.FS.Library.Services.Interface;

    public class NotificationRecipientService : INotificationRecipientService
    {
        public DBResult AddRecipient(string recipient, int userId)
        {
            throw new System.NotImplementedException();
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
            throw new System.NotImplementedException();
        }
    }
}