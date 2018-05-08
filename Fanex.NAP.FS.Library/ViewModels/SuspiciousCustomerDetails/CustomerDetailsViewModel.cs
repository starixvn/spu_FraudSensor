namespace Fanex.NAP.FS.Library.ViewModels.SuspiciousCustomerDetails
{
    using Models.SuspiciousCustomer;
    using Models.SuspiciousInfo;
    using System;
    using System.Collections.Generic;

    public class CustomerDetailsViewModel
    {
        public SuspiciousCustomerModel CustInfo { get; set; }

        public string Username { get; set; }

        public string Currency { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public string Assignee { get; set; }

        public IEnumerable<SuspiciousCustomerFraudCaseModel> ViolatedFraudCases { get; set; }
    }
}