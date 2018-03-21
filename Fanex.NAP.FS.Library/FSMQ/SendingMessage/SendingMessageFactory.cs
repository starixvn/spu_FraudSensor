namespace Fanex.NAP.FS.Library.FSMQ
{
    using System;

    public static class SendingMessageFactory
    {
        public static ISendingMessage GetSender(SendingType type)
        {
            switch (type)
            {
                case SendingType.Adding:
                    return new AddingCase();

                case SendingType.Updating:
                    return new UpdatingCase();

                case SendingType.Removing:
                    return new RemovingCase();

                case SendingType.Deactivating:
                    return new DeactivatingCase();

                case SendingType.Activating:
                    return new ActivatingCase();

                default:
                    throw new ArgumentOutOfRangeException("type");
            }
        }
    }
}
