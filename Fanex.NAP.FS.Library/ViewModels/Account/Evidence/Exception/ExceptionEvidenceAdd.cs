namespace Fanex.NAP.FS.Library.ViewModels.Account.Evidence.Exception
{
    public class ExceptionEvidenceAdd
    {
        public int AccountId { get; set; }

        public int ExceptionAccountId { get; set; }

        public int SubscriberId { get; set; }

        public string Comment { get; set; }

        public int UserId { get; set; }
    }
}