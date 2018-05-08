namespace Fanex.NAP.FS.Library.Services
{
    using Core.BaseClasses;
    using Core.ExportExcel;
    using Criteria.SuspiciousCustomer;
    using ExcelConfig.SuspiciousCustomer;
    using Models.SuspiciousCustomer;
    using NAP.Core.Service.Impl;
    using System.Collections.Generic;
    using System.Linq;
    using ViewModels.SuspiciousCustomer;

    public class SuspiciousCustomerReportService : ISuspiciousCustomerReportService
    {
        public IEnumerable<SuspiciousCustomerReportModel> GetSuspiciousCustomerReport(SuspiciousCustomerReportGetCriteria criteria)
        {
            return DataService.GetData<SuspiciousCustomerReportModel>(criteria);
        }

        public IEnumerable<SuspiciousCustomerNoteModel> GetNotesData(int custId)
        {
            var criteria = new SuspiciousCustomerNoteGetCriteria(custId);

            return DataService.GetData<SuspiciousCustomerNoteModel>(criteria);
        }

        public IEnumerable<SuspiciousCustomerNoteView> GetNoteViews(int custId)
        {
            var user_service = new UserService();

            return GetNotesData(custId)
                .Select(d => new SuspiciousCustomerNoteView
                {
                    Date = d.LogDate.ToString("MMM dd, yyyy hh:mm:ss tt"),
                    Description = d.Description,
                    Type = d.LogType,
                    Author = user_service.Get(d.CreatorId).Username
                });
        }

        public byte[] ExportExcel(SuspiciousCustomerReportGetCriteria criteria, string fraudCaseName, string assignee, string template)
        {
            var data = DataService.GetData<SuspiciousCustomerReportModel>(criteria);

            var wb = ExcelUtilities.GetWorkbook(template)
                .RenderSheet(new SuspiciousCustomerConfig(fraudCaseName, assignee), criteria, data, null);

            return wb.ToBytes();
        }
    }
}