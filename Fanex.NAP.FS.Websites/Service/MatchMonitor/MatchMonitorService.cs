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
        public static MatchList GetMatches(MatchGetAllCriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (var query = db.QueryMultiple(criteria))
                {
                    var fraudCases = query.Read<Models.MatchMonitor.FraudCase>();
                    var matches = query.Read<Models.MatchMonitor.Match>();

                    return new MatchList
                    {
                        Matches = matches.GroupJoin(fraudCases, m => m.MatchId, f => f.MatchId, GetMatch),
                        FraudCases = fraudCases.GroupBy(f => f.FraudCaseName).Select(g => g.Key)
                    };
                }
            }
        }

        public static IEnumerable<Models.MatchMonitor.League> GetLeagues(LeagueGetCriteria criteria)
        {
            return DataService.GetData<Models.MatchMonitor.League>(criteria);
        }

        public static object VerifyMatch(VerifyMatchCriteria criteria)
        {
            DataService.UpdateData(criteria);

            return null;
        }

        public static Models.MatchMonitor.BalanceUpDate GetBalanceUpDate()
        {
            return DataService.GetData<Models.MatchMonitor.BalanceUpDate>(new BalanceUpDateGetCriteria()).First();
        }

        private static Match GetMatch(Models.MatchMonitor.Match match, IEnumerable<Models.MatchMonitor.FraudCase> fraudCases)
        {
            var userService = new Core.Service.Impl.UserService();

            var homeName = RefData.GetTeamName(match.HomeId.ToString());
            var awayName = RefData.GetTeamName(match.AwayId.ToString());

            return new Match
            {
                Event = new Event
                {
                    HomeName =  homeName == null ? "-" : homeName[1].ToString(),
                    AwayName =   awayName == null? "-" : awayName[1].ToString(),
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
                Status = match.EventStatus,
                Score = string.Format("{0} - {1}", match.LiveHomeScore, match.LiveAwayScore)
            };
        }
    }
}