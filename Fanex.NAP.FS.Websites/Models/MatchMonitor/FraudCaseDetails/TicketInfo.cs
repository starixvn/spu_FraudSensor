namespace Fanex.NAP.FS.Websites.Models.MatchMonitor.FraudCaseDetails
{
    using Fanex.BetList.Core.Entities;

    public class TicketInfo : Ticket
    {
        public string CurrencyName { get; set; }

        public int HTHomeScore { get; set; }

        public int HTAwayScore { get; set; }

        public int FinalHomeScore { get; set; }

        public int FinalAwayScore { get; set; }
    }
}