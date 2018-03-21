namespace Fanex.NAP.FS.Library.FSMQ
{
    public class ActivatingCase : SendingMessage
    {
        public ActivatingCase()
        {
            Type = SendingType.Activating;
        }

        public ActivatingCase(IMessageQueueSender messageQueue)
            : base(messageQueue)
        {
            Type = SendingType.Activating;
        }
    }
}
