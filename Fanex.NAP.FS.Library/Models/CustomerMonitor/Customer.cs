namespace Fanex.NAP.FS.Library.Models.CustomerMonitor
{
    public class Customer
    {
        public int CustId { get; set; }

        public string Username { get; set; }

        public int TotalTicket { get; set; }

        public long TicketCount { get; set; }

        public int ViolationCount { get; set; }

        public int MatchCount { get; set; }

        public string Remark { get; set; }

        public string Note { get; set; }
    }
}