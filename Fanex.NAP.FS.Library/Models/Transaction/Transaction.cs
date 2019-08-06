namespace Fanex.NAP.FS.Library.Models.Transaction
{
    using Fanex.NAP.FS.Library.Models.Account;
    using Fanex.NAP.FS.Library.Models.Device;
    using System;    

    public class Transaction
    {
        public Account Account { get; set; }

        public string RobotTracking { get; set; }

        public string ActionName { get; set; }

        public string ActionResult { get; set; }

        public string City { get; set; }

        public string Country { get; set; }

        public string CountryCode { get; set; }

        public Device Device { get; set; }

        public string Browser { get; set; }

        public string Domain { get; set; }

        public DeviceFlagged Flagged { get; set; }

        public string IPAddress { get; set; }

        public string Isp { get; set; }

        public string Region { get; set; }

        public DateTime TransDate { get; set; }

        public string ServiceProvider { get; set; }
    }
}