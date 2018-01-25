namespace Fanex.NAP.FS.WinService
{
    using Fanex.NAP.FS.Library.FSMQ;
    using System;
    using System.Messaging;

    public class ReceivingMessageHandler : IReceivingMessageHandler
    {

        public void HandleReceivedMessage(object sender, ReceiveCompletedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void TriggerReceivingMessage()
        {
            throw new NotImplementedException();
        }

        public void ClearTasks()
        {
            throw new NotImplementedException();
        }
    }
}
