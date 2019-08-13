namespace Fanex.NAP.FS.Library.Models.Transaction
{    
    using System;    

    public class Transaction
    {
        public long TransID { get; set; }

        public long AccountID { get; set; }

        public long SubscriberID { get; set; }

        public string Tracking { get; set; }

        public bool IsRobot { get; set; }

        public string ActionName { get; set; }

        public string ActionResult { get; set; }

        public string City { get; set; }

        public string Country { get; set; }

        public string CountryCode { get; set; }

        public long DeviceID { get; set; }

        public string DeviceCode { get; set; }

        public string Browser { get; set; }

        public string Domain { get; set; }

        public DeviceFlagged Flagged { get; set; }

        public string IP { get; set; }

        public string Isp { get; set; }

        public string Region { get; set; }

        public DateTime TransDate { get; set; }

        public string ServiceProvider { get; set; }
    }    
}