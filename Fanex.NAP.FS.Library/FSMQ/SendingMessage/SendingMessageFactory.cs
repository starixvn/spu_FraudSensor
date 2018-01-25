namespace Fanex.NAP.FS.Library.FSMQ
{
    using System;

    public class SendingMessageFactory
    {
        public static ISendingMessage GetSender(SendingType type)
        {
            switch (type)
            {
                case SendingType.AddingTask:
                    return new AddingTask();

                case SendingType.UpdatingTask:
                    return new UpdatingTask();

                case SendingType.RemovingTask:
                    return new RemovingTask();

                case SendingType.DeactivatingCase:
                    return new DeactivatingCase();

                case SendingType.ActivatingCase:
                    return new ActivatingCase();

                default:
                    throw new ArgumentOutOfRangeException("type");
            }
        }
    }
}
