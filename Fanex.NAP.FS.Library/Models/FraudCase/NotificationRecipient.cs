namespace Fanex.NAP.FS.Library.Models.FraudCase
{
    using Fanex.NAP.Core.Service.Impl;

    public class NotificationRecipient
    {
        private static UserService _user = new UserService();

        public int RecipientId { get; set; }

        public string Recipient => _user.Get(RecipientId)?.Username;
    }
}