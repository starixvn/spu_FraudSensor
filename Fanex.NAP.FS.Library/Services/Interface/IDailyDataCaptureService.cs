namespace Fanex.NAP.FS.Library.Services.Interface
{
    using Fanex.NAP.FS.Library.Criteria.DailyDataCapture;
    using Fanex.NAP.FS.Library.ViewModels.DailyDataMonitor;
    using System.Collections.Generic;

    public interface IDailyDataCaptureService
    {
        IEnumerable<CapturedMatch> GetMatches(DailyDataGetCriteria criteria);
    }
}