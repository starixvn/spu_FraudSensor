namespace Fanex.NAP.FS.Websites.Service.MatchMonitor
{
    using Fanex.Data;
    using Fanex.NAP.FS.Library.Core.ExportExcel;
    using Fanex.NAP.FS.Library.Criteria.MatchMonitor;
    using Fanex.NAP.FS.Library.ExcelConfig.MatchMonitor;
    using Fanex.NAP.FS.Library.Models;
    using Fanex.NAP.FS.Library.ViewModels.MatchMonitor;
    using MatchModel = Fanex.NAP.FS.Library.Models.MatchMonitor;
    using Library.Core.BaseClasses;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.NAP.FS.Library.Core.Helpers.Api;
    using Fanex.NAP.FS.Library.Core.Helpers;
    using Fanex.NAP.FS.Library.Core.Utilities;
    using DTO = Fanex.NAP.FS.Library.DTO;

    public class MatchMonitorService : IMatchMonitorService
    {
        public MatchList GetMatches(MatchFilter filter)
        {
            if (filter.Sites != null && filter.Sites != "All")
            {
                return GetMatchBySite(filter);
            }

            return GetMatches(filter, null);
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

        public byte[] ExportExcel(MatchFilter filter, string template)
        {
            var data = GetMatches(filter);
            var wb = ExcelUtilities.GetWorkbook(template);
            new MatchMonitorExcelConfig().RenderExcel(wb, filter, data.Matches, null);

            return wb.ToBytes();
        }

        public DBResult UpdateMatchNote(MatchNoteUpdateCriteria criteria)
        {
            return DataService.UpdateData(criteria, "MatchMonitor");
        }

        public Match GetMatch(Library.Models.MatchMonitor.Match match, IEnumerable<Library.Models.MatchMonitor.FraudCase> fraudCases)
        {
            var userService = new Core.Service.Impl.UserService();
            var homeName = CachedRef.GetTeamName(match.HomeId);
            var awayName = CachedRef.GetTeamName(match.AwayId);

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
                MatchCode = match.MatchCode,
                Verified = match.IsVerified,
                VerifiedBy = match.VerifiedBy == null ? null : userService.Get(match.VerifiedBy.Value).Username,
                Status = (char)(match.EventStatus[0] - 32) + match.EventStatus.Remove(0, 1),
                Score = string.Format("{0} - {1}", match.LiveHomeScore, match.LiveAwayScore),
                Notes = string.IsNullOrEmpty(match.Notes) ? "-" : match.Notes
            };
        }

        #region Private Methods
        private MatchList GetMatchBySite(MatchFilter filter)
        {
            var rawMatchesCustomers = GetSuspiciousCustIDsByDate(filter.EventDate);

            var customers = rawMatchesCustomers.Select(c => c.CustId);

            var customersBySites = GetCustomersBySites(customers, filter.Sites);

            var matchesCustomersBySites = JoinMatchesCustomers(rawMatchesCustomers, customersBySites);

            return GetMatches(filter, matchesCustomersBySites);
        }

        private IEnumerable<MatchModel.MatchCustomerBySite> GetSuspiciousCustIDsByDate(DateTime date)
        {
            var criteria = new GetSuspiciousCustIDsByDateCriteria
            {
                EventDate = date
            };

            return DataService.GetData<MatchModel.MatchCustomerBySite>(criteria);
        }

        private IEnumerable<DTO.CustomerSubcriber> GetCustomersBySites(IEnumerable<int> customers, string sites)
        {
            var urlCustSites = AppSettings.Get("FPS_WEBAPI_CUSTOMER_SUBSCRIBER");

            var customersBySites = customers.Distinct().Chunk(500)
                                    .Select(q => ApiHelper.Post<IEnumerable<DTO.CustomerSubcriber>>(urlCustSites, 
                                                new DTO.CustomerSubcriberList { CustIds = string.Join(",", q), Subscribers = sites != "All" ? sites : null }))
                                    .ToArray();

            Task.WhenAll(customersBySites);

            return customersBySites.SelectMany(c=>c.Result);
        }

        private IEnumerable<MatchModel.MatchCustomerBySite> JoinMatchesCustomers(IEnumerable<MatchModel.MatchCustomerBySite> matchesCustomers, IEnumerable<DTO.CustomerSubcriber> customersBySites)
        {
            return matchesCustomers.Join(customersBySites,
                m => m.CustId,
                s => s.CustId,
                (matches, sites) => new MatchModel.MatchCustomerBySite
                {
                    CustId = matches.CustId,
                    MatchId = matches.MatchId,
                    Site = sites.SubscriberName
                });
        }

        private MatchList GetMatches(MatchFilter filter, IEnumerable<MatchModel.MatchCustomerBySite> matchCustomerBySites)
        {
            var criteria = new MatchGetAllCriteria
            {
                EventDate = filter.EventDate,
                IsVerified = filter.IsVerified,
                MatchStatus = filter.MatchStatus,
                LeagueIds = filter.LeagueIds,
                IsMainMarket = filter.IsMainMarket,
                IsShowSpecialCurrencies = filter.IsShowSpecialCurrencies,
            };

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (var query = db.QueryMultiple(criteria))
                {
                    var fraudCases = query.Read<MatchModel.FraudCase>();
                    var matches = query.Read<MatchModel.Match>();

                    var matchesBySites = matchCustomerBySites != null ? FilterMatchBySites(matches, matchCustomerBySites) : matches;

                    foreach (var item in fraudCases)
                    {
                        item.SuspiciousTicketsCount = item.SuspiciousTicketsCount ?? 0;
                    }

                    return new MatchList
                    {
                        Matches = matchesBySites
                            .GroupJoin(fraudCases, m => m.MatchId, f => f.MatchId, GetMatch)
                            .OrderBy(m => m.Status == "Running" ? 0 : 1)
                            .ThenByDescending(m => m.FraudCases.Sum(f => f.Value.Violations.Value))
                            .ThenBy(m => m.MatchId),
                        FraudCases = fraudCases.GroupBy(f => f.FraudCaseName).Select(g => g.Key)
                    };
                }
            }
        }

        private IEnumerable<MatchModel.Match> FilterMatchBySites(IEnumerable<MatchModel.Match> matches, IEnumerable<MatchModel.MatchCustomerBySite> matchCustomerBySites)
        {
            var matchesCustomeres = matchCustomerBySites.Select(m => m.MatchId).Distinct();

            return matches.Join(matchesCustomeres,
                            m => m.MatchId,
                            s => s,
                            (match, site) => match);
        }
        #endregion
    }
}