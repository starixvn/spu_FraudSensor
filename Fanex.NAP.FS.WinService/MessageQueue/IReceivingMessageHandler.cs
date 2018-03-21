namespace Fanex.NAP.FS.WinService
{
    using System.Messaging;

    public interface IReceivingMessageHandler
    {
        void HandleReceivedMessage(object sender, ReceiveCompletedEventArgs e);

        void TriggerReceivingMessage();

        void ClearCases();
    }
}
