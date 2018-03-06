namespace Fanex.NAP.FS.WinService
{
    using Fanex.NAP.FS.Library.FSMQ;

    public class FraudCaseLoader : IFraudCaseLoader
    {
        private readonly ISendingMessage _sendingCase;

        public FraudCaseLoader()
        {
            _sendingCase = new AddingCase(new MessageQueueSender());
        }

        public FraudCaseLoader(IMessageQueueSender messageQueue)
        {
            _sendingCase = new AddingCase(messageQueue);
        }

        public void LoadFraudCases()
        {
            ////??
        }
    }
}