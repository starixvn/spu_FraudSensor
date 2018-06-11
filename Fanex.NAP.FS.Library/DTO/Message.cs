namespace Fanex.NAP.FS.Library.DTO
{
    using System;

    public class Message
    {
        public string FraudCaseName { get; set; }

        public string MessageContent { get; set; }

        public DateTime LogDate { get; set; }

        public string LogDateString
        {
            get
            {
                return LogDate == DateTime.MinValue ? "" : LogDate.ToString("MMM dd, yyyy H:mm");
            }
        }

        public bool IsViewed { get; set; }
    }
}