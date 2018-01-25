namespace Fanex.NAP.FS.Library.FSMQ
{
    using Core.Utilities;
    using Fanex.Logging;
    using System;
    using System.Messaging;

    public class FSMessageQueue
    {
        protected readonly MessageQueue _messageQueue;

        protected FSMessageQueue()
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
    }
}
