using Fanex.NAP.Core.Service.Impl;
using Fanex.NAP.FS.Library.Core.ExportExcel;
using Fanex.NAP.FS.Library.Criteria;
using Fanex.NAP.FS.Library.Models;
using Fanex.NAP.FS.Websites.Criteria.SuspiciousCustomer;
using Fanex.NAP.FS.Websites.ExcelConfig.SuspiciousCustomer;
using Fanex.NAP.FS.Websites.Models.SuspiciousCustomer;
using Fanex.NAP.FS.Websites.ViewModels.SuspiciousCustomer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fanex.NAP.FS.Websites.Service.SuspiciousCustomer
{
    public static class SuspiciousCustomerService
    {
        private static IEnumerable<SuspiciousCustomerNote> GetNotesData(int custId)
        {
            var criteria = new SuspiciousCustomerNoteGetCriteria { CustId = custId };

            return DataService.GetData<SuspiciousCustomerNote>(criteria);
        }

        public static IEnumerable<SuspiciousCustomerNoteView> GetNoteViews(int custId)
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

        public static byte[] ExportExcel(SuspiciousCustomerReportGetCriteria criteria, string fraudCaseName, string assignee, string template)
        {
            var data = DataService.GetData<SuspiciousCustomerReportModel>(criteria);

            var wb =
                ExcelUtilities.GetWorkbook(template)
                .RenderSheet(new SuspiciousCustomerConfig(fraudCaseName, assignee), criteria, data, null);

            return wb.ToBytes();    
        }
    }
}