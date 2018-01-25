namespace Fanex.NAP.FS.WinService.Core
{
    using System.Messaging;

    public interface IReceivingMessageHandler
    {
        void HandleReceivedMessage(object sender, ReceiveCompletedEventArgs e);

        void TriggerReceivingMessage();

        void ClearTasks();
    }
}
