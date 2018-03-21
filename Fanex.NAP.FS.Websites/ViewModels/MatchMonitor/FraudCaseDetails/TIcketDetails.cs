using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fanex.NAP.FS.Websites.ViewModels.MatchMonitor.FraudCaseDetails
{
    public class TIcketDetails
    {
        public string Member { get; set; }

        public string TransDate { get; set; }

        public string BetID { get; set; }

        public string Choice { get; set; }

        public string Score { get; set; }

        public string Odds { get; set; }

        public double Stake { get; set; }

        public double WinLoss { get; set; }

        public string Status { get; set; }

        public string IP { get; set; }

        public string Bettype { get; set; }
    }
}