namespace Fanex.NAP.FS.WinService
{
    using Fanex.NAP.FS.Library.Models;
    using System;

    public interface IMessager
    {
        void ListenToMessage(Action<MessageType, FraudCaseId> handler);
    }
}