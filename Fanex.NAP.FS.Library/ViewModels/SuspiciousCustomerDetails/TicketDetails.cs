namespace Fanex.NAP.FS.Library.ViewModels.SuspiciousCustomerDetails
{
    using Fanex.NAP.FS.Library.ViewModels.MatchMonitor.FraudCaseDetails;

    public class TicketDetails
    {
        public string TransDate { get; set; }

        public string BetID { get; set; }

        public string Choice { get; set; }

        public string Odds { get; set; }

        public decimal Stake { get; set; }

        public decimal? WinLoss { get; set; }

        public string Status { get; set; }

        public string IP { get; set; }

        public string Bettype { get; set; }
        
        public string CurrencyName { get; set; }

        public Result Result { get; set; }
    }
}