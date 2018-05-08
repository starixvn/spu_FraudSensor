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
        string GetAssginee(int custId);

        IEnumerable<SuspiciousCustomerFraudCaseModel> GetSuspiciousCustomerFraudCase(int custId);

        DBResult VerifySuspiciousCustomerFraudCase(int custId, int userId, int fraudCaseId);

        DBResult InsertCustomerLogNotes(int custId, int userId, string logDesc);

        IEnumerable<SuspiciousCustomerNoteModel> GetCustomerLogNotes(int custId);

        DBResult DeleteCustomerLogNotes(int userId, int customerLogId);

        DBResult UpdateSuspiciousCustomerInfo(int custId, int userId, bool isDelinquent, bool isDirty, bool isHedge);

        #endregion

        #region Statistics

        Statistics GetStatistics(int custId, DateTime from, DateTime to);

        IEnumerable<object> GetViolatedTickets(int custId, DateTime from, DateTime to);

        #endregion
    }
}