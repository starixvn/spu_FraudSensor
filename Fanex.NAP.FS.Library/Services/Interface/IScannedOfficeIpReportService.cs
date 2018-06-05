namespace Fanex.NAP.FS.Library.Services.Interface
{
    using Fanex.NAP.FS.Library.Models.MatchMonitor.FraudCaseDetails;
    using Fanex.NAP.FS.Library.ViewModels.MatchMonitor.FraudCaseDetails;
    using System;
    using System.Collections.Generic;

    public interface IScannedOfficeIpReportService
    {
        IEnumerable<TicketDetails> GetTickets(DateTime fromDate, DateTime toDate);

        IEnumerable<TicketInfo> GetRawTickets(DateTime fromDate, DateTime toDate);
    }
}