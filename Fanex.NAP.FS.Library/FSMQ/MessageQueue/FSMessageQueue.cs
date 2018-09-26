namespace Fanex.NAP.FS.Library.FSMQ
{
    using Core.Utilities;
    using Fanex.Logging;
    using System;
    using System.Messaging;

    public class FSMessageQueue : IDisposable
    {
        protected readonly MessageQueue _messageQueue;

        public FSMessageQueue()
        {
            var queuePath = ConfigSettings.GetConfigStringByName("queuePath");

            try
            {
                _messageQueue = new MessageQueue(queuePath) { Formatter = new XmlMessageFormatter(new[] { typeof(int) }) };
            }
            catch (InvalidOperationException ex)
            {
                Logger.Log.Error("_messageQueue = MessageQueue.Exists(queuePath) .. : " + ex);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            _messageQueue.Dispose();
        }
    }
}