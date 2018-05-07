namespace Fanex.NAP.FS.Library.Services
{
    using Fanex.NAP.FS.Library.Models.SuspiciousCustomerDetails;
    using System;
    using System.Collections.Generic;

    public interface ISuspiciousCustomerReportDetailsService
    {
        #region statistics

        IEnumerable<Summary> GetSummaries(int custid, DateTime from, DateTime to);

        IEnumerable<ChoiceStatistics> GetChoices(int custid, DateTime from, DateTime to);

        IEnumerable<SportStatistics> GetTop5Sports(int custid, DateTime from, DateTime to);

        IEnumerable<BettypeStatistics> GetTop5Bettypes(int custid, DateTime from, DateTime to);

        #endregion statistics
    }
}