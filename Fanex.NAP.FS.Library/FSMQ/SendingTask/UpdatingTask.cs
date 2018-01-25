namespace Fanex.NAP.FS.Library.FSMQ
{
    public class UpdatingTask : SendingMessage
    {
        public UpdatingTask()
        {
            Type = SendingType.UpdatingTask;
        }

        public UpdatingTask(IMessageQueueSender messageQueue)
            : base(messageQueue)
        {
            Type = SendingType.UpdatingTask;
        }
    }
}
