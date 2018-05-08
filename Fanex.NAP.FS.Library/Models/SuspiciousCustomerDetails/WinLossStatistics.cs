namespace Fanex.NAP.FS.Library.Models.SuspiciousCustomerDetails
{
    public class WinLossStatistics
    {
        public double SumTO { get; set; }

        public double SumWL { get; set; }

        public double Margin => SumWL / SumTO * 100;
    }
}