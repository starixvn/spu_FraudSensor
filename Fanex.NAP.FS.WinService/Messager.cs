namespace Fanex.NAP.FS.WinService
{
    using Fanex.NAP.FS.Library.Models;
    using System;
    using System.Messaging;
    using System.Threading.Tasks;

    public class Messager : IMessager
    {
        private IMessageQueueReceiver q;

        private Action<MessageType, FraudCaseId> handler;

        private Task _currentThread;

        public Messager(IMessageQueueReceiver q)
        {
            this.q = q;
            this.q.AttachReceiveCompletedEventHandler(HandleReceivedMsg);
        }

        public void ListenToMessage(Action<MessageType, FraudCaseId> handler)
        {
            this.handler = handler;
        }

        private void HandleReceivedMsg(object sender, ReceiveCompletedEventArgs e)
        {
            using (var msg = q.EndReceive(e.AsyncResult))
            {
                var message = msg.Body;
                var id = (int)message;
                var label = msg.Label;

                if (string.IsNullOrEmpty(label))
                {
                    return;
                }

                _currentThread = _currentThread == null
                    ? Task.Factory.StartNew(() => HandleMessage(label, id))
                    : _currentThread.ContinueWith((antecedent) => HandleMessage(label, id));
            }

            q.BeginReceive();
        }

        private void HandleMessage(string label, int id)
        {
            var type = ConvertLabelIntoSendingType(label);

            handler(type, (FraudCaseId)id);
        }

        private static MessageType ConvertLabelIntoSendingType(string label)
        {
            MessageType type;
            var result = Enum.TryParse(label, true, out type);

            return result ? type : MessageType.None;
        }
    }
}