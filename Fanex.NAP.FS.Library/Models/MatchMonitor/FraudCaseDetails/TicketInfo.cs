namespace Fanex.NAP.FS.Library.Models.MatchMonitor.FraudCaseDetails
{
    using Fanex.BetList.Core.Entities;

    public class TicketInfo : Ticket
    {
        public string CurrencyName { get; set; }

        public int HTHomeScore { get; set; }

        public int HTAwayScore { get; set; }

        public int FinalHomeScore { get; set; }

        public int FinalAwayScore { get; set; }

        public double FullStake { get; set; }

        public double ComPTStake { get; set; }

        public double VirtualStake { get; set; }

        public int DangerLevel { get; set; }

        public int GroupId { get; set; }

        public bool IsIPFraud { get; set; }
    }
}