namespace Fanex.NAP.FS.Library.FSMQ
{
    public class RemovingTask : SendingMessage
    {
        public RemovingTask()
        {
            Type = SendingType.RemovingTask;
        }

        public RemovingTask(IMessageQueueSender messageQueue)
            : base(messageQueue)
        {
            Type = SendingType.RemovingTask;
        }
    }
}
