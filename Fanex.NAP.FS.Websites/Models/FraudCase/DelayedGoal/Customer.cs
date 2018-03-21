using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fanex.NAP.FS.Websites.Models.FraudCase.DelayedGoal
{
    public class Customer
    {
        public int CustId { get; set; }

        public string AccountName { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}