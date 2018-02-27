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
                        Matches = new[]
                        {
                            new Match
                            {
                                Event = new Event
                                {
                                    HomeName = "String theorist",
                                    AwayName = "Loop theorist",
                                    LeagueName = "Gravity"
                                },
                                FraudCases = new Dictionary<string, FraudCase>
                                {
                                    {  "String theory", new FraudCase{ FraudCaseId = 1, FraudCaseName = "String theory", Violations = 10} },
                                    {  "Loop theory", new FraudCase{ FraudCaseId = 1, FraudCaseName = "Loop theory", Violations = 0} },
                                    {  "Vacuum theory", new FraudCase{ FraudCaseId = 1, FraudCaseName = "Vacuum theory", Violations = null} }
                                },
                                MatchId = 5,
                                Score = "0 - 0",
                                Status = "completed",
                                Verified = false,
                                VerifiedBy = "Simon"
                            }
                        },
                        FraudCases = new[]
                        {
                            "String theory",
                            "Loop theory",
                            "Vacuum theory"
                        }
                    };


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
            if (criteria.EventDate.Date == DateTime.Today)
            {
                criteria.IsLatestDate = true;
            }

            return new[]
            {
                new Models.MatchMonitor.League
                {
                    LeagueId = 1,
                    LeagueName = "Gravity"
                },
                new Models.MatchMonitor.League
                {
                    LeagueId = 2,
                    LeagueName = "Vacuum"
                }
            };

            return DataService.GetData<Models.MatchMonitor.League>(criteria);
        }

        public static object VerifyMatch(VerifyMatchCriteria criteria)
        {
            DataService.UpdateData(criteria);

            return null;
        }

        private static Match GetMatch(Models.MatchMonitor.Match match, IEnumerable<Models.MatchMonitor.FraudCase> fraudCases)
        {
            return new Match
            {
                Event = new Event
                {
                    HomeName = RefData.GetTeamName(match.Home).ToString(),
                    AwayName = RefData.GetTeamName(match.Away).ToString(),
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