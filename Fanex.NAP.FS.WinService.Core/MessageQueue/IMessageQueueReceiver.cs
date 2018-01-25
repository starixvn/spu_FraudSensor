namespace Fanex.NAP.FS.WinService.Core
{
    using System;
    using System.Messaging;

    public interface IMessageQueueReceiver
    {
        void AttachReceiveCompletedEventHandler(ReceiveCompletedEventHandler receiveCompleted);

        void DeattachReceiveCompletedEventHandler(ReceiveCompletedEventHandler receiveCompleted);

        void Stop();

        void Clear();

        void BeginReceive();

        Message EndReceive(IAsyncResult asyncResult);
    }
}
