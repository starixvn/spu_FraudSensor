namespace Fanex.NAP.FS.Library.FSMQ
{
    public class AddingCase : SendingMessage
    {
        public AddingCase()
        {
            Type = SendingType.Adding;
        }

        public AddingCase(IMessageQueueSender messageQueue)
            : base(messageQueue)
        {
            Type = SendingType.Adding;
        }
    }
}