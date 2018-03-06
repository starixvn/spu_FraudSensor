namespace Fanex.NAP.FS.Library.FSMQ
{
    public class MessageQueueSender : FSMessageQueue, IMessageQueueSender
    {
        public void Clear()
        {
            _messageQueue.Purge();
        }

        public void Send(object fraudCase)
        {
            _messageQueue.Send(fraudCase);
        }

        public void Send(object fraudCase, string label)
        {
            _messageQueue.Send(fraudCase, label);
        }
    }
}
