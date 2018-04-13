namespace Fanex.NAP.FS.Library.ViewModels.MatchMonitor.FraudCaseDetails
{
    using Fanex.BetList.Core.Templates;
    using Fanex.NAP.FS.Library.Models.MatchMonitor.FraudCaseDetails;

    public class TicketDetails
    {
        public string Member { get; set; }

        public string TransDate { get; set; }

        public string BetID { get; set; }

        public string Choice { get; set; }

        public string Score { get; set; }

        public OddsDetails Odds { get; set; }

        public decimal Stake { get; set; }

        public decimal? WinLoss { get; set; }

        public string Status { get; set; }

        public string IP { get; set; }

        public string Bettype { get; set; }

        public Choice_Template ChoiceExcel { get; set; }

        public string BettypeRealName { get; set; }

        public string CurrencyName { get; set; }

        public TicketInfo RawData { get; set; }

        public Result Result { get; set; }
    }
}