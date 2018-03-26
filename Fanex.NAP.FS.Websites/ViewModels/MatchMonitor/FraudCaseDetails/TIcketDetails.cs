using Fanex.BetList.Core.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fanex.NAP.FS.Websites.ViewModels.MatchMonitor.FraudCaseDetails
{
    public class TicketDetails
    {
        public string Member { get; set; }

        public DateTime TransDate { get; set; }

        public string BetID { get; set; }

        public string Choice { get; set; }

        public string Score { get; set; }

        public OddsDetails Odds { get; set; }

        public decimal Stake { get; set; }

        public decimal WinLoss { get; set; }

        public string Status { get; set; }

        public string IP { get; set; }

        public string Bettype { get; set; }

        public Choice_Template ChoiceExcel { get; set; }
        public string BettypeRealName { get; internal set; }
    }
}