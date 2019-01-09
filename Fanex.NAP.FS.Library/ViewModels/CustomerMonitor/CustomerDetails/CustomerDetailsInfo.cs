namespace Fanex.NAP.FS.Library.ViewModels.CustomerMonitor.CustomerDetails
{
    using System;

    public class CustomerDetailsInfo
    {
        public int CustId { get; set; }

        public string Username { get; set; }

        public int TicketCount { get; set; }

        public int ViolationCount { get; set; }

        public int MatchCount { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }
    }
}