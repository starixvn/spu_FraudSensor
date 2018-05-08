namespace Fanex.NAP.FS.Library.Models.SuspiciousCustomerDetails
{
    using System.Collections.Generic;

    public class Statistics
    {
        public IEnumerable<Summary> Summaries { get; set; }

        public IEnumerable<ChoiceStatistics> Choices { get; set; }

        public IEnumerable<SportStatistics> Sports { get; set; }

        public IEnumerable<BettypeStatistics> Bettypes { get; set; }
    }
}