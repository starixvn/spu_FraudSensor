#pragma warning disable S1244 // Floating point numbers should not be tested for equality

namespace Fanex.NAP.FS.Library.Models.SuspiciousCustomerDetails
{
    public class WinLossStatistics
    {
        public double SumTO { get; set; }

        public double SumWL { get; set; }

        public double Margin => SumTO == 0 ? 0 : SumWL / SumTO * 100;
    }
}

#pragma warning restore S1244 // Floating point numbers should not be tested for equality