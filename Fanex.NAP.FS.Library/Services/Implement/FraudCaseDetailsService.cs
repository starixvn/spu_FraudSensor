namespace Fanex.NAP.FS.Library.Service
{
    using Core.BaseClasses;
    using Fanex.BetList.Core.Builder;
    using Fanex.BetList.Core.TicketHelper;
    using Fanex.Data;    
    using Fanex.NAP.FS.Library.Core.ExportExcel;
    using Fanex.NAP.FS.Library.Core.Helpers;
    using Fanex.NAP.FS.Library.Core.Helpers.Api;
    using Fanex.NAP.FS.Library.Core.Utilities;
    using Fanex.NAP.FS.Library.Criteria.MatchMonitor;
    using Fanex.NAP.FS.Library.Criteria.MatchMonitor.FraudCaseDetails;
    using Fanex.NAP.FS.Library.DTO;
    using Fanex.NAP.FS.Library.ExcelConfig.MatchMonitor;
    using Fanex.NAP.FS.Library.Models;
    using Fanex.NAP.FS.Library.Models.MatchMonitor;
    using Fanex.NAP.FS.Library.Models.MatchMonitor.FraudCaseDetails;
    using Fanex.NAP.FS.Library.ViewModels.MatchMonitor.FraudCaseDetails;
    using Fanex.Data.Repository;
    using System;
    using System.Collections.Generic;
    using System.Linq;    
    using System.Threading.Tasks;
    using Fanex.Logging;

    public class FraudCaseDetailsService : IFraudCaseDetailsService
    {
        public int environment { get; set; }

        public int mailingListId { get; set; }

        public FraudCaseDetailsService(int env, int mailId)
        {
            environment = env;
            mailingListId = mailId;
        }

        #region Other Fraud Cases
        public IEnumerable<TicketDetails> GetTicketDetails(int matchId, int fraudCaseId, DateTime eventDate, bool isShowSpecialCurrencies, bool saveChoice = false)
        {
            var criteria = new GetTicketCriteria
            {
                EventDate = eventDate,
                FraudCaseId = fraudCaseId,
                MatchId = matchId,
                IsShowSpecialCurrencies = isShowSpecialCurrencies
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

                    var bettype = CachedRef.GetBettypeName(a.BetTypeId, "en");
                    var grouped_bettype = GetOrder(bettype) != BettypeOrder.Other ? bettype : "Others";

                    return new TicketDetails
                    {
                        Member = a.UserName,
                        Bettype = grouped_bettype,
                        TransDate = a.TransDate.ToString("MMM dd, yyyy hh:mm:ss tt"),
                        BetID = a.TransId.ToString(),
                        IP = a.IP,
                        Odds = new OddsDetails { Value = (double)a.Odds, Type = CachedRef.GetOddsTypeName(int.Parse(a.OddsType)), Html = html_builder.BuildOdds(a, null, ticketHelper.GetOddsTypeNameById) },
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
                .ThenBy(t => DateTime.Parse(t.TransDate))
                .ThenBy(t => t.Member);
        }

        public IEnumerable<CustomerDetails> GetCustomerDetails(int matchId, int fraudCaseId, DateTime eventDate, bool isShowSpecialCurrencies)
        {
            var criteria = new GetCustomerCriteria
            {
                EventDate = eventDate,
                FraudCaseId = fraudCaseId,
                MatchId = matchId,
                IsShowSpecialCurrencies = isShowSpecialCurrencies
            };

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            using (var query = db.QueryMultiple(criteria))
            {
                var custInfo = query.Read<CustInfo>();

                IEnumerable<CustomerEvidence> custEvidence;

                if (fraudCaseId == (int)FraudCaseId.Arbitrage)
                {
                    custEvidence = new CustomerEvidence[0];
                }
                else
                {
                    custEvidence = GetEvidences(custInfo.Select(info => info.CustId));
                }

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
                        Evidence = string.Join(", ", evidence.Distinct().Select(e => e.EvidenceCode + "-" + e.EvidenceName)),
                        AssociatedAccounts = totalAssociation.ContainsKey(info.CustId) ? totalAssociation[info.CustId] : 0,
                        CurrencyName = info.CurrencyName
                    }).OrderBy(c => c.Member);
            }
        }

        private IEnumerable<CustomerEvidence> GetEvidences(IEnumerable<int> custIds)
        {
            var url = AppSettings.Get("FPS_WEBAPI_GET_CUSTOMER_EVIDENCE");

            var tsk = custIds.Distinct().Chunk(500).Select(a => new CustomerEvidenceList
            {
                CustIdList = string.Join(",", a)
            }).Select(p => ApiHelper.Post<IEnumerable<CustomerEvidence>>(url, p)).ToArray();

            Task.WaitAll(tsk);

            return tsk.Where(t => t.Result != null).SelectMany(t => t.Result);
        }


        public byte[] ExportExcel(int matchId, int fraudCaseId, DateTime eventDate, string templatePath, bool isShowSpecialCurrencies)
        {
            var legend = GetInfo(matchId, fraudCaseId, eventDate);
            var tickets = GetTicketDetails(matchId, fraudCaseId, eventDate, isShowSpecialCurrencies, true);
            var customers = GetCustomerDetails(matchId, fraudCaseId, eventDate, isShowSpecialCurrencies);

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
                    var info = query.ReadSingle<MatchInfo>();
                    var fraudCaseName = query.IsEmptyOrClosed ? "-" : query.ReadSingle<string>();
                    var home = CachedRef.GetTeamName(info.HomeId);
                    var away = CachedRef.GetTeamName(info.AwayId);
                    var eventName = string.Format("<span class='underdog'>{0}</span> - vs - <span class='underdog'>{1}</span>", home, away);
                    var htScore = string.Format("{0}-{1}", info.HTHomeScore, info.HTAwayScore);
                    var ftScore = string.Format("{0}-{1}", info.FinalHomeScore, info.FinalAwayScore);

                    return new MatchFraudCaseInfo
                    {
                        MatchId = matchId,
                        FraudCaseId = fraudCaseId,
                        EventDate = eventDate,
                        FraudCase = fraudCaseName,
                        Event = eventName,
                        HTScore = htScore,
                        FTScore = ftScore,
                        AwayId = info.AwayId,
                        HomeId = info.HomeId,
                        Home = home,
                        Away = away
                    };
                }
            }
        }

        public IEnumerable<TicketDetails> GetLast7DaysDetails(int custId, bool isShowSpecialCurrencies)
        {
            var null_winloss = new HashSet<string>
            {
                "running",
                "reject"
            };

            var html_builder = new BetListHTMLBuilder();
            var ticketHelper = new TicketHelper();

            return GetLast7DaysInfo(custId, isShowSpecialCurrencies)
                .OrderBy(a => a.TransId)
                .Select(a =>
                {
                    var bettype = CachedRef.GetBettypeName(a.BetTypeId);
                    var grouped_bettype = GetOrder(bettype) != BettypeOrder.Other ? bettype : "Others";

                    return new TicketDetails
                    {
                        Member = a.UserName,
                        Bettype = grouped_bettype,
                        TransDate = a.TransDate.ToString("MMM dd, yyyy hh:mm:ss tt"),
                        BetID = a.TransId.ToString(),
                        IP = a.IP,
                        Odds = new OddsDetails { Value = (double)a.Odds, Type = CachedRef.GetOddsTypeName(int.Parse(a.OddsType)), Html = html_builder.BuildOdds(a, null, ticketHelper.GetOddsTypeNameById) },
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
                .Select(a => new AssociatedCustomer { Username = a.ToOWUsername, Currency = a.CurrencyName })
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

        public IEnumerable<TicketInfo> GetLast7DaysInfo(int custId, bool isShowSpecialCurrencies)
        {
            return DataService.GetData<TicketInfo>(new Last7DaysTicketGetCriteria { CustId = custId, IsShowSpecialCurrencies = isShowSpecialCurrencies });
        }

        public IEnumerable<CustomerTotalAssociation> GetTotalAssociations(IEnumerable<int> custIds)
        {
            var url = AppSettings.Get("FPS_WEBAPI_GET_TOTAL_ASSOCIATION");

            var tsk = ApiHelper.Post<string, IEnumerable<CustomerTotalAssociation>>(url, string.Join(",", custIds), environment, mailingListId);

            Task.WaitAll(tsk);

            return tsk.Result ?? Enumerable.Empty<CustomerTotalAssociation>();
        }

        public IEnumerable<CustomerAssociation> GetAssociations(int custId)
        {
            var url = AppSettings.Get("FPS_WEBAPI_GET_ASSOCIATION_ACCOUNT");

            return ApiHelper.Post<string, IEnumerable<CustomerAssociation>>(url, custId.ToString(), environment, mailingListId).Result;
        }

        public IEnumerable<TicketDetails> GetTicketFromIP(string ip, bool isShowSpecialCurrencies)
        {
            var criteria = new GetTicketByIPCriteria { IP = ip, IsShowSpecialCurrencies = isShowSpecialCurrencies };

            var html_builder = new BetListHTMLBuilder();
            var ticketHelper = new TicketHelper();

            var null_winloss = new HashSet<string>
            {
                "running",
                "reject"
            };

            return DataService.GetData<TicketInfo>(criteria)
                .OrderBy(t => t.UserName)
                .ThenBy(t => t.TransDate)
                .Select(a =>
                {
                    var bettype = CachedRef.GetBettypeName(a.BetTypeId);
                    var grouped_bettype = GetOrder(bettype) != BettypeOrder.Other ? bettype : "Others";

                    return new TicketDetails
                    {
                        Member = a.UserName,
                        Bettype = grouped_bettype,
                        TransDate = a.TransDate.ToString("MMM dd, yyyy hh:mm:ss tt"),
                        BetID = a.TransId.ToString(),
                        IP = a.IP,
                        Odds = new OddsDetails { Value = (double)a.Odds, Type = CachedRef.GetOddsTypeName(int.Parse(a.OddsType)), Html = html_builder.BuildOdds(a, null, ticketHelper.GetOddsTypeNameById) },
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

        public byte[] ExportToExcelIpTickets(string ip, string template, bool isShowSpecialCurrencies)
        {
            var data =
                DataService.GetData<TicketInfo>(new GetTicketByIPCriteria { IP = ip, IsShowSpecialCurrencies = isShowSpecialCurrencies })
                .OrderBy(t => t.UserName)
                .ThenBy(t => t.TransDate);

            var buDate = DataService.GetSingle<BalanceUpDate>(new BalanceUpDateGetCriteria());
            var config = new TicketByIpConfig(ip, buDate.BalanceUpDay);
            var wb = ExcelUtilities.GetWorkbook(template);

            config.RenderExcel(wb, null, data, null);

            return wb.ToBytes();
        }
        #endregion Other Fraud Cases

        #region Flagged Accounts
        public IEnumerable<FlaggedAccountTicket> GetFlaggedAccountTickets(FlaggedAccountCriteria criteria)
        {
            var otherEvidenceCodes = AppSettings.Get("FPS_GET_OTHER_EVIDENCES");
            var isBettypeOther = false;

            if (criteria.Bettype == 0)
            {
                isBettypeOther = true;
                criteria.Bettype = -1;
            }

            var rawTickets = DataService.GetData<TicketInfo>(criteria);
            rawTickets = isBettypeOther ? rawTickets.Where(t => !(new List<int> { 1, 3, 7, 8 }).Contains(t.BetTypeId)) : rawTickets;

            string siteList = criteria.Sites != "All" ? criteria.Sites : null;
            string evidenceList = CheckCriteriaEvidenceOthers(criteria.EvidenceCodeList, otherEvidenceCodes);

            var custOfSites = FilterCustomersBySites(rawTickets, siteList);

            if (!custOfSites.Any())
            {
                return new List<FlaggedAccountTicket>();
            }

            var custOfEvidences = FilterCustomersByEvidences(custOfSites, evidenceList);

            if (criteria.EvidenceCodeList != null && criteria.EvidenceCodeList.Contains(otherEvidenceCodes))
            {
                return FilterFlaggedAccountTicketsOtherEvidence(rawTickets, custOfSites, custOfEvidences, criteria);
            }

            return FilterFlaggedAccountTickets(rawTickets, custOfSites, custOfEvidences, criteria);
        }

        private static IEnumerable<CustomerSubcriber> FilterCustomersBySites(IEnumerable<TicketInfo> rawTickets, string sites)
        {
            var urlCustSites = AppSettings.Get("FPS_WEBAPI_CUSTOMER_SUBSCRIBER");

            Logger.Log.Info("Call [FPS_WEBAPI_CUSTOMER_SUBSCRIBER] - Start");

            var customersBySites = rawTickets.Select(t => t.CustId).Distinct().Chunk(500)
                                    .Select(q => ApiHelper.Post<IEnumerable<CustomerSubcriber>>(urlCustSites, new CustomerSubcriberList { CustIds = string.Join(",", q), Subscribers = sites }))
                                    .ToArray();

            Task.WaitAll(customersBySites);

            Logger.Log.Info("Call [FPS_WEBAPI_CUSTOMER_SUBSCRIBER] - End");

            return customersBySites.SelectMany(t => t.Result);
        }

        private static IEnumerable<CustomerEvidence> FilterCustomersByEvidences(IEnumerable<CustomerSubcriber> customerBySites, string evidenceList)
        {
            var urlCustEvidences = AppSettings.Get("FPS_WEBAPI_GET_CUSTOMER_EVIDENCE");

            Logger.Log.Info("Call [FPS_WEBAPI_GET_CUSTOMER_EVIDENCE] - Start");

            var customerByEvidences = customerBySites.Select(c => c.CustId).Distinct().Chunk(500)
                                       .Select(q => ApiHelper.Post<IEnumerable<CustomerEvidence>>(urlCustEvidences, new CustomerEvidenceList { CustIdList = string.Join(",", q), EvidenceCodeList = evidenceList }))
                                       .ToArray();
            Task.WaitAll(customerByEvidences);

            Logger.Log.Info("Call [FPS_WEBAPI_GET_CUSTOMER_EVIDENCE] - End");

            return customerByEvidences.SelectMany(t => t.Result);
        }

        private IEnumerable<FlaggedAccountTicket> FilterFlaggedAccountTicketsOtherEvidence(IEnumerable<TicketInfo> rawData, IEnumerable<CustomerSubcriber> custSubcribers, IEnumerable<CustomerEvidence> evidences, FlaggedAccountCriteria criteria)
        {
            var legend = GetInfo(criteria.MatchId, 6, criteria.EventDate);
            var html_builder = new BetListHTMLBuilder();
            var ticketHelper = new TicketHelper();
            var null_winloss = new HashSet<string>
            {
                "running",
                "reject"
            };

            // fiter by Sites
            var custIds = custSubcribers.Select(c => c.CustId);
            var subcriberDict = custSubcribers.ToLookup(c => c.CustId, c => c.SubscriberName);
            Func<long, bool> conditionSubcriber = s => custIds.Contains((int)s);

            // filter by Evidences
            var evidenceDict = evidences.ToLookup(c => c.CustId, c => c.EvidenceCode + "-" + c.EvidenceName);
            var evidenceExtract = evidences.Where(c => !criteria.EvidenceCodeList.Contains(c.EvidenceCode));
            var lstEvidenceExtract = evidenceExtract.Select(c => (long)c.CustId).ToList();

            Func<long, bool> conditionExtract = f => !lstEvidenceExtract.Contains(f);

            var data = rawData.AsParallel()
                .Where(c => conditionExtract(c.CustId) && conditionSubcriber(c.CustId))
                .Select(a =>
                {
                    a.HomeId = legend.HomeId;
                    a.AwayId = legend.AwayId;

                    var bettype = CachedRef.GetBettypeName(a.BetTypeId);
                    var grouped_bettype = GetOrder(bettype) != BettypeOrder.Other ? bettype : "Others";

                    return new FlaggedAccountTicket
                    {
                        Member = a.UserName,
                        Evidences = evidenceDict[(int)a.CustId],
                        Site = subcriberDict[(int)a.CustId].FirstOrDefault(),
                        DangerousLevel = a.DangerLevel,
                        Bettype = grouped_bettype,
                        TransDate = a.TransDate.ToString("MMM dd, yyyy hh:mm:ss tt"),
                        BetID = a.TransId.ToString(),
                        IP = a.IP,
                        Odds = new OddsDetails { Value = (double)a.Odds, Type = CachedRef.GetOddsTypeName(int.Parse(a.OddsType)), Html = html_builder.BuildOdds(a, null, ticketHelper.GetOddsTypeNameById) },
                        Score = string.Format("{0}-{1}", a.LiveHomeScore, a.LiveAwayScore),
                        Stake = a.Stake,
                        Status = a.Status,
                        WinLoss = null_winloss.Contains(a.Status.ToLowerInvariant()) ? null : new decimal?(a.Winlost),
                        Choice = html_builder.BuildChoice(a, ticketHelper, null, false),
                        ChoiceExcel = criteria.SaveChoice ? html_builder.BuildChoiceBlock(a, ticketHelper, null, false) : null,
                        BettypeRealName = bettype,
                        CurrencyName = a.CurrencyName,
                        RawData = criteria.SaveChoice ? a : null
                    };
                })
                .OrderBy(t => GetOrder(t.Bettype))
                .ThenBy(t => DateTime.Parse(t.TransDate))
                .ThenBy(t => t.Member);

            return data;
        }

        private IEnumerable<FlaggedAccountTicket> FilterFlaggedAccountTickets(IEnumerable<TicketInfo> rawData, IEnumerable<CustomerSubcriber> custSubcribers, IEnumerable<CustomerEvidence> evidences, FlaggedAccountCriteria criteria)
        {
            var legend = GetInfo(criteria.MatchId, 6, criteria.EventDate);
            var html_builder = new BetListHTMLBuilder();
            var ticketHelper = new TicketHelper();
            var null_winloss = new HashSet<string>
            {
                "running",
                "reject"
            };

            // fiter by Sites
            var custIds = custSubcribers.Select(c => c.CustId);
            var subcriberDict = custSubcribers.ToLookup(c => c.CustId, c => c.SubscriberName);
            Func<long, bool> conditionSubcriber = s => custIds.Contains((int)s);

            // filter by Evidences
            var evidenceDict = evidences.ToLookup(c => c.CustId, c => c.EvidenceCode + "-" + c.EvidenceName);
            var listEvidenceByCus = evidences.Select(c => (long)c.CustId).ToList();
            Func<long, bool> conditionExtract = f => 1.Equals(1);

            if (!string.IsNullOrEmpty(criteria.EvidenceCodeList))
            {
                conditionExtract = f => listEvidenceByCus.Contains(f);
            }

            var data = rawData.AsParallel()
               .Where(c => conditionExtract(c.CustId) && conditionSubcriber(c.CustId))
               .Where(c => conditionExtract(c.CustId))
               .Select(a =>
               {
                   a.HomeId = legend.HomeId;
                   a.AwayId = legend.AwayId;

                   var bettype = CachedRef.GetBettypeName(a.BetTypeId);
                   var grouped_bettype = GetOrder(bettype) != BettypeOrder.Other ? bettype : "Others";

                   return new FlaggedAccountTicket
                   {
                       Member = a.UserName,
                       Evidences = evidenceDict[(int)a.CustId],
                       Site = subcriberDict[(int)a.CustId].FirstOrDefault(),
                       DangerousLevel = a.DangerLevel,
                       Bettype = grouped_bettype,
                       TransDate = a.TransDate.ToString("MMM dd, yyyy hh:mm:ss tt"),
                       BetID = a.TransId.ToString(),
                       IP = a.IP,
                       Odds = new OddsDetails { Value = (double)a.Odds, Type = CachedRef.GetOddsTypeName(int.Parse(a.OddsType)), Html = html_builder.BuildOdds(a, null, ticketHelper.GetOddsTypeNameById) },
                       Score = string.Format("{0}-{1}", a.LiveHomeScore, a.LiveAwayScore),
                       Stake = a.Stake,
                       Status = a.Status,
                       WinLoss = null_winloss.Contains(a.Status.ToLowerInvariant()) ? null : new decimal?(a.Winlost),
                       Choice = html_builder.BuildChoice(a, ticketHelper, null, false),
                       ChoiceExcel = criteria.SaveChoice ? html_builder.BuildChoiceBlock(a, ticketHelper, null, false) : null,
                       BettypeRealName = bettype,
                       CurrencyName = a.CurrencyName,
                       RawData = criteria.SaveChoice ? a : null
                   };
               })
               .OrderBy(t => GetOrder(t.Bettype))
               .ThenBy(t => DateTime.Parse(t.TransDate))
               .ThenBy(t => t.Member);

            return data;
        }

        public IEnumerable<CustomerDetails> GetFlaggedAccountCustomers(FlaggedAccountCriteria criteria)
        {
            var isBettypeOther = false;

            if (criteria.Bettype == 0)
            {
                isBettypeOther = true;
                criteria.Bettype = -1;
            }

            var rawCustomers = DataService.GetData<CustInfo>(criteria);

            var custsByBettype = FilterCustomerByBetType(rawCustomers, isBettypeOther);

            return FilterCustomerDetail(custsByBettype, criteria.Sites, criteria.EvidenceCodeList);
        }

        private IEnumerable<CustomerDetails> FilterCustomerDetail(IEnumerable<CustInfo> custInfo, string sites, string evidenceCodes)
        {
            IEnumerable<CustomerEvidence> custEvidence;
            var otherEvidenceCode = AppSettings.Get("FPS_GET_OTHER_EVIDENCES");

            var custsOfSites = GetCustsOfSites(custInfo.Select(c => c.CustId), sites);

            if (!custsOfSites.Any())
            {
                return new List<CustomerDetails>();
            }

            custEvidence = GetCustOfEvidences(custsOfSites.Select(c => c.CustId), evidenceCodes);

            if (evidenceCodes != null && evidenceCodes.Contains(otherEvidenceCode))
            {
                return FilterCustomerDetailOtherEvidences(custInfo, custsOfSites, custEvidence, evidenceCodes);
            }

            return FilterCustomerDetail(custInfo, custsOfSites, custEvidence, string.IsNullOrEmpty(evidenceCodes));
        }

        private IEnumerable<CustInfo> FilterCustomerByBetType(IEnumerable<CustInfo> custInfo, bool isBetTypeOthers)
        {
            if (isBetTypeOthers)
            {
                return custInfo
                            .Where(c => !new List<int> { 1, 3, 7, 8 }.Contains(c.BettypeId))
                            .GroupBy(g => g.CustId, (key, group) => new CustInfo {
                                CustId = key,
                                Username = group.First().Username,
                                CurrencyName = group.First().CurrencyName,
                                NoOfTickets = group.Sum(c => c.NoOfTickets),
                                TotalStake = group.Sum(c => c.TotalStake),
                                TotalWinlost = group.Sum(c => c.TotalWinlost)
                            });
            }

            return custInfo.GroupBy(g => g.CustId, (key, group) => new CustInfo
            {
                CustId = key,
                Username = group.First().Username,
                CurrencyName = group.First().CurrencyName,
                NoOfTickets = group.Sum(c => c.NoOfTickets),
                TotalStake = group.Sum(c => c.TotalStake),
                TotalWinlost = group.Sum(c => c.TotalWinlost)
            });
        }

        private IEnumerable<CustomerSubcriber> GetCustsOfSites(IEnumerable<int> custIds, string sites)
        {
            var urlCustSites = AppSettings.Get("FPS_WEBAPI_CUSTOMER_SUBSCRIBER");

            Logger.Log.Info("FS: call [FPS_WEBAPI_CUSTOMER_SUBSCRIBER] - start");

            var custOfSites = custIds.Chunk(500)
                              .Select(q => ApiHelper.Post<IEnumerable<CustomerSubcriber>>(urlCustSites, new CustomerSubcriberList { CustIds = string.Join(",", q), Subscribers = sites != "All" ? sites : null }))
                              .ToArray();

            Task.WaitAll(custOfSites);

            Logger.Log.Info("FS: call [FPS_WEBAPI_CUSTOMER_SUBSCRIBER] - end");

            return custOfSites.SelectMany(t => t.Result);
        }

        private IEnumerable<CustomerEvidence> GetCustOfEvidences(IEnumerable<int> custIds, string evidenceCodeList)
        {
            var url = AppSettings.Get("FPS_WEBAPI_GET_CUSTOMER_EVIDENCE");
            var otherEvidenceCode = AppSettings.Get("FPS_GET_OTHER_EVIDENCES");

            var evidenceCodes = CheckCriteriaEvidenceOthers(evidenceCodeList, otherEvidenceCode);

            Logger.Log.Info("FS: call [FPS_WEBAPI_GET_CUSTOMER_EVIDENCE] - start");

            var tsk = custIds.Distinct().Chunk(500).Select(a => new CustomerEvidenceList
            {
                CustIdList = string.Join(",", a),
                EvidenceCodeList = evidenceCodes
            }).Select(p => ApiHelper.Post<IEnumerable<CustomerEvidence>>(url, p)).ToArray();

            Task.WaitAll(tsk);

            Logger.Log.Info("FS: call [FPS_WEBAPI_GET_CUSTOMER_EVIDENCE] - end");

            return tsk.Where(t => t.Result != null).SelectMany(t => t.Result);
        }

        public byte[] ExportFlaggedAccounts(FlaggedAccountCriteria criteria, string templatePath)
        {
            var legend = GetInfo(criteria.MatchId, 6, criteria.EventDate);
            legend.Filter = GenerateFilterInfo(criteria);
            var tickets = GetFlaggedAccountTickets(criteria);

            GetFlaggedAccountCustomerCriteria cusCriteria = ObjectHelper.ConvertObject<GetFlaggedAccountCustomerCriteria>(criteria);
            var customers = GetFlaggedAccountCustomers(cusCriteria);

            var wb =
                ExcelUtilities.GetWorkbook(templatePath)
                    .RenderSheet(new FlaggedAccountTicketConfig(tickets.Count()), legend, tickets, null)
                    .RenderSheet(new CustomerDetailConfig(customers.Count()), legend, customers, null);
            
            return wb.ToBytes();
        }

        private string[] GenerateFilterInfo(ICriteria criteria)
        {
            string[] filterInfo = new string[7];

            if(criteria is GetFlaggedAccountTicketCriteria)
            {
                var filter = criteria as GetFlaggedAccountTicketCriteria;

                // Market
                filterInfo[0] = filter.LiveStatus == -1 ? "All" : (filter.LiveStatus == 0 ? "Non-Live" : "Live");

                // Currency
                filterInfo[1] = filter.CurrencyNames;

                // Evidence name
                string strCode = string.Empty;
                var otherEvidenceCode = AppSettings.Get("FPS_GET_OTHER_EVIDENCES");
                if (otherEvidenceCode != null && filter.EvidenceCodeList != null)
                {
                    strCode = filter.EvidenceCodeList;
                    strCode = strCode.Replace(otherEvidenceCode, "Others");
                    strCode = strCode.Replace(',', '/');
                }

                filterInfo[2] = ReplaceEvidences(strCode);

                // From date, to date
                filterInfo[3] = filter.TransDateFromString != null ? filter.TransDateFromString.Replace(',', '-') : string.Empty;
                filterInfo[4] = filter.TransDateToString != null ? filter.TransDateToString.Replace(',', '-') : string.Empty;

                // Bet type
                filterInfo[5] = filter.Bettype == -1 ? "All" : (filter.Bettype == 0 ? "Others" : CachedRef.GetBettypeName(filter.Bettype));

                // Sites
                filterInfo[6] = filter.Sites;
            }            
            
            return filterInfo;
        }

        public IEnumerable<BetTypes> GetBetTypes()
        {
            var lstBetTypes = new List<BetTypes>();
            lstBetTypes.Add(new BetTypes { BetType = 1 });
            lstBetTypes.Add(new BetTypes { BetType = 3 });
            lstBetTypes.Add(new BetTypes { BetType = 7 });
            lstBetTypes.Add(new BetTypes { BetType = 8 });

            return lstBetTypes;
        }

        private static string CheckCriteriaEvidenceOthers(string code, string otherEvidenceCodes)
        {
            if (code == null || code.Contains(otherEvidenceCodes))
            {
                return null;
            }

            return code;
        }

        #endregion

        #region Arbitrage
        public IEnumerable<CustomerDetails> GetArbitrageCustomerDetails(GetArbitrageCustomerCriteria criteria)
        {
            var rawCustomers = DataService.GetData<CustInfo>(criteria);

            return FilterCustomerDetail(rawCustomers, criteria.Sites, criteria.EvidenceCodeList);
        }        

        private IEnumerable<CustomerDetails> FilterCustomerDetailOtherEvidences(IEnumerable<CustInfo> custInfo, IEnumerable<CustomerSubcriber> custSubcribers, IEnumerable<CustomerEvidence> custEvidence, string lstEvidenceSelected)
        {
            // fiter by Sites
            var custIds = custSubcribers.Select(c => c.CustId);
            Func<long, bool> conditionSubcriber = s => custIds.Contains((int)s);

            // filter by Evidences
            var distinctedCustEvidence = custEvidence.Distinct();
            var evidenceExtract = distinctedCustEvidence.Where(c => !lstEvidenceSelected.Contains(c.EvidenceCode));
            var lstEvidenceExtract = evidenceExtract.Select(c => (long)c.CustId).ToList();
            var totalAssociation = GetTotalAssociations(custInfo.Select(i => i.CustId)).ToDictionary(o => o.CustId, o => o.TotalAssociation);
            Func<long, bool> conditionExtract = f => !lstEvidenceExtract.Contains(f);

            IEnumerable<CustomerDetails> custDetails = custInfo
                   .Where(c => conditionExtract(c.CustId) && conditionSubcriber(c.CustId))
                   .Where(c => conditionExtract(c.CustId))
                   .GroupJoin(
                   distinctedCustEvidence,
                   info => info.CustId,
                   evid => evid.CustId,
                   (info, evidence) => new CustomerDetails
                   {
                       CustId = info.CustId,
                       Member = info.Username,
                       TotalWinLoss = (double)info.TotalWinlost,
                       TotalStake = (double)info.TotalStake,
                       Tickets = info.NoOfTickets,
                       Evidence = string.Join(", ", evidence.Distinct().Select(e => e.EvidenceCode + "-" + e.EvidenceName)),
                       AssociatedAccounts = totalAssociation.ContainsKey(info.CustId) ? totalAssociation[info.CustId] : 0,
                       CurrencyName = info.CurrencyName
                   }).OrderBy(c => c.Member);

            return custDetails;
        }

        private IEnumerable<CustomerDetails> FilterCustomerDetail(IEnumerable<CustInfo> custInfo, IEnumerable<CustomerSubcriber> custSubcribers, IEnumerable<CustomerEvidence> custEvidence, bool isAllEvidence)
        {
            // fiter by Sites
            var custIds = custSubcribers.Select(c => c.CustId);
            Func<long, bool> conditionSubcriber = s => custIds.Contains((int)s);

            // filter by Evidences
            var distinctedCustEvidence = custEvidence.Distinct();
            Func<long, bool> conditionExtract = f => 1.Equals(1);

            if (!isAllEvidence)
            {
                var listEvidenceByCus = distinctedCustEvidence.Select(c => (long)c.CustId).ToList();
                conditionExtract = f => listEvidenceByCus.Contains(f);
            }

            var totalAssociation = GetTotalAssociations(custInfo.Select(i => i.CustId)).ToDictionary(o => o.CustId, o => o.TotalAssociation);

            IEnumerable<CustomerDetails> custDetails = custInfo
                   .Where(c => conditionExtract(c.CustId) && conditionSubcriber(c.CustId))
                   .GroupJoin(
                   distinctedCustEvidence,
                   info => info.CustId,
                   evid => evid.CustId,
                   (info, evidence) => new CustomerDetails
                   {
                       CustId = info.CustId,
                       Member = info.Username,
                       TotalWinLoss = (double)info.TotalWinlost,
                       TotalStake = (double)info.TotalStake,
                       Tickets = info.NoOfTickets,
                       Evidence = string.Join(", ", evidence.Distinct().Select(e => e.EvidenceCode + "-" + e.EvidenceName)),
                       AssociatedAccounts = totalAssociation.ContainsKey(info.CustId) ? totalAssociation[info.CustId] : 0,
                       CurrencyName = info.CurrencyName
                   }).OrderBy(c => c.Member);

            return custDetails;
        }
       
        public IEnumerable<ArbitrageGroupTicketList> GetArbitrageTickets(GetArbitrageTicketCriteria criteria, bool isSummary)
        {
            IEnumerable<ArbitrageGroupTicketList> ticketDetails;
            var otherEvidenceCodes = AppSettings.Get("FPS_GET_OTHER_EVIDENCES");

            var rawTickets = DataService.GetData<TicketInfo>(criteria);

            var sites = criteria.Sites != "All" ? criteria.Sites : null;
            
            var custOfSites = FilterCustomersBySites(rawTickets, sites);

            if (!custOfSites.Any())
            {
                return new List<ArbitrageGroupTicketList>();
            }

            var evidenceCodes = CheckCriteriaEvidenceOthers(criteria.EvidenceCodeList, otherEvidenceCodes);

            var custOfEvidences = FilterCustomersByEvidences(custOfSites, evidenceCodes);
            
            if (criteria.EvidenceCodeList != null && criteria.EvidenceCodeList.Contains(otherEvidenceCodes))
            {
                ticketDetails = FilterArbitrageTicketsEvidenceOthers(rawTickets, custOfSites, custOfEvidences, criteria.EvidenceCodeList);

                if (!isSummary)
                {
                    ticketDetails = ticketDetails.Where(g => g.GroupId == criteria.GroupId && g.Score == criteria.Score && g.Handicap == (decimal)criteria.Handicap);
                }

                return ticketDetails;
            }

            ticketDetails = FilterArbitrageTickets(rawTickets, custOfSites, custOfEvidences, string.IsNullOrEmpty(criteria.EvidenceCodeList));

            if (!isSummary)
            {
                ticketDetails = ticketDetails.Where(g => g.GroupId == criteria.GroupId && g.Score == criteria.Score && g.Handicap == (decimal)criteria.Handicap);
            }

            return ticketDetails;
        }

        private IEnumerable<ArbitrageGroupTicketList> FilterArbitrageTicketsEvidenceOthers(IEnumerable<TicketInfo> rawData, IEnumerable<CustomerSubcriber> custSubcribers, IEnumerable<CustomerEvidence> evidences, string lstEvidenceSelected)
        {
            // fiter by Sites
            var custIds = custSubcribers.Select(c => c.CustId);
            var subcriberDict = custSubcribers.ToLookup(c => c.CustId, c => c.SubscriberName);
            Func<long, bool> conditionSubcriber = s => custIds.Contains((int)s);

            // filter by Evidences
            var evidenceDict = evidences.ToLookup(c => c.CustId, c => c.EvidenceCode + "-" + c.EvidenceName);
            var evidenceExtract = evidences.Where(c => !lstEvidenceSelected.Contains(c.EvidenceCode));
            var lstEvidenceExtract = evidenceExtract.Select(c => (long)c.CustId).ToList();

            Func<long, bool> conditionExtract = f => !lstEvidenceExtract.Contains(f);

            var data = rawData
                .AsParallel()
                .Where(c => conditionExtract(c.CustId) && conditionSubcriber(c.CustId))
                .GroupBy(t => Tuple.Create(t.BetTypeId, t.Handicap1 - t.Handicap2, t.LiveHomeScore, t.LiveAwayScore, t.GroupId), t => new ArbitrageTicket
                {
                    BetId = t.TransId,
                    Currency = t.CurrencyName,
                    DangerousLevel = t.DangerLevel,
                    Site = subcriberDict[(int)t.CustId].FirstOrDefault(),
                    Evidences = evidenceDict[(int)t.CustId],
                    Odds = t.Odds,
                    RawTransDate = t.TransDate,
                    Username = t.UserName,
                    BetTeam = t.BetTeam,
                    Stake = new ArbitrageStake
                    {
                        ComPT = t.ComPTStake,
                        Virtual = t.VirtualStake,
                        Full = t.FullStake
                    },                   
                    Handicap = (double)(t.Handicap1 - t.Handicap2),
                    Score = $"{t.LiveHomeScore} - {t.LiveAwayScore}",
                    IsIPFraud = t.IsIPFraud
                })
                   .OrderBy(g => g.Key.Item1).ThenBy(g => g.Key.Item3).ThenBy(g => g.Key.Item4).ThenBy(g => g.Key.Item2)
                    .Select(g =>
                    {
                        var tmp = g.OrderBy(t => t.RawTransDate).ThenBy(t => t.Username).ToList();
                        return new ArbitrageGroupTicketList
                        {
                            BettypeId = g.Key.Item1,
                            BettypeName = CachedRef.GetBettypeName(g.Key.Item1),
                            Away = tmp.Where(t => t.BetTeam == "a"),
                            Home = tmp.Where(t => t.BetTeam == "h"),
                            Handicap = g.Key.Item2,
                            Score = $"{g.Key.Item3} - {g.Key.Item4}",
                            LiveHomeScore = g.Key.Item3,
                            LiveAwayScore = g.Key.Item4,
                            GroupId = g.Key.Item5
                        };
                    }).ToList();
          
            return data;
        }

        private IEnumerable<ArbitrageGroupTicketList> FilterArbitrageTickets(IEnumerable<TicketInfo> rawData, IEnumerable<CustomerSubcriber> custSubcribers, IEnumerable<CustomerEvidence> evidences, bool isAllEvidence)
        {
            // fiter by Sites
            var custIds = custSubcribers.Select(c => c.CustId);
            var subcriberDict = custSubcribers.ToLookup(c => c.CustId, c => c.SubscriberName);
            Func<long, bool> conditionSubcriber = s => custIds.Contains((int)s);

            // filter by Evidences
            var evidenceDict = evidences.ToLookup(c => c.CustId, c => c.EvidenceCode + "-" + c.EvidenceName);
            var listEvidenceByCus = evidences.Select(c => (long)c.CustId).ToList();
            Func<long, bool> conditionExtract = f => 1.Equals(1);

            if (!isAllEvidence)
            {
                conditionExtract = f => listEvidenceByCus.Contains(f);
            }

            var data = rawData
                .AsParallel()
                .Where(c => conditionExtract(c.CustId) && conditionSubcriber(c.CustId))
                .GroupBy(t => Tuple.Create(t.BetTypeId, t.Handicap1 - t.Handicap2, t.LiveHomeScore, t.LiveAwayScore, t.GroupId), t => new ArbitrageTicket
                {
                    BetId = t.TransId,
                    Currency = t.CurrencyName,
                    DangerousLevel = t.DangerLevel,
                    Site = subcriberDict[(int)t.CustId].FirstOrDefault(),
                    Evidences = evidenceDict[(int)t.CustId],
                    Odds = t.Odds,
                    RawTransDate = t.TransDate,
                    Username = t.UserName,
                    BetTeam = t.BetTeam,
                    Stake = new ArbitrageStake
                    {
                        ComPT = t.ComPTStake,
                        Virtual = t.VirtualStake,
                        Full = t.FullStake
                    },                                       
                    Handicap = (double)(t.Handicap1 - t.Handicap2),
                    Score = $"{t.LiveHomeScore} - {t.LiveAwayScore}",
                    IsIPFraud = t.IsIPFraud
                })
                .OrderBy(g => g.Key.Item1).ThenBy(g=>g.Key.Item3).ThenBy(g=>g.Key.Item4).ThenBy(g => g.Key.Item2)
                .Select(g =>
                {
                    var tmp = g.OrderBy(t => t.RawTransDate).ThenBy(t => t.Username).ToList();
                    return new ArbitrageGroupTicketList
                    {
                        BettypeId = g.Key.Item1,
                        BettypeName = CachedRef.GetBettypeName(g.Key.Item1),                        
                        Away = tmp.Where(t => t.BetTeam == "a"),
                        Home = tmp.Where(t => t.BetTeam == "h"),
                        Handicap = g.Key.Item2,
                        Score = $"{g.Key.Item3} - {g.Key.Item4}",
                        LiveHomeScore = g.Key.Item3,
                        LiveAwayScore = g.Key.Item4,
                        GroupId = g.Key.Item5
                    };
                }).ToList();

            return data;
        }
  
        public IEnumerable<Evidence> GetListEvidences()
        {
            string otherEvidenceCode = AppSettings.Get("FPS_GET_OTHER_EVIDENCES");
            var lstEvidences = new List<Evidence>();

            lstEvidences.Add(new Evidence { EvidenceCode = "5.1", EvidenceName = "5.1 - VIP Smart Punter" });
            lstEvidences.Add(new Evidence { EvidenceCode = "5.2", EvidenceName = "5.2 - Smart Punter" });
            lstEvidences.Add(new Evidence { EvidenceCode = "5.3", EvidenceName = "5.3 - 8RM Accounts" });
            lstEvidences.Add(new Evidence { EvidenceCode = "5.4", EvidenceName = "5.4 - VIP Monitor Accounts" });
            lstEvidences.Add(new Evidence { EvidenceCode = "5.5", EvidenceName = "5.5 - Danger 5" });
            lstEvidences.Add(new Evidence { EvidenceCode = "5.6", EvidenceName = "5.6 - Danger 6" });
            lstEvidences.Add(new Evidence { EvidenceCode = "5.7", EvidenceName = "5.7 - Danger 7" });
            lstEvidences.Add(new Evidence { EvidenceCode = "5.8", EvidenceName = "5.8 - Danger 8" });
            lstEvidences.Add(new Evidence { EvidenceCode = "5.9", EvidenceName = "5.9 - Danger 9" });
            lstEvidences.Add(new Evidence { EvidenceCode = "5.10", EvidenceName = "5.10 - Danger 10" });
            lstEvidences.Add(new Evidence { EvidenceCode = "6.1", EvidenceName = "6.1 - MBC Abnormal Group 1" });
            lstEvidences.Add(new Evidence { EvidenceCode = "6.2", EvidenceName = "6.2 - Abnormal Betting - Other Bettype" });
            lstEvidences.Add(new Evidence { EvidenceCode = "6.3", EvidenceName = "6.3 - Goal Delay - Provider" });
            lstEvidences.Add(new Evidence { EvidenceCode = "6.4", EvidenceName = "6.4 - MBC Abnormal Group 2" });
            lstEvidences.Add(new Evidence { EvidenceCode = "6.5", EvidenceName = "6.5 - Licensee Abnormal Betting  1" });
            lstEvidences.Add(new Evidence { EvidenceCode = "6.6", EvidenceName = "6.6 - Hedging Group" });
            lstEvidences.Add(new Evidence { EvidenceCode = "6.7", EvidenceName = "6.7 - Licensee Fixed Game & Goal Delay" });
            lstEvidences.Add(new Evidence { EvidenceCode = "6.8", EvidenceName = "6.8 - Licensee Both Side Betting" });
            lstEvidences.Add(new Evidence { EvidenceCode = "6.9", EvidenceName = "6.9 - Licensee Monitoring Group" });
            lstEvidences.Add(new Evidence { EvidenceCode = "6.10", EvidenceName = "6.10 - Backup Group" });
            lstEvidences.Add(new Evidence { EvidenceCode = "7.1", EvidenceName = "7.1 - Abnormal - Snooker" });
            lstEvidences.Add(new Evidence { EvidenceCode = "7.2", EvidenceName = "7.2 - Smart Punter All in one" });
            lstEvidences.Add(new Evidence { EvidenceCode = "7.3", EvidenceName = "7.3 - Abnormal - Basketball" });
            lstEvidences.Add(new Evidence { EvidenceCode = "7.4", EvidenceName = "7.4 - Abnormal - Baseball and Volleyball" });
            lstEvidences.Add(new Evidence { EvidenceCode = "7.5", EvidenceName = "7.5 - Abnormal - Tennis" });
            lstEvidences.Add(new Evidence { EvidenceCode = "7.6", EvidenceName = "7.6 - Abnormal - Muay Thai and Hedging Account" });
            lstEvidences.Add(new Evidence { EvidenceCode = "7.7", EvidenceName = "7.7 - Abnormal - NFL,Dart and Ice-Hockey" });
            lstEvidences.Add(new Evidence { EvidenceCode = "7.8", EvidenceName = "7.8 - Smart Punter - E-Sport" });
            lstEvidences.Add(new Evidence { EvidenceCode = "7.9", EvidenceName = "7.9 - Abnormal - Motorsports and Cricket" });
            lstEvidences.Add(new Evidence { EvidenceCode = "7.10", EvidenceName = "7.10 - Abnormal - Badminton" });
            lstEvidences.Add(new Evidence { EvidenceCode = otherEvidenceCode, EvidenceName = "Others" });

            return lstEvidences;
        }

        public string ReplaceEvidences(string evidences)
        {
            if (!string.IsNullOrEmpty(evidences))
            {
                int totalAll = GetListEvidences().Count();
                string[] lstCode = evidences.Split('/');
                if (lstCode.Count() == totalAll)
                {
                    return "All";
                }
            }
            return evidences;
        }
      
        public IEnumerable<Currency> GetAllCurrency(bool isShowSpecialCurrencies)
        {
            var criteria = new GetCurrencyCriteria();
            criteria.IsShowSpecialCurrencies = isShowSpecialCurrencies;

            return DataService.GetData<Currency>(criteria);
        }

        public IEnumerable<ScoreHDP> GetScoreHDP(int matchId, bool isShowSpecialCurrencies)
        {
            var criteria = new GetScoreHDPCriteria
            {
                MatchId = matchId,
                IsShowSpecialCurrencies = isShowSpecialCurrencies
            };

            return DataService.GetData<ScoreHDP>(criteria);
        }          
     
        public IEnumerable<Site> GetSites()
        {
            var url = AppSettings.Get("FPS_WEBAPI_CUSTOMER_SUBSCRIBER");
            var responseSites = ApiHelper.Get<IEnumerable<string>>(url);
            Task.WaitAll(responseSites);

            var sites = responseSites.Result.OrderBy(c => c);

            List<Site> lstSites = new List<Site>();
            foreach (string item in sites)
            {
                lstSites.Add(new Site { SiteId = item, SiteName = item });
            }

            List<string> siteRemoves = new List<string> { "Alpha", "Alpha789y", "ALPHA88", "AlphaCN88" };
            lstSites.RemoveAll(c => siteRemoves.Contains(c.SiteName));

            return lstSites;
        }
    
        public byte[] ExportExcelArbitrageTickets(GetArbitrageTicketCriteria criteria, string[] array, string templatePath, bool isSummary)
        {
            var legend = GetInfo(criteria.MatchId, (int)FraudCaseId.Arbitrage, criteria.EventDate);
            legend.Filter = array;

            var groupsTicket = GetArbitrageTickets(criteria, isSummary);

            int totalTicket = groupsTicket.Sum(g => g.Home.Count() + g.Away.Count());

            var wb = ExcelUtilities.GetWorkbook(templatePath)
                                    .RenderSheet(new ArbitrageConfig(totalTicket, true), legend, groupsTicket, null);

            return wb.ToBytes();
        }

        public byte[] ExportExcelArbitrageCustomers(GetArbitrageCustomerCriteria criteria, string[] array, string templatePath)
        {
            var legend = GetInfo(criteria.MatchId, (int)FraudCaseId.Arbitrage, criteria.EventDate);
            legend.Filter = array;

            var customers = GetArbitrageCustomerDetails(criteria);

            var wb = ExcelUtilities.GetWorkbook(templatePath)
                                    .RenderSheet(new CustomerDetailConfig(customers.Count(), true), legend, customers, null);

            return wb.ToBytes();
        }
        #endregion Arbitrage
    }
}