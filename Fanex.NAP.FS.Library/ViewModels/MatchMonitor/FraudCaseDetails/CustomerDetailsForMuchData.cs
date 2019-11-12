namespace Fanex.NAP.FS.Library.ViewModels.MatchMonitor.FraudCaseDetails
{
    using System.Collections.Generic;

    public class CustomerDetailsForMuchData : CustomerDetails
    {
        public int TotalCustomers { get; set; }

        public IEnumerable<int> CustIdList { get; set; }
    }
}