using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fanex.NAP.FS.Websites.Models.FraudCase.DelayedGoal
{
    public class Customer
    {
        public int Id { get; set; }

        public int CreatorId { get; set; }

        public DateTime AddedDate { get; set; }
    }
}