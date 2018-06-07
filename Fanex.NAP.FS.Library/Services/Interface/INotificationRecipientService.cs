using Fanex.NAP.FS.Library.Models;
using Fanex.NAP.FS.Library.Models.FraudCase;
using System.Collections.Generic;

namespace Fanex.NAP.FS.Library.Services.Interface
{
    public interface INotificationRecipientService
    {
        IEnumerable<NotificationRecipient> GetRecipients();

        DBResult AddRecipient(string recipients, int userId);

        DBResult RemoveRecipient(int recipientId, int userId);
    }
}