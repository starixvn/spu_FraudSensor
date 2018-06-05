namespace Fanex.NAP.FS.Library.Services
{
    using DTO;
    using System.Collections.Generic;

    public interface INotificationService
    {
        int GetUnreadMessageCounter(int custId);

        IEnumerable<Message> Get(int custId);
    }
}