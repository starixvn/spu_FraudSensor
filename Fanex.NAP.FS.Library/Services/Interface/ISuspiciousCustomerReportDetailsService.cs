namespace Fanex.NAP.FS.Library.Services
{
    using Fanex.NAP.FS.Library.Models.SuspiciousCustomerDetails;
    using Models;
    using Models.SuspiciousCustomer;
    using Models.SuspiciousInfo;
    using System;
    using System.Collections.Generic;

    public interface ISuspiciousCustomerReportDetailsService
    {
        #region Customer Info
        string GetAssignee(int custId);

        DBResult UpdateAssignee(int custId, int? assignee, int userId);

        SuspiciousCustomerModel GetSuspiciousCustomerInfo(int custId);

        IEnumerable<SuspiciousCustomerFraudCaseModel> GetSuspiciousCustomerFraudCase(int custId);

        DBResult VerifySuspiciousCustomerFraudCase(int custId, int userId, int fraudCaseId);

        DBResult InsertCustomerLogNotes(int custId, int userId, string logDesc);

        IEnumerable<SuspiciousCustomerNoteModel> GetCustomerLogNotes(int custId);

        DBResult DeleteCustomerLogNotes(int userId, int customerLogId);

        DBResult UpdateSuspiciousCustomerInfo(int custId, int userId, bool? isDelinquent, bool? isDirty, bool? isHedge);

        string GetCustomerCurrency(int custId);

        #endregion

        #region Statistics

        Statistics GetStatistics(int custId, DateTime from, DateTime to, bool isShowSpecialCurrencie);

        IEnumerable<object> GetViolatedTickets(int custId, DateTime from, DateTime to, bool isShowSpecialCurrencie);

        #endregion
    }
}