namespace Fanex.NAP.FS.Library.Models.SuspiciousCustomerDetails
{
    public class Summary: WinLossStatistics
    {
        public int BetCount { get; set; }

        public int RptMonth { get; set; }

        public int RptYear { get; set; }

        public static Summary operator +(Summary a, Summary b)
        {
            if(a == null)
            {
                return b;
            }

            if(b == null)
            {
                return a;
            }

            return new Summary
            {
                 BetCount = a.BetCount + b.BetCount,
                 RptMonth = -1,
                 RptYear = -1,
                 SumTO = a.SumTO + b.SumTO,
                 SumWL = a.SumWL + b.SumWL
            };
        }
    }
}