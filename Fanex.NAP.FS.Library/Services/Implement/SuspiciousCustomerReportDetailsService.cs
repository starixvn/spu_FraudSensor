namespace Fanex.NAP.FS.Library.Services
{
    using BetList.Core.Builder;
    using Fanex.BetList.Core.TicketHelper;
    using Fanex.Data;
    using Fanex.NAP.FS.Library.Criteria.SuspiciousCustomerDetails;
    using Fanex.NAP.FS.Library.Models.MatchMonitor.FraudCaseDetails;
    using Fanex.NAP.FS.Library.Models.SuspiciousCustomerDetails;
    using Fanex.NAP.FS.Library.ViewModels.SuspiciousCustomerDetails;
    using RemotingRef;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class SuspiciousCustomerReportDetailsService : ISuspiciousCustomerReportDetailsService
    {
        #region statistics

        public Statistics GetStatistics(int custId, DateTime from, DateTime to)
        {
            var criteria = new SuspiciousCustomerDetailsStatisticsGetCriteria
            {
                CustId = custId,
                FromDate = from,
                ToDate = to
            };

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            using (var queries = db.QueryMultiple(criteria))
            {
                return new Statistics
                {
                    Summaries = queries.Read<Summary>(),
                    Choices = queries.Read<ChoiceStatistics>(),
                    Sports = queries.Read<SportStatistics>(),
                    Bettypes = queries.Read<BettypeStatistics>()
                };
            }
        }

        public IEnumerable<object> GetViolatedTickets(int custId, DateTime from, DateTime to)
        {
            var criteria = new SuspiciousCustomerDetailsViolatedTicketGetCriteria
            {
                CustId = custId,
                FromDate = from,
                ToDate = to
            };

            var html_builder = new BetListHTMLBuilder();
            var ticketHelper = new TicketHelper();

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            using (var queries = db.QueryMultiple(criteria))
            {
                var fraudCases =
                    queries.Read<FraudCaseViolated>()
                    .GroupBy(f => f.FraudCaseName)
                    .Select(g => new FraudCaseViolated { FraudCaseName = g.Key, TransIDs = string.Join(",", g.Select(f => f.TransIDs)) })
                    .OrderBy(f => f.FraudCaseName);

                var tickets =
                    queries.Read<TicketInfo>().
                    ToDictionary(t => t.TransId);

                foreach (var fc in fraudCases)
                {
                    yield return new { key = fc.FraudCaseName };

                    foreach (var t in fc.Ids.Select(id => tickets[id]).OrderBy(t => t.TransDate))
                    {
                        var obj = new TicketDetails
                        {
                            BetID = t.TransId.ToString(),
                            Bettype = RefData.GetBetTypeName(t.BetTypeId.ToString(), "en"),
                            CurrencyName = t.CurrencyName,
                            Choice = html_builder.BuildChoice(t, ticketHelper, null, false),
                            IP = t.IP,
                            Odds = html_builder.BuildOdds(t, null, CacheRef.GetOddsTypeName),
                            Result = new ViewModels.MatchMonitor.FraudCaseDetails.Result
                            {
                                FinalAwayScore = t.FinalAwayScore,
                                FinalHomeScore = t.FinalHomeScore,
                                HTHomeScore = t.HTHomeScore,
                                HTAwayScore = t.HTAwayScore
                            },
                            TransDate = t.TransDate.ToString("MMM dd, yyyy<br/>hh:mm:ss tt"),
                            Stake = t.Stake,
                            Status = t.Status,
                            WinLoss = t.Winlost
                        };

                        yield return new { key = fc.FraudCaseName, value = obj };
                    }
                }
            }
        }

        #endregion statistics
    }
}