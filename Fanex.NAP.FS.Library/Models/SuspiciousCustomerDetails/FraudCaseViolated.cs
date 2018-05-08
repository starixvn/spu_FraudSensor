namespace Fanex.NAP.FS.Library.Models.SuspiciousCustomerDetails
{
    using System.Collections.Generic;
    using System.Linq;

    public class FraudCaseViolated
    {
        public string TransIDs { get; set; }

        public string FraudCaseName { get; set; }

        public IEnumerable<long> Ids => TransIDs.Split(',').Select(long.Parse);
    }
}