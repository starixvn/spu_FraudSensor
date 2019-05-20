namespace Fanex.NAP.FS.Library.Services.Interface
{
    using Fanex.NAP.FS.Library.Criteria.CheckBetReport;
    using Fanex.NAP.FS.Library.Models;
    using Fanex.NAP.FS.Library.ViewModels.CheckBetReport;
    using System.Collections.Generic;

    public interface ICheckBetReportService
    {
        IEnumerable<CheckBetTicket> Generate(CheckBetReportGetCriteria criteria);

        DBResult UpdateNote(CheckBetReportUpdateNoteCriteria criteria);
    }
}