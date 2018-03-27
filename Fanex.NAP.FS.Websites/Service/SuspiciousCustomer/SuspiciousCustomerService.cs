using Fanex.NAP.Core.Service.Impl;
using Fanex.NAP.FS.Websites.Criteria.SuspiciousCustomer;
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
                    Date = d.LogDate.ToString("MMM dd, yyyy"),
                    Description = d.Description,
                    Type = d.LogType,
                    Author = user_service.Get(d.CreatorId).Username
                });
        }
    }
}