namespace Fanex.NAP.FS.Library.Service
{
    using Fanex.BetList.Core.Builder;
    using Fanex.BetList.Core.TicketHelper;
    using Fanex.Data;
    using Fanex.NAP.FS.Library.Core.ExportExcel;
    using Fanex.NAP.FS.Library.Core.Helpers;
    using Fanex.NAP.FS.Library.Core.Helpers.Api;
    using Fanex.NAP.FS.Library.DTO;
    using Fanex.NAP.FS.Library.Criteria.MatchMonitor.FraudCaseDetails;
    using Fanex.NAP.FS.Library.ExcelConfig.MatchMonitor;
    using Fanex.NAP.FS.Library.Models.MatchMonitor.FraudCaseDetails;
    using Fanex.NAP.FS.Library.ViewModels.MatchMonitor.FraudCaseDetails;
    using RemotingRef;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core.BaseClasses;

    public class FraudCaseDetailsService : IFraudCaseDetailsService
    {
        public IEnumerable<TicketDetails> GetTicketDetails(int matchId, int fraudCaseId, DateTime eventDate, bool saveChoice = false)
        {
            var criteria = new GetTicketCriteria
            {
                EventDate = eventDate,
                FraudCaseId = fraudCaseId,
                MatchId = matchId
            };

            var legend = GetInfo(matchId, fraudCaseId, eventDate);

            var html_builder = new BetListHTMLBuilder();
            var ticketHelper = new TicketHelper();

            var null_winloss = new HashSet<string>
            {
                "running",
                "reject"
            };

            return DataService.GetData<TicketInfo>(criteria)
                .Select(a =>
                {
                    a.HomeId = legend.HomeId;
                    a.AwayId = legend.AwayId;

                    var bettype = RefData.GetBetTypeName(a.BetTypeId.ToString(), "en");

                    var grouped_bettype = GetOrder(bettype) != BettypeOrder.Other ? bettype : "Others";

                    return new TicketDetails
                    {
                        Member = a.UserName,
                        Bettype = grouped_bettype,
                        TransDate = a.TransDate.ToString("MMM dd, yyyy hh:mm:ss tt"),
                        BetID = a.TransId.ToString(),
                        IP = a.IP,
                        Odds = new OddsDetails { Value = (double)a.Odds, Type = RefData.GetOddsTypeName(a.OddsType, "en"), Html = html_builder.BuildOdds(a, null, CacheRef.GetOddsTypeName) },
                        Score = string.Format("{0}-{1}", a.LiveHomeScore, a.LiveAwayScore),
                        Stake = a.Stake,
                        Status = a.Status,
                        WinLoss = null_winloss.Contains(a.Status.ToLowerInvariant()) ? null : new decimal?(a.Winlost),
                        Choice = html_builder.BuildChoice(a, ticketHelper, null, false),
                        ChoiceExcel = saveChoice ? html_builder.BuildChoiceBlock(a, ticketHelper, null, false) : null,
                        BettypeRealName = bettype,
                        CurrencyName = a.CurrencyName,
                        RawData = saveChoice ? a : null
                    };
                })
                .OrderBy(t => GetOrder(t.Bettype))
                .ThenBy(t => t.Member)
                .ThenBy(t => t.TransDate);
        }

        public IEnumerable<CustomerDetails> GetCustomerDetails(int matchId, int fraudCaseId, DateTime eventDate)
        {
            var criteria = new GetCustomerCriteria
            {
                EventDate = eventDate,
                FraudCaseId = fraudCaseId,
                MatchId = matchId
            };

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            using (var query = db.QueryMultiple(criteria))
            {
                var custInfo = query.Read<CustInfo>();
                var custEvidence = GetEvidences(custInfo.Select(info => info.CustId));

                var totalAssociation = GetTotalAssociations(custInfo.Select(i => i.CustId)).ToDictionary(o => o.CustId, o => o.TotalAssociation);

                return custInfo.GroupJoin(
                    custEvidence,
                    info => info.CustId,
                    evid => evid.CustId,
                    (info, evidence) => new CustomerDetails
                    {
                        CustId = info.CustId,
                        Member = info.Username,
                        TotalWinLoss = (double)info.TotalWinlost,
                        TotalStake = (double)info.TotalStake,
                        Tickets = info.NoOfTickets,
                        Evidence = string.Join(", ", evidence.Select(e => e.EvidenceCode + "-" + e.EvidenceName)),
                        AssociatedAccounts = totalAssociation.ContainsKey(info.CustId) ? totalAssociation[info.CustId] : 0,
                        CurrencyName = info.CurrencyName
                    }).OrderBy(c => c.Member);
            }
        }

        public byte[] ExportExcel(int matchId, int fraudCaseId, DateTime eventDate, string templatePath)
        {
            var legend = GetInfo(matchId, fraudCaseId, eventDate);

            var tickets = GetTicketDetails(matchId, fraudCaseId, eventDate, true);
            var customers = GetCustomerDetails(matchId, fraudCaseId, eventDate);

            var wb =
                ExcelUtilities.GetWorkbook(templatePath)
                    .RenderSheet(new TicketDetailConfig(tickets.Count()), legend, tickets, null)
                    .RenderSheet(new CustomerDetailConfig(customers.Count()), legend, customers, null);

            return wb.ToBytes();
        }

        public MatchFraudCaseInfo GetInfo(int matchId, int fraudCaseId, DateTime eventDate)
        {
            var criteria = new GetMatchInfoCriteria
            {
                MatchId = matchId,
                EventDate = eventDate,
                FraudCaseId = fraudCaseId
            };

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (var query = db.QueryMultiple(criteria))
                {
                    var info = query.ReadSingle();
                    var fraudCaseName = query.ReadSingle<string>();

                    var home = RefData.GetTeamName(info.homeid.ToString())[1];
                    var away = RefData.GetTeamName(info.awayid.ToString())[1];

                    var eventName = string.Format("<span class='underdog'>{0}</span> - vs - <span class='underdog'>{1}</span>", home, away);

                    var htScore = string.Format("{0}-{1}", info.HTHomeScore, info.HTAwayScore);
                    var ftScore = string.Format("{0}-{1}", info.finalhomescore, info.finalawayscore);

                    return new MatchFraudCaseInfo
                    {
                        MatchId = matchId,
                        FraudCaseId = fraudCaseId,
                        EventDate = eventDate,
                        FraudCase = fraudCaseName,
                        Event = eventName,
                        HTScore = htScore,
                        FTScore = ftScore,
                        AwayId = info.awayid,
                        HomeId = info.homeid,
                        Home = home,
                        Away = away
                    };
                }
            }
        }

        public IEnumerable<TicketDetails> GetLast7DaysDetails(int custId)
        {
            var null_winloss = new HashSet<string>
            {
                "running",
                "reject"
            };

            var html_builder = new BetListHTMLBuilder();
            var ticketHelper = new TicketHelper();

            return GetLast7DaysInfo(custId)
                .OrderBy(a => a.TransId)
                .Select(a =>
                {
                    var bettype = RefData.GetBetTypeName(a.BetTypeId.ToString(), "en");

                    var grouped_bettype = GetOrder(bettype) != BettypeOrder.Other ? bettype : "Others";

                    return new TicketDetails
                    {
                        Member = a.UserName,
                        Bettype = grouped_bettype,
                        TransDate = a.TransDate.ToString("MMM dd, yyyy hh:mm:ss tt"),
                        BetID = a.TransId.ToString(),
                        IP = a.IP,
                        Odds = new OddsDetails { Value = (double)a.Odds, Type = RefData.GetOddsTypeName(a.OddsType, "en"), Html = html_builder.BuildOdds(a, null, CacheRef.GetOddsTypeName) },
                        Score = string.Format("{0}-{1}", a.LiveHomeScore, a.LiveAwayScore),
                        Stake = a.Stake,
                        Status = a.Status,
                        WinLoss = null_winloss.Contains(a.Status.ToLowerInvariant()) ? null : new decimal?(a.Winlost),
                        Choice = html_builder.BuildChoice(a, ticketHelper, null, false),
                        ChoiceExcel = null,
                        BettypeRealName = bettype,
                        CurrencyName = a.CurrencyName,
                        RawData = null,
                        Result = new Result
                        {
                            HTHomeScore = a.HTHomeScore,
                            HTAwayScore = a.HTAwayScore,
                            FinalHomeScore = a.FinalHomeScore,
                            FinalAwayScore = a.FinalAwayScore
                        }
                    };
                });
        }

        public IEnumerable<AssociatedCustomer> GetAssociatedAccounts(int custId)
        {
            return GetAssociations(custId)
                .Select(a => new AssociatedCustomer { Username = a.ToOWUsername })
                .OrderBy(a => a.Username);
        }

        public BettypeOrder GetOrder(string bettype)
        {
            if (bettype.Contains("1H"))
            {
                var tmp_bettype = bettype.Replace("1H", "").Trim();

                if (tmp_bettype == "Handicap")
                {
                    return BettypeOrder.FHDP;
                }

                if (tmp_bettype == "Over/Under")
                {
                    return BettypeOrder.FOU;
                }
            }

            if (bettype == "Handicap")
            {
                return BettypeOrder.HDP;
            }

            if (bettype == "Over/Under")
            {
                return BettypeOrder.OU;
            }

            return BettypeOrder.Other;
        }

        public IEnumerable<TicketInfo> GetLast7DaysInfo(int custId)
        {
            return DataService.GetData<TicketInfo>(new Last7DaysTicketGetCriteria { CustId = custId });
        }

        public IEnumerable<CustomerTotalAssociation> GetTotalAssociations(IEnumerable<int> custIds)
        {
            var url = AppSettings.Get("FPS_WEBAPI_GET_TOTAL_ASSOCIATION");

            return ApiHelper.Post<string, IEnumerable<CustomerTotalAssociation>>(url, string.Join(",", custIds)).Result;
        }

        public IEnumerable<CustomerAssociation> GetAssociations(int custId)
        {
            var url = AppSettings.Get("FPS_WEBAPI_GET_ASSOCIATION_ACCOUNT");

            return ApiHelper.Post<string, IEnumerable<CustomerAssociation>>(url, custId.ToString()).Result;
        }

        private IEnumerable<CustomerEvidence> GetEvidences(IEnumerable<int> custIds)
        {
            var url = AppSettings.Get("FPS_WEBAPI_GET_CUSTOMER_EVIDENCE");

            var param = new CustomerEvidenceList
            {
                CustIdList = string.Join(",", custIds)
            };

            return ApiHelper.Post<CustomerEvidenceList, IEnumerable<CustomerEvidence>>(url, param).Result;
        }
    }
}