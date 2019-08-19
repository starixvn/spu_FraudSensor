namespace Fanex.NAP.FS.Websites.Service
{
    using Fanex.NAP.FS.Library.Criteria.MatchMonitor;
    using Fanex.NAP.FS.Library.Models;
    using Fanex.NAP.FS.Library.ViewModels.MatchMonitor;
    using System.Collections.Generic;

    public interface IMatchMonitorService
    {
        MatchList GetMatches(MatchFilter filter);

        IEnumerable<Library.Models.MatchMonitor.League> GetLeagues(LeagueGetCriteria criteria);

        object VerifyMatch(VerifyMatchCriteria criteria);

        Library.Models.MatchMonitor.BalanceUpDate GetBalanceUpDate();

        byte[] ExportExcel(MatchFilter filter, string template);

        DBResult UpdateMatchNote(MatchNoteUpdateCriteria criteria);

        Match GetMatch(Library.Models.MatchMonitor.Match match, IEnumerable<Library.Models.MatchMonitor.FraudCase> fraudCases);
    }
}