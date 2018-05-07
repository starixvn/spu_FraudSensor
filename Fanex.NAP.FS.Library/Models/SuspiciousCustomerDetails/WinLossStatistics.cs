namespace Fanex.NAP.FS.Library.Models.SuspiciousCustomerDetails
{
    public class WinLossStatistics
    {
        public double Turnover { get; set; }

        public double WinLoss { get; set; }

        public double Margin => WinLoss / Turnover * 100;
    }
}