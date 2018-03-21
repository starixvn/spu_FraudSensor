using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fanex.NAP.FS.Websites.Models.MatchMonitor.FraudCaseDetails
{
    public class TicketInfo
    {
        public string Username { get; set; }

        public int Bettype { get; set; }

        public DateTime TransDate { get; set; }

        public long TransId { get; set; }

        public string Betteam { get; set; }

        public int LiveHomeScore { get; set; }

        public int LiveAwayScore { get; set; }

        public double Odds { get; set; }

        public int OddsType { get; set; }

        public decimal Stake { get; set; }

        public decimal Winlost { get; set; }

        public string Status { get; set; }

        public string IP { get; set; }
    }
}