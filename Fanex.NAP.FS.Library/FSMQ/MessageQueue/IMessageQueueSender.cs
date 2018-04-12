namespace Fanex.NAP.FS.Library.FSMQ
{
    public interface IMessageQueueSender
    {
        void Clear();

        void Send(object task);

        void Send(object task, string label);
    }
}