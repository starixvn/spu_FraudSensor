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
    using System.Text;
    using ViewModels.SuspiciousCustomer;

    public class SuspiciousCustomerReportService : ISuspiciousCustomerReportService
    {
        public IEnumerable<SuspiciousCustomerReportModel> GetSuspiciousCustomerReport(SuspiciousCustomerReportGetCriteria criteria)
        {
            return GetData(criteria);
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
                    Type = d.Type,
                    Author = user_service.Get(d.CreatorId).Username
                });
        }

        public byte[] ExportExcel(SuspiciousCustomerReportGetCriteria criteria, string fraudCaseName, string assignee, string template, IEnumerable<SuspiciousCustomerReportModel> data)
        {
            var wb = ExcelUtilities.GetWorkbook(template)
                                    .RenderSheet(new SuspiciousCustomerConfig(fraudCaseName, assignee), criteria, data, null);

            return wb.ToBytes();
        }

        public string ExportCSV(IEnumerable<SuspiciousCustomerReportModel> data, SuspiciousCustomerReportGetCriteria criteria, string fraudCaseName, string assignee, string classification, string verified)
        {
            var content = new StringBuilder();

            content.AppendLine("SUSPICIOUS CUSTOMER");

            content.AppendLine($"From Date: {criteria.FromDate.ToString("MM/dd/yyyy")}, ToDate: {criteria.ToDate.ToString("MM/dd/yyyy")}, Member: {criteria.Username}, Fraud Case: {fraudCaseName}, Verified: {verified}, Classification: {classification}, Assignees: {assignee}");

            content.AppendLine("#,Dirty,Delinquent,Hedge,Verified,Notes,Member,Total Violations,Violated Cases,#Tickets,Assignee");
            var counter = 0;           
            foreach (var item in data)
            {
                counter++;
                content.AppendLine($"{counter}," +
                    $"{(item.IsDirty ? "Yes" : "No")}," +
                    $"{(item.IsDelinquent ? "Yes": "No")}," +
                    $"{(item.IsHedge ? "Yes" : "No")}," +
                    $"{(item.IsVerified ? "Yes" : "No")}," +
                    $"{(item.HasNote ? "Yes" : "No")}," +
                    $"{item.Username}," +
                    $"{item.TotalViolation}," +
                    $"{item.ViolatedCases}," +
                    $"{item.BetCount}," +
                    $"{item.Assignee}");
            }

            return content.ToString();
        }

        public IEnumerable<SuspiciousCustomerReportModel> GetData(SuspiciousCustomerReportGetCriteria criteria)
        {
            return DataService.GetData<SuspiciousCustomerReportModel>(criteria);
        }
    }
}