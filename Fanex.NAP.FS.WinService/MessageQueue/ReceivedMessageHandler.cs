namespace Fanex.NAP.FS.WinService
{
    using Library.FSMQ;
    using System;
    using System.Messaging;
    using System.Threading.Tasks;

    public class ReceivingMessageHandler : IReceivingMessageHandler
    {
        private readonly IMessageQueueReceiver _queue;
        private readonly IFraudCaseManager _fraudCaseManager;
        private Task _currentThread;

        public ReceivingMessageHandler(IFraudCaseManager fraudCaseManager = null, IMessageQueueReceiver queue = null)
        {
            _fraudCaseManager = fraudCaseManager ?? new FraudCaseManager();
            _queue = queue ?? new MessageQueueReceiver();
            _queue.AttachReceiveCompletedEventHandler(HandleReceivedMessage);
        }

        public void HandleReceivedMessage(object sender, ReceiveCompletedEventArgs e)
        {
            using (var msg = _queue.EndReceive(e.AsyncResult))
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

            _queue.BeginReceive();
        }

        public void TriggerReceivingMessage()
        {
            _queue.BeginReceive();
        }

        public void ClearCases()
        {
            _fraudCaseManager.ClearCases();
            _queue.DeattachReceiveCompletedEventHandler(HandleReceivedMessage);
        }

        private void HandleMessage(string label, int id)
        {
            var type = ConvertLabelIntoSendingType(label);

            switch (type)
            {
                case SendingType.Adding:
#if DEBUG
                    Console.WriteLine("Adding: {0}", id);
#endif

                    _fraudCaseManager.AddCase(id);
                    break;

                case SendingType.Updating:
#if DEBUG
                    Console.WriteLine("Updating: {0}", id);
#endif

                    _fraudCaseManager.UpdateCase(id);
                    break;

                case SendingType.Removing:
#if DEBUG
                    Console.WriteLine("Removing: {0}", id);
#endif

                    _fraudCaseManager.RemoveCase(id);
                    break;

                case SendingType.Deactivating:
#if DEBUG
                    Console.WriteLine("DeactivateCase: {0}", id);
#endif
                    _fraudCaseManager.RemoveCase(id);
                    break;

                case SendingType.Activating:
#if DEBUG
                    Console.WriteLine("ActivateCase: {0}", id);
#endif
                    _fraudCaseManager.AddCase(id);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static SendingType ConvertLabelIntoSendingType(string label)
        {
            SendingType type;
            var result = Enum.TryParse(label, true, out type);

            return result ? type : SendingType.None;
        }
    }
}
