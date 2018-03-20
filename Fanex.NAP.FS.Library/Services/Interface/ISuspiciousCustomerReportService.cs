namespace Fanex.NAP.FS.Library.Services
{
    using Criteria;
    using Models;
    using System.Collections.Generic;

    public interface ISuspiciousCustomerReportService
    {
        IEnumerable<SuspiciousCustomerReportModel> GetSuspiciousCustomerReport(SuspiciousCustomerReportGetCriteria criteria);
    }
}
