namespace Fanex.NAP.FS.Library.Services
{
    using Criteria;
    using Models;
    using Models.SuspiciousCustomer;
    using System.Collections.Generic;
    using ViewModels.SuspiciousCustomer;

    public interface ISuspiciousCustomerReportService
    {
        IEnumerable<SuspiciousCustomerReportModel> GetSuspiciousCustomerReport(SuspiciousCustomerReportGetCriteria criteria);

        IEnumerable<SuspiciousCustomerNote> GetNotesData(int custId);

        IEnumerable<SuspiciousCustomerNoteView> GetNoteViews(int custId);

        byte[] ExportExcel(SuspiciousCustomerReportGetCriteria criteria, string fraudCaseName, string assignee, string template);
    }
}