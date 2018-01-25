namespace Fanex.NAP.FS.Library.FSMQ
{
    public class AddingTask : SendingMessage
    {
        public AddingTask()
        {
            Type = SendingType.AddingTask;
        }

        public AddingTask(IMessageQueueSender messageQueue)
            : base(messageQueue)
        {
            Type = SendingType.AddingTask;
        }
    }
}
