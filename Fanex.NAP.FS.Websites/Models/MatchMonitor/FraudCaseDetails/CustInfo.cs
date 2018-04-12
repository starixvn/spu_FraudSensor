namespace Fanex.NAP.FS.Websites.Models.MatchMonitor.FraudCaseDetails
{
    public class CustInfo
    {
        public int CustId { get; set; }

        public string Username { get; set; }

        public int NoOfTickets { get; set; }

        public decimal TotalStake { get; set; }

        public decimal TotalWinlost { get; set; }

        public string CurrencyName { get; set; }
    }
}