namespace Fanex.NAP.FS.Library.DTO
{
    using System;

    public class Message
    {
        public string FraudCaseName { get; set; }

        public string MessageContent { get; set; }

        public DateTime LogDate { get; set; }

        public bool IsViewed { get; set; }
    }
}