namespace Fanex.NAP.FS.Library.FSMQ
{
    public class MessageQueueSender : FSMessageQueue, IMessageQueueSender
    {
        public void Clear()
        {
            _messageQueue.Purge();
        }

        public void Send(object task)
        {
            _messageQueue.Send(task);
        }

        public void Send(object task, string label)
        {
            _messageQueue.Send(task, label);
        }
    }
}
