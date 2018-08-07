namespace Fanex.NAP.FS.Websites.Service.MatchMonitor
{
    using Fanex.Data;
    using Fanex.NAP.FS.Library.Core.ExportExcel;
    using Fanex.NAP.FS.Library.Criteria.MatchMonitor;
    using Fanex.NAP.FS.Library.ExcelConfig.MatchMonitor;
    using Fanex.NAP.FS.Library.Models;
    using Fanex.NAP.FS.Library.ViewModels.MatchMonitor;
    using Library.Core;
    using Library.Core.BaseClasses;
    using RemotingRef;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class MatchMonitorService : IMatchMonitorService
    {
        readonly RemotingRefService refService = new RemotingRefService();

        public MatchList GetMatches(MatchGetAllCriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (var query = db.QueryMultiple(criteria))
                {
                    var fraudCases = query.Read<Library.Models.MatchMonitor.FraudCase>();
                    var matches = query.Read<Library.Models.MatchMonitor.Match>();

                    return new MatchList
                    {
                        Matches =
                            matches
                            .GroupJoin(fraudCases, m => m.MatchId, f => f.MatchId, GetMatch)
                            .OrderBy(m => m.Status == "Running" ? 0 : 1)
                            .ThenByDescending(m => m.FraudCases.Sum(f => f.Value.Violations.Value))
                            .ThenBy(m => m.MatchId),
                        FraudCases = fraudCases.GroupBy(f => f.FraudCaseName).Select(g => g.Key)
                    };
                }
            }
        }

        public IEnumerable<Library.Models.MatchMonitor.League> GetLeagues(LeagueGetCriteria criteria)
        {
            return DataService.GetData<Library.Models.MatchMonitor.League>(criteria);
        }

        public object VerifyMatch(VerifyMatchCriteria criteria)
        {
            DataService.UpdateData(criteria, "MatchMonitor");

            return new object();
        }

        public Library.Models.MatchMonitor.BalanceUpDate GetBalanceUpDate()
        {
            return DataService.GetData<Library.Models.MatchMonitor.BalanceUpDate>(new BalanceUpDateGetCriteria()).First();
        }

        public byte[] ExportExcel(MatchGetAllCriteria criteria, string template)
        {
            var data = GetMatches(criteria);
            var wb = ExcelUtilities.GetWorkbook(template);
            new MatchMonitorExcelConfig().RenderExcel(wb, criteria, data.Matches, null);

            return wb.ToBytes();
        }

        public DBResult UpdateMatchNote(MatchNoteUpdateCriteria criteria)
        {
            return DataService.UpdateData(criteria, "MatchMonitor");
        }

        public Match GetMatch(Library.Models.MatchMonitor.Match match, IEnumerable<Library.Models.MatchMonitor.FraudCase> fraudCases)
        {
            var userService = new Core.Service.Impl.UserService();
            var homeName = refService.GetTeamName(match.HomeId.ToString(), "en");
            var awayName = refService.GetTeamName(match.AwayId.ToString(), "en");

            return new Match
            {
                Event = new Event
                {
                    HomeName = homeName ?? "-",
                    AwayName = awayName ?? "-",
                    LeagueName = match.LeagueName
                },
                FraudCases =
                    fraudCases
                    .GroupBy(f => Tuple.Create(f.FraudCaseId, f.FraudCaseName))
                    .Select(g => new FraudCase(g.Key.Item1, g.Key.Item2, g))
                    .ToDictionary(f => f.FraudCaseId.ToString()),
                MatchId = match.MatchId,
                Verified = match.IsVerified,
                VerifiedBy = match.VerifiedBy == null ? null : userService.Get(match.VerifiedBy.Value).Username,
                Status = (char)(match.EventStatus[0] - 32) + match.EventStatus.Remove(0, 1),
                Score = string.Format("{0} - {1}", match.LiveHomeScore, match.LiveAwayScore),
                Notes = string.IsNullOrEmpty(match.Notes) ? "-" : match.Notes
            };
        }
    }
}