namespace Fanex.NAP.FS.Library.Services
{
    using Fanex.NAP.FS.Library.Models.SuspiciousCustomerDetails;
    using System;
    using System.Collections.Generic;

    public interface ISuspiciousCustomerReportDetailsService
    {
        #region statistics

        Statistics GetStatistics(int custId, DateTime from, DateTime to);

        IEnumerable<object> GetViolatedTickets(int custId, DateTime from, DateTime to);

        #endregion statistics
    }
}