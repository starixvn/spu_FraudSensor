namespace Fanex.NAP.FS.Library.Models.Transaction
{
    using Fanex.NAP.FS.Library.Core.Consts;
    using System;    

    public class Transaction
    {
        public long TransID { get; set; }

        public DateTime TransDate { get; set; }        

        public long AccountID { get; set; }

        public long SubscriberID { get; set; }

        public string RobotTracking { get; set; }

        public string ActionResult { get; set; }

        public long DeviceID { get; set; }

        public string DeviceCode { get; set; }

        public string DeviceType { get; set; }

        public string DeviceStatus { get; set; }

        public string Browser { get; set; }

        public string Domain { get; set; }

        public string IP { get; set; }      

        public string Country { get; set; }

        public string Region { get; set; }

        public string City { get; set; }

        public string ServiceProvider { get; set; }

        public string TransDateToString
        {
            get
            {
                if (TransDate == DateTime.MinValue)
                {
                    return string.Empty;
                }

                return TransDate.ToString(Consts.FormatDateToString_MMMddyyyyhhmmsstt);
            }
        }
    }
}