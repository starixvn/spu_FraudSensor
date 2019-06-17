namespace Fanex.NAP.FS.Library.DTO
{
    public class CustomerSubcriber
    {
        public int CustId { get; set; }

        public string SubscriberName { get; set; }
    }

    public class CustomerSubcriberList
    {
        public string CustIds { get; set; }

        public string Subscribers { get; set; }
    }
}