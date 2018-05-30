namespace Fanex.NAP.FS.Library.Models.SuspiciousCustomer
{
    using System;

    public class SuspiciousCustomerNoteModel
    {
        public DateTime LogDate { get; set; }

        public string Description { get; set; }

        public int CreatorId { get; set; }

        public string Type { get; set; }

        public string Author { get; set; }

        public int CustomerLogId { get; set; }

        public string LogDateString => LogDate.ToString("MMM dd, yyyy hh:mm:ss tt");
    }
}