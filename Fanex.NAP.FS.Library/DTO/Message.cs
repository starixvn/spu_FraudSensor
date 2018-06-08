namespace Fanex.NAP.FS.Library.DTO
{
    using System;

    public class Message
    {
        public string FraudCaseName { get; set; }

        public string MessageContent { get; set; }

        public DateTime LogDate { get; set; }

        public string LogDateString { get; set; }
        /*{
            return DateTime.Now.ToString("MMM dd, yyyy H:mm");
            
            get
            {
                return DateTime.Now.ToString("MMM dd, yyyy H:mm");
            }
        }*/

        public bool IsViewed { get; set; }
    }
}