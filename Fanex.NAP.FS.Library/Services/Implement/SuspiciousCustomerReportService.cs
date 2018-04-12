namespace Fanex.NAP.FS.Library.Services
{
    using Core;
    using Criteria;
    using Models;
    using System.Collections.Generic;

    public class SuspiciousCustomerReportService : FSBaseService, ISuspiciousCustomerReportService
    {
        public IEnumerable<SuspiciousCustomerReportModel> GetSuspiciousCustomerReport(SuspiciousCustomerReportGetCriteria criteria)
        {
            return GetData<SuspiciousCustomerReportModel>(criteria);
        }
    }
}