namespace Fanex.NAP.FS.Library.FSMQ
{
    public interface ISendingMessage
    {
        SendingType Type { get; set; }

        bool Send(object message);
    }
}
