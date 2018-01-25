namespace Fanex.NAP.FS.WinService.Core
{
    using Fanex.NAP.FS.Library.FSMQ;
    using System;
    using System.Messaging;

    public class MessageQueueReceiver : FSMessageQueue, IMessageQueueReceiver
    {
        public void AttachReceiveCompletedEventHandler(ReceiveCompletedEventHandler receiveCompleted)
        {
            _messageQueue.ReceiveCompleted += receiveCompleted;
        }

        public void DeattachReceiveCompletedEventHandler(ReceiveCompletedEventHandler receiveCompleted)
        {
            _messageQueue.ReceiveCompleted -= receiveCompleted;
        }

        public void Stop()
        {
            _messageQueue.Purge();
            _messageQueue.Close();
        }

        public void Clear()
        {
            _messageQueue.Purge();
        }

        public void BeginReceive()
        {
            _messageQueue.BeginReceive();
        }

        public Message EndReceive(IAsyncResult asyncResult)
        {
            return _messageQueue.EndReceive(asyncResult);
        }
    }
}
