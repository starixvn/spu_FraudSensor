namespace Fanex.NAP.FS.Websites.Models.SuspiciousCustomer
{
    using System;

    public class SuspiciousCustomerNote
    {
        public DateTime LogDate { get; set; }

        public string Description { get; set; }

        public int CreatorId { get; set; }

        public string LogType { get; set; }
    }
}