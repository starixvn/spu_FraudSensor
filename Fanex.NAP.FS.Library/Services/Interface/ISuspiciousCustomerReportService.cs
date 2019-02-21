namespace Fanex.NAP.FS.Library.Services
{
    using Criteria.SuspiciousCustomer;
    using Models.SuspiciousCustomer;
    using System.Collections.Generic;
    using ViewModels.SuspiciousCustomer;

    public interface ISuspiciousCustomerReportService
    {
        IEnumerable<SuspiciousCustomerReportModel> GetSuspiciousCustomerReport(SuspiciousCustomerReportGetCriteria criteria);

        IEnumerable<SuspiciousCustomerNoteModel> GetNotesData(int custId);

        IEnumerable<SuspiciousCustomerNoteView> GetNoteViews(int custId);

        byte[] ExportExcel(SuspiciousCustomerReportGetCriteria criteria, string fraudCaseName, string assignee, string template, IEnumerable<SuspiciousCustomerReportModel> data);

        IEnumerable<SuspiciousCustomerReportModel> GetData(SuspiciousCustomerReportGetCriteria criteria);

        string ExportCSV(IEnumerable<SuspiciousCustomerReportModel> data, SuspiciousCustomerReportGetCriteria criteria, string fraudCaseName, string assignee, string classification, string verified);

    }
}