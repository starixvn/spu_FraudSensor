using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fanex.NAP.FS.Websites.Models.SuspiciousCustomer
{
    public class SuspiciousCustomerNote
    {
        public DateTime LogDate { get; set; }

        public string Description { get; set; }

        public int CreatorId { get; set; }

        public string LogType { get; set; }
    }
}