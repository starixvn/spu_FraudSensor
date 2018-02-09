using Fanex.Data;
using Fanex.NAP.FS.Websites.Criteria.MatchMonitor;
using Fanex.NAP.FS.Websites.ViewModels.MatchMonitor;
using RemotingRef;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fanex.NAP.FS.Websites.Service.MatchMonitor
{
    public static class MatchMonitorService
    {
        public static IEnumerable<Match> GetMatches(MatchGetAllCriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (var query = db.QueryMultiple(criteria))
                {
                    var fraudCases = query.Read<Models.MatchMonitor.FraudCase>();
                    var matches = query.Read<Models.MatchMonitor.Match>();

                    return matches.GroupJoin(fraudCases, m => m.MatchId, f => f.MatchId, GetMatch);
                }
            }
        }

        private static Match GetMatch(Models.MatchMonitor.Match match, IEnumerable<Models.MatchMonitor.FraudCase> fraudCases)
        {
            return new Match
            {
                Event = new Event
                {
                    MatchName = string.Format("{0} vs {1}", RefData.GetTeamName(match.Home), RefData.GetTeamName(match.Away)),
                    LeagueName = match.LeagueName
                },
                FraudCases =
                    fraudCases
                    .GroupBy(f => Tuple.Create(f.FraudCaseId, f.FraudCaseName))
                    .Select(g => new FraudCase(g.Key.Item1, g.Key.Item2, g))
                    .ToDictionary(f => f.FraudCaseName),
                MatchId = match.MatchId,
                Verified = match.IsVerified,
                VerifiedBy = match.VerifiedBy,
                Status = match.EventStatus,
                Score = string.Format("{0} - {1}", match.LiveHomeScore, match.LiveAwayScore)
            };
        }
    }
}