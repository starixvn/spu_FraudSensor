namespace Fanex.NAP.FS.Library.Service
{
    using Core.BaseClasses;
    using Fanex.BetList.Core.Builder;
    using Fanex.BetList.Core.TicketHelper;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NAP.FS.Library.Core.Consts;
    using Fanex.NAP.FS.Library.Core.ExportExcel;
    using Fanex.NAP.FS.Library.Core.Extensions;
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
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class FraudCaseDetailsService : IFraudCaseDetailsService
    {
        public int environment { get; set; }

        public int mailingListId { get; set; }

        private const int maxLengthRowsDisplay = 1000;

        public HashSet<string> ticketStatuses { get; set; }

        public FraudCaseDetailsService(int env, int mailId)
        {
            environment = env;
            mailingListId = mailId;

            ticketStatuses = new HashSet<string>
            {
                Consts.TICKET_STATUS_RUNNING,
                Consts.TICKET_STATUS_REJECT
            };
        }

        #region Other Fraud Cases
        public IEnumerable<TicketDetails> GetTicketDetails(GetTicketCriteria criteria, bool saveChoice = false)
        {
            var matchInfo = GetMatchInfo(criteria.MatchId, criteria.FraudCaseId, criteria.EventDate);
            var htmlBuilder = new BetListHTMLBuilder();
            var ticketHelper = new TicketHelper();

            return DataService.GetData<TicketInfo>(criteria)
                .Select(a =>
                {
                    a.HomeId = matchInfo.HomeId;
                    a.AwayId = matchInfo.AwayId;

                    var bettype = CachedRef.GetBettypeName(a.BetTypeId);
                    var groupedBettype = GetOrder(bettype) != BettypeOrder.Others ? bettype : Consts.TICKET_BETTYPEGROUP_OTHERS;

                    return new TicketDetails
                    {
                        Member = a.UserName,
                        Bettype = groupedBettype,
                        TransDate = a.TransDate.ToString(Consts.TICKET_DATETIME_FORMAT),
                        BetID = a.TransId.ToString(),
                        IP = a.IP,
                        Odds = new OddsDetails { Value = (double)a.Odds, Type = CachedRef.GetOddsTypeName(int.Parse(a.OddsType)), Html = htmlBuilder.BuildOdds(a, null, ticketHelper.GetOddsTypeNameById) },
                        Score = string.Format(Consts.TICKET_SCORE_FORMAT, a.LiveHomeScore, a.LiveAwayScore),
                        Stake = a.Stake,
                        Status = a.Status,
                        WinLoss = ticketStatuses.Contains(a.Status.ToLowerInvariant()) ? null : new decimal?(a.Winlost),
                        Choice = htmlBuilder.BuildChoice(a, ticketHelper, null, false),
                        ChoiceExcel = saveChoice ? htmlBuilder.BuildChoiceBlock(a, ticketHelper, null, false) : null,
                        BettypeRealName = bettype,
                        CurrencyName = a.CurrencyName,
                        RawData = saveChoice ? a : null
                    };
                })
                .OrderBy(t => GetOrder(t.Bettype))
                .ThenBy(t => DateTime.Parse(t.TransDate))
                .ThenBy(t => t.Member);
        }

        public IEnumerable<CustomerDetails> GetCustomerDetails(GetCustomerCriteria criteria, bool isExportFile = false)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            using (var query = db.QueryMultiple(criteria))
            {
                var custInfo = query.Read<CustInfo>();
                var custIdCaches = custInfo.Select(i => i.CustId).Join();
                var custEvidence = (criteria.FraudCaseId == (int)FraudCaseId.Arbitrage) ? new CustomerEvidence[0] : GetCustomerEvidences(custInfo.Select(info => info.CustId));
                var totalAssociation = isExportFile ? GetTotalAssociations(custInfo.Select(i => i.CustId)).ToDictionary(o => o.CustId, o => o.TotalAssociation) : new Dictionary<int, string>();
                var defaultAssociatedAccount = environment == 2 ? Consts.DOM_DEFAULT_TOTAL_ASSOCIATED_ACCOUNT_V2 : (environment == 3 ? Consts.DOM_DEFAULT_TOTAL_ASSOCIATED_ACCOUNT_NAPFS : Consts.DOM_DEFAULT_TOTAL_ASSOCIATED_ACCOUNT);

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
                        AssociatedAccounts = isExportFile ? (totalAssociation.ContainsKey(info.CustId) ? totalAssociation[info.CustId].ToString() : "0") : defaultAssociatedAccount,
                        CurrencyName = info.CurrencyName,
                        CustIdCaches = custIdCaches
                    }).OrderBy(c => c.Member);
            }
        }

        private IEnumerable<CustomerEvidence> GetCustomerEvidences(IEnumerable<int> custIds)
        {
            var url = AppSettings.Get("FPS_WEBAPI_GET_CUSTOMER_EVIDENCE");

            var task = custIds.Distinct().Chunk(500).Select(a => new CustomerEvidenceList
            {
                CustIdList = string.Join(",", a)
            }).Select(p => ApiHelper.Post<IEnumerable<CustomerEvidence>>(url, p)).ToArray();

            Task.WaitAll(task);

            return task.Where(t => t.Result != null).SelectMany(t => t.Result);
        }

        public byte[] ExportExcelForDetails(GetTicketCriteria criteria, string templatePath)
        {
            var legend = GetMatchInfo(criteria.MatchId, criteria.FraudCaseId, criteria.EventDate);
            var tickets = GetTicketDetails(criteria, true);
            var customers = GetCustomerDetails(new GetCustomerCriteria {
                MatchId = criteria.MatchId,
                FraudCaseId = criteria.FraudCaseId,
                EventDate = criteria.EventDate,
                IsShowSpecialCurrencies = criteria.IsShowSpecialCurrencies
            }, true);

            var wb =
                ExcelUtilities.GetWorkbook(templatePath)
                    .RenderSheet(new TicketDetailConfig(tickets.Count()), legend, tickets, null)
                    .RenderSheet(new CustomerDetailConfig(customers.Count()), legend, customers, null);

            return wb.ToBytes();
        }

        public MatchFraudCaseInfo GetMatchInfo(int matchId, int fraudCaseId, DateTime eventDate)
        {
            var criteria = new GetMatchInfoCriteria
            {
                MatchId = matchId,
                EventDate = eventDate,
                FraudCaseId = fraudCaseId
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (var query = db.QueryMultiple(criteria))
                {
                    var info = query.ReadSingle<MatchInfo>();
                    var fraudCaseName = query.IsEmptyOrClosed ? "-" : query.ReadSingle<string>();
                    var home = CachedRef.GetTeamName(info.HomeId);
                    var away = CachedRef.GetTeamName(info.AwayId);
                    var eventName = string.Format("<span class='underdog'>{0}</span> - vs - <span class='underdog'>{1}</span>", home, away);
                    var htScore = string.Format(Consts.TICKET_SCORE_FORMAT, info.HTHomeScore, info.HTAwayScore);
                    var ftScore = string.Format(Consts.TICKET_SCORE_FORMAT, info.FinalHomeScore, info.FinalAwayScore);

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
            var htmlBuilder = new BetListHTMLBuilder();
            var ticketHelper = new TicketHelper();

            return GetLast7DaysInfo(custId, isShowSpecialCurrencies)
                .OrderBy(a => a.TransId)
                .Select(a =>
                {
                    var bettype = CachedRef.GetBettypeName(a.BetTypeId);
                    var groupedBettype = GetOrder(bettype) != BettypeOrder.Others ? bettype : Consts.TICKET_BETTYPEGROUP_OTHERS;

                    return new TicketDetails
                    {
                        Member = a.UserName,
                        Bettype = groupedBettype,
                        TransDate = a.TransDate.ToString(Consts.TICKET_DATETIME_FORMAT),
                        BetID = a.TransId.ToString(),
                        IP = a.IP,
                        Odds = new OddsDetails { Value = (double)a.Odds, Type = CachedRef.GetOddsTypeName(int.Parse(a.OddsType)), Html = htmlBuilder.BuildOdds(a, null, ticketHelper.GetOddsTypeNameById) },
                        Score = string.Format(Consts.TICKET_SCORE_FORMAT, a.LiveHomeScore, a.LiveAwayScore),
                        Stake = a.Stake,
                        Status = a.Status,
                        WinLoss = ticketStatuses.Contains(a.Status.ToLowerInvariant()) ? null : new decimal?(a.Winlost),
                        Choice = htmlBuilder.BuildChoice(a, ticketHelper, null, false),
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
            var b = GetAssociations(custId);
            //Logging.LogService.Info(b.Select(r => r.ToCustId).Join(","));

            return b.Select(a => new AssociatedCustomer { Username = a.ToOWUsername, Currency = a.CurrencyName })
                .Distinct(a => a.Username)
                .OrderBy(a => a.Username);
        }

        public BettypeOrder GetOrder(string bettype)
        {
            if (bettype.Contains("1H"))
            {
                var tmp_bettype = bettype.Replace("1H", "").Trim();

                if (tmp_bettype == "Handicap")
                {
                    return BettypeOrder.FH_Handicap;
                }

                if (tmp_bettype == "Over/Under")
                {
                    return BettypeOrder.FH_OverUnder;
                }
            }

            if (bettype == "Handicap")
            {
                return BettypeOrder.FT_Handicap;
            }

            if (bettype == "Over/Under")
            {
                return BettypeOrder.FT_OverUnder;
            }

            return BettypeOrder.Others;
        }

        public IEnumerable<TicketInfo> GetLast7DaysInfo(int custId, bool isShowSpecialCurrencies)
        {
            return DataService.GetData<TicketInfo>(new Last7DaysTicketGetCriteria { CustId = custId, IsShowSpecialCurrencies = isShowSpecialCurrencies });
        }

        public IEnumerable<CustomerTotalAssociation> GetTotalAssociations(IEnumerable<int> custIds)
        {
            var url = AppSettings.Get(Consts.FPS_WEBAPI_GET_TOTAL_ASSOCIATION);

            try
            {
                var custIdFromAPI = custIds.Chunk(200)
                                 .Select(q => ApiHelper.Post<string, IEnumerable<CustomerTotalAssociation>>(url, q.Join(), environment, mailingListId))
                                 .ToArray();
                Task.WhenAll(custIdFromAPI);

                return custIdFromAPI.SelectMany(t => t.Result).ToList();
            }
            catch (Exception)
            {
                var custIdFromAPINA = new List<CustomerTotalAssociation>();
                custIdFromAPINA.Add(new CustomerTotalAssociation
                {
                    CustId = -1,
                    TotalAssociation = Consts.FPS_WEBAPI_RESPONSE_NA
                });

                return custIdFromAPINA;
            }
        }

        public IEnumerable<CustomerAssociation> GetAssociations(int custId)
        {
            var url = AppSettings.Get("FPS_WEBAPI_GET_ASSOCIATION_ACCOUNT");

            var associations = ApiHelper.Post<IEnumerable<CustomerAssociation>>(url, new { CustIdList = custId.ToString() });

            Task.WhenAll(associations);

            return associations.Result;
        }

        public IEnumerable<TicketDetails> GetTicketsFromIP(string ip, bool isShowSpecialCurrencies)
        {
            var criteria = new GetTicketByIPCriteria { IP = ip, IsShowSpecialCurrencies = isShowSpecialCurrencies };
            var htmlBuilder = new BetListHTMLBuilder();
            var ticketHelper = new TicketHelper();

            return DataService.GetData<TicketInfo>(criteria)
                .OrderBy(t => t.UserName)
                .ThenBy(t => t.TransDate)
                .Select(a =>
                {
                    var bettype = CachedRef.GetBettypeName(a.BetTypeId);
                    var groupedBettype = GetOrder(bettype) != BettypeOrder.Others ? bettype : Consts.TICKET_BETTYPEGROUP_OTHERS;

                    return new TicketDetails
                    {
                        Member = a.UserName,
                        Bettype = groupedBettype,
                        TransDate = a.TransDate.ToString(Consts.TICKET_DATETIME_FORMAT),
                        BetID = a.TransId.ToString(),
                        IP = a.IP,
                        Odds = new OddsDetails { Value = (double)a.Odds, Type = CachedRef.GetOddsTypeName(int.Parse(a.OddsType)), Html = htmlBuilder.BuildOdds(a, null, ticketHelper.GetOddsTypeNameById) },
                        Score = string.Format(Consts.TICKET_SCORE_FORMAT, a.LiveHomeScore, a.LiveAwayScore),
                        Stake = a.Stake,
                        Status = a.Status,
                        WinLoss = ticketStatuses.Contains(a.Status.ToLowerInvariant()) ? null : new decimal?(a.Winlost),
                        Choice = htmlBuilder.BuildChoice(a, ticketHelper, null, false),
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

        public byte[] ExportExcelTicketsByIP(string ip, string template, bool isShowSpecialCurrencies)
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
            var isBettypeOther = false;

            if (criteria.Bettype == 0)
            {
                isBettypeOther = true;
                criteria.Bettype = -1;
            }

            var rawTickets = DataService.GetData<TicketInfo>(criteria);
            rawTickets = isBettypeOther ? rawTickets.Where(t => !(new List<int> { (int)Bettype.FT_Handicap, (int)Bettype.FT_OverUnder, (int)Bettype.FH_Handicap, (int)Bettype.FH_OverUnder }).Contains(t.BetTypeId)) : rawTickets;
            
            var custOfSites = FilterCustomersBySites(rawTickets, criteria.Sites);

            if (custOfSites.Any())
            {
                var custExistedInMS = custOfSites.Where(c => c.IsExistedInMS && !string.IsNullOrEmpty(c.SubscriberName)).ToList();
                var custOfEvidences = custExistedInMS.Any() && criteria.EvidenceCodeList != "None" ? GetCustomersByEvidences(custExistedInMS, criteria.EvidenceCodeList) : new List<CustomerEvidence>();
                custOfEvidences = custOfEvidences.Concat(custOfSites.Where(c => !c.IsExistedInMS).Select(n => new CustomerEvidence { CustId = n.CustId, EvidenceCode = null, EvidenceName = null, IsExistedInMS = false })).ToList();

                custOfEvidences = FilterCustomersByEvidences(custOfEvidences, criteria.EvidenceCodeList, criteria.OtherEvidenceCodes);

                return FilterFlaggedAccountTickets(rawTickets, custOfSites, custOfEvidences, criteria);                
            }

            return new List<FlaggedAccountTicket>();
        }

        private static IEnumerable<CustomerSubcriber> FilterCustomersBySites(IEnumerable<TicketInfo> rawTickets, string sites)
        {
            var isAllOptions = sites == "All" || sites == null;
            var isNoneOptionOnly = sites == "None";
            var isIncludeNoneOption = isAllOptions || sites.Contains("None");
            sites = sites != null ? sites.Replace("None", ""): sites;

            var urlCustSites = AppSettings.Get("FPS_WEBAPI_CUSTOMER_SUBSCRIBER");           

            var customersBySites = rawTickets.Select(t => t.CustId).Distinct().Chunk(500)
                                    .Select(q => ApiHelper.Post<IEnumerable<CustomerSubcriber>>(urlCustSites, new CustomerSubcriberList { CustIds = string.Join(",", q), Subscribers = isAllOptions ? null : sites }))
                                    .ToArray();

            Task.WhenAll(customersBySites);

            var resultsFromAPI = customersBySites.SelectMany(t => t.Result).ToList();

            if (isNoneOptionOnly)
            {
                resultsFromAPI = resultsFromAPI.Where(c => !c.IsExistedInMS).ToList();
            }
            else if (!isIncludeNoneOption)
            {
                resultsFromAPI = resultsFromAPI.Where(c => !string.IsNullOrEmpty(c.SubscriberName) && c.IsExistedInMS).ToList();
            }

            return resultsFromAPI;
        }        

        private static IEnumerable<CustomerEvidence> GetCustomersByEvidences(IEnumerable<CustomerSubcriber> customerBySites, string evidenceList)
        {
            evidenceList = evidenceList == null ? evidenceList : evidenceList.Replace("None", "").TrimEnd(',');
            var urlCustEvidences = AppSettings.Get("FPS_WEBAPI_GET_CUSTOMER_EVIDENCE");

            var customerByEvidences = customerBySites.Select(c => c.CustId).Distinct().Chunk(500)
                                       .Select(q => ApiHelper.Post<IEnumerable<CustomerEvidence>>(urlCustEvidences, new CustomerEvidenceList { CustIdList = string.Join(",", q), EvidenceCodeList = evidenceList }))
                                       .ToArray();
            Task.WhenAll(customerByEvidences);

            return customerByEvidences.SelectMany(t => t.Result).ToList();
        }
     
        private IEnumerable<FlaggedAccountTicket> FilterFlaggedAccountTickets(
            IEnumerable<TicketInfo> rawData, 
            IEnumerable<CustomerSubcriber> customerSubcribers, 
            IEnumerable<CustomerEvidence> customerEvidences, 
            FlaggedAccountCriteria criteria)
        {
            var legend = GetMatchInfo(criteria.MatchId, (int)FraudCaseId.FlaggedAccount, criteria.EventDate);
            var htmlBuilder = new BetListHTMLBuilder();
            var ticketHelper = new TicketHelper();

            // filter by Sites
            var custIds = customerSubcribers.Select(c => c.CustId);
            var customerSubcriberDict = customerSubcribers.ToLookup(c => c.CustId, c => c.SubscriberName);
            bool filterBySites(long s) => custIds.Contains((int)s);

            // filter by Evidences
            var customerEvidenceDict = customerEvidences.ToLookup(c => c.CustId, c => c.EvidenceCodeForDisplay + "-" + c.EvidenceName);
            var customersBySelectedEvidences = customerEvidences.Select(c => (long)c.CustId).ToList();
            bool filterByEvidences(long e) => customersBySelectedEvidences.Contains(e);

            var data = rawData.AsParallel()
               .Where(c => filterByEvidences(c.CustId) && filterBySites(c.CustId))
               .Select(a =>
               {
                   a.HomeId = legend.HomeId;
                   a.AwayId = legend.AwayId;

                   var bettype = CachedRef.GetBettypeName(a.BetTypeId);
                   var groupedBettype = GetOrder(bettype) != BettypeOrder.Others ? bettype : Consts.TICKET_BETTYPEGROUP_OTHERS;

                   return new FlaggedAccountTicket
                   {
                       Member = a.UserName,
                       Evidences = customerEvidenceDict[(int)a.CustId],
                       Site = customerSubcriberDict[(int)a.CustId].FirstOrDefault(),
                       DangerousLevel = a.DangerLevel,
                       Bettype = groupedBettype,
                       TransDate = a.TransDate.ToString(Consts.TICKET_DATETIME_FORMAT),
                       BetID = a.TransId.ToString(),
                       IP = a.IP,
                       Odds = new OddsDetails { Value = (double)a.Odds, Type = CachedRef.GetOddsTypeName(int.Parse(a.OddsType)), Html = htmlBuilder.BuildOdds(a, null, ticketHelper.GetOddsTypeNameById) },
                       Score = string.Format(Consts.TICKET_SCORE_FORMAT, a.LiveHomeScore, a.LiveAwayScore),
                       Stake = a.Stake,
                       Status = a.Status,
                       WinLoss = ticketStatuses.Contains(a.Status.ToLowerInvariant()) ? null : new decimal?(a.Winlost),
                       Choice = htmlBuilder.BuildChoice(a, ticketHelper, null, false),
                       ChoiceExcel = criteria.SaveChoice ? htmlBuilder.BuildChoiceBlock(a, ticketHelper, null, false) : null,
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

        public IEnumerable<CustomerDetails> GetFlaggedAccountCustomers(FlaggedAccountCriteria criteria, bool isExportFile = false)
        {
            var isBettypeOthers = false;

            if (criteria.Bettype == 0)
            {
                isBettypeOthers = true;
                criteria.Bettype = -1;
            }

            var rawCustomers = DataService.GetData<CustInfo>(criteria);
            var custsByBettype = FilterCustomerByBetType(rawCustomers, isBettypeOthers);

            return FilterCustomerDetail(custsByBettype, criteria.Sites, criteria.EvidenceCodeList, criteria.OtherEvidenceCodes, isExportFile);
        }

        private IEnumerable<CustomerDetails> FilterCustomerDetail(
            IEnumerable<CustInfo> custInfo, 
            string sites, 
            string evidenceCodes, 
            string otherEvidenceCode,
            bool isExportFile)
        {
            var custsOfSites = GetCustsOfSites(custInfo.Select(c => c.CustId), sites);

            if (custsOfSites.Any())
            {
                var custExistedInMS = custsOfSites.Where(c => c.IsExistedInMS && !string.IsNullOrEmpty(c.SubscriberName)).Select(c => c.CustId).ToList();
                var custOfEvidences = custExistedInMS.Any() && evidenceCodes != "None" ? GetCustOfEvidences(custExistedInMS, evidenceCodes) : new List<CustomerEvidence>();
                custOfEvidences = custOfEvidences.Concat(custsOfSites.Where(c => !c.IsExistedInMS && string.IsNullOrEmpty(c.SubscriberName))
                                                                    .Select(n => new CustomerEvidence { CustId = n.CustId, EvidenceCode = null, EvidenceName = null, IsExistedInMS = false }));

                custOfEvidences = FilterCustomersByEvidences(custOfEvidences, evidenceCodes, otherEvidenceCode);

                return FilterCustomerDetails(custInfo, custsOfSites, custOfEvidences, isExportFile);
            }

            return new List<CustomerDetails>();
        }

        private IEnumerable<CustInfo> FilterCustomerByBetType(IEnumerable<CustInfo> custInfo, bool isBetTypeOthers)
        {
            if (isBetTypeOthers)
            {
                return custInfo
                            .Where(c => !new List<int> { (int)Bettype.FT_Handicap, (int)Bettype.FT_OverUnder, (int)Bettype.FH_Handicap, (int)Bettype.FH_OverUnder }.Contains(c.BettypeId))
                            .GroupBy(g => g.CustId, (key, group) => new CustInfo
                            {
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
            var isAllOptions = sites == Consts.FRAUD_CASE_FILTER_ALL || sites == null;
            var isNoneOptionOnly = sites == "None";
            var isIncludeNoneOption = isAllOptions || sites.Contains("None");
            sites = isAllOptions ? sites : sites.Replace("None", "");

            var urlCustSites = AppSettings.Get("FPS_WEBAPI_CUSTOMER_SUBSCRIBER");

            var custOfSites = custIds.Chunk(500).Select(q => ApiHelper.Post<IEnumerable<CustomerSubcriber>>(urlCustSites, new CustomerSubcriberList { CustIds = string.Join(",", q), Subscribers = sites != Consts.FRAUD_CASE_FILTER_ALL ? sites : null }))
                              .ToArray();

            Task.WhenAll(custOfSites);

            var resultsFromAPI = custOfSites.SelectMany(t => t.Result).ToList();

            if (isNoneOptionOnly)
            {
                resultsFromAPI = resultsFromAPI.Where(c => !c.IsExistedInMS).ToList();
            }
            else if (!isIncludeNoneOption)
            {
                resultsFromAPI = resultsFromAPI.Where(c => !string.IsNullOrEmpty(c.SubscriberName) && c.IsExistedInMS).ToList();
            }

            return resultsFromAPI;
        }

        private IEnumerable<CustomerEvidence> GetCustOfEvidences(IEnumerable<int> custIds, string evidenceCodeList)
        {
            var url = AppSettings.Get("FPS_WEBAPI_GET_CUSTOMER_EVIDENCE");
            var evidenceCodes = evidenceCodeList != null ? evidenceCodeList.Replace("None", "").TrimEnd(',') : evidenceCodeList;

            var custOfEvidences = custIds.Distinct().Chunk(500).Select(a => new CustomerEvidenceList
            {
                CustIdList = string.Join(",", a),
                EvidenceCodeList = evidenceCodes
            }).Select(p => ApiHelper.Post<IEnumerable<CustomerEvidence>>(url, p)).ToArray();

            Task.WhenAll(custOfEvidences);

            return custOfEvidences.Where(t => t.Result != null).SelectMany(t => t.Result);
        }       

        public byte[] ExportExcelFlaggedAccounts(FlaggedAccountCriteria criteria, string templatePath)
        {
            var legend = GetMatchInfo(criteria.MatchId, (int)FraudCaseId.FlaggedAccount, criteria.EventDate);
            legend.Filter = GenerateExcelFileFilterInfo(criteria);
            var tickets = GetFlaggedAccountTickets(criteria);

            GetFlaggedAccountCustomerCriteria cusCriteria = ObjectHelper.ConvertObject<GetFlaggedAccountCustomerCriteria>(criteria);
            var customers = GetFlaggedAccountCustomers(cusCriteria, true);

            var wb =
                ExcelUtilities.GetWorkbook(templatePath)
                    .RenderSheet(new FlaggedAccountTicketConfig(tickets.Count()), legend, tickets, null)
                    .RenderSheet(new CustomerDetailConfig(customers.Count()), legend, customers, null);

            return wb.ToBytes();
        }

        private string[] GenerateExcelFileFilterInfo(ICriteria criteria)
        {
            string[] filterInfo = new string[7];

            if (criteria is GetFlaggedAccountTicketCriteria)
            {
                var filter = criteria as GetFlaggedAccountTicketCriteria;

                // Market
                filterInfo[0] = filter.LiveStatus == -1 ? "All" : (filter.LiveStatus == 0 ? "Non-Live" : "Live");

                // Currency
                filterInfo[1] = filter.CurrencyNames;

                // Evidence name
                string strCode = string.Empty;
                var otherEvidenceCode = filter.OtherEvidenceCodes;
                if (otherEvidenceCode != null && filter.EvidenceCodeList != null)
                {
                    strCode = filter.EvidenceCodeList;
                    strCode = strCode.Replace(otherEvidenceCode, Consts.TICKET_BETTYPEGROUP_OTHERS);
                    strCode = strCode.Replace(',', '/');
                }

                filterInfo[2] = ReplaceFilterEvidences(strCode);

                // From date, to date
                filterInfo[3] = filter.TransDateFromString != null ? filter.TransDateFromString.Replace(',', '-') : string.Empty;
                filterInfo[4] = filter.TransDateToString != null ? filter.TransDateToString.Replace(',', '-') : string.Empty;

                // Bet type
                filterInfo[5] = filter.Bettype == -1 ? "All" : (filter.Bettype == 0 ? Consts.TICKET_BETTYPEGROUP_OTHERS : CachedRef.GetBettypeName(filter.Bettype));

                // Sites
                filterInfo[6] = filter.Sites;
            }

            return filterInfo;
        }

        public IEnumerable<BetTypes> GetBetTypes()
        {
            return new List<BetTypes>
            {
                new BetTypes { BetType = (int)Bettype.FT_Handicap },
                new BetTypes { BetType = (int)Bettype.FT_OverUnder },
                new BetTypes { BetType = (int)Bettype.FH_Handicap },
                new BetTypes { BetType = (int)Bettype.FH_OverUnder }
            };
        }     

        #endregion

        #region Arbitrage
        public IEnumerable<CustomerDetails> GetArbitrageCustomerDetails(GetArbitrageCustomerCriteria criteria, int totalTicketCustomer)
        {
            return GetArbitrageCustomerDetails(criteria, totalTicketCustomer, false);
        }

        private IEnumerable<CustomerDetails> GetArbitrageCustomerDetails(GetArbitrageCustomerCriteria criteria, int totalTicketCustomer, bool isExportFile)
        {
            var rawCustomers = DataService.GetData<CustInfo>(criteria);

            if (criteria.Sites == Consts.FRAUD_CASE_FILTER_ALL 
                && ReplaceFilterEvidences(criteria.EvidenceCodeList) == Consts.FRAUD_CASE_FILTER_ALL
                && totalTicketCustomer >= maxLengthRowsDisplay 
                && !isExportFile)
            {
                return GetCustomerDetailsForMuchData(rawCustomers);
            }

            return FilterCustomerDetail(rawCustomers, criteria.Sites, criteria.EvidenceCodeList, criteria.OtherEvidenceCodes, isExportFile);
        }

        private List<CustomerDetailsForMuchData> GetCustomerDetailsForMuchData(IEnumerable<CustInfo> custInfo)
        {
            var customers = new List<CustomerDetailsForMuchData>();
            var totalCustomers = custInfo.Count();
            var custIdList = custInfo.Select(cust => cust.CustId);

            customers.Add(new CustomerDetailsForMuchData { TotalCustomers = totalCustomers, CustIdList = custIdList });

            return customers;
        }        

        private IEnumerable<CustomerDetails> FilterCustomerDetails(
            IEnumerable<CustInfo> custInfo, 
            IEnumerable<CustomerSubcriber> custSubcribers, 
            IEnumerable<CustomerEvidence> custEvidences,
            bool isExportFile)
        {
            var customerEvidenceDict = custEvidences.ToLookup(c => c.CustId, c => c.EvidenceCodeForDisplay + "-" + c.EvidenceName);
            var defaultAssociatedAccount = environment == 2 ? Consts.DOM_DEFAULT_TOTAL_ASSOCIATED_ACCOUNT_V2 : (environment == 3 ? Consts.DOM_DEFAULT_TOTAL_ASSOCIATED_ACCOUNT_NAPFS : Consts.DOM_DEFAULT_TOTAL_ASSOCIATED_ACCOUNT);
            var custIdCaches = custInfo.Select(i => i.CustId).Join();
            var custExistedInMS = custEvidences.Where(c => c.IsExistedInMS).ToList();
            var totalAssociation = isExportFile && custExistedInMS.Any() ? GetTotalAssociations(custInfo.Select(i => i.CustId)) : new List<CustomerTotalAssociation>();
            var dicTotalAssociation = totalAssociation.ToDictionary(o => o.CustId, o => o.TotalAssociation);

            var custDetails = custInfo.Distinct().AsEnumerable()
                .GroupJoin(custSubcribers.Distinct(),
                    custA => custA.CustId,
                    custB => custB.CustId,
                    (custA, custB) => new CustomerDetails
                    {
                        CustId = custA.CustId,
                        Member = custA.Username,
                        TotalWinLoss = (double)custA.TotalWinlost,
                        TotalStake = (double)custA.TotalStake,
                        Tickets = custA.NoOfTickets,
                        Evidence = string.Join(", ", customerEvidenceDict[custA.CustId]),
                        AssociatedAccounts = isExportFile ? (dicTotalAssociation.ContainsKey(custA.CustId) ? dicTotalAssociation[custA.CustId].ToString() : "0") : defaultAssociatedAccount,
                        CurrencyName = custA.CurrencyName,
                        CustIdCaches = custIdCaches
                    })
                .GroupJoin(custEvidences.Distinct(),
                     custA => custA.CustId,
                    custB => custB.CustId,
                    (custA, custB) => custA)
                .OrderBy(c => c.Member);

            return custDetails;
        }

        public IEnumerable<ArbitrageGroupTicketList> GetArbitrageTickets(GetArbitrageTicketCriteria criteria, bool isSummary)
        {
            IEnumerable<ArbitrageGroupTicketList> ticketDetails = new List<ArbitrageGroupTicketList>(); 
            criteria.Site = criteria.IsOnlyCN88 ? "CN88" : criteria.Site;
            var rawTickets = DataService.GetData<TicketInfo>(criteria);
            var custOfSites = FilterCustomersBySites(rawTickets, criteria.Sites);

            if (custOfSites.Any())
            {
                var custExistedInMS = custOfSites.Where(c => c.IsExistedInMS && !string.IsNullOrEmpty(c.SubscriberName)).ToList();
                var custOfEvidences = custExistedInMS.Any() && criteria.EvidenceCodeList != "None" ? GetCustomersByEvidences(custExistedInMS, criteria.EvidenceCodeList) : new List<CustomerEvidence>();
                custOfEvidences = custOfEvidences.Concat(custOfSites.Where(c => !c.IsExistedInMS).Select(n => new CustomerEvidence { CustId = n.CustId, EvidenceCode = null, EvidenceName = null, IsExistedInMS = false })).ToList();

                custOfEvidences = FilterCustomersByEvidences(custOfEvidences, criteria.EvidenceCodeList, criteria.OtherEvidenceCodes);

                ticketDetails = FilterArbitrageTickets(rawTickets, custOfSites, custOfEvidences, criteria.IsByIPAndDevice, criteria.IsOnlyCN88);

                if (!isSummary)
                {
                    ticketDetails = ticketDetails.Where(g => g.GroupId == criteria.GroupId && g.Score == criteria.Score && g.Handicap == (decimal)criteria.Handicap);
                    if (criteria.IsByIPAndDevice)
                    {
                        ticketDetails = ticketDetails.Where(g => g.IP == criteria.IP);
                    }
                }
            }

            return ticketDetails;
        }

        private static IEnumerable<CustomerEvidence> FilterCustomersByEvidences(IEnumerable<CustomerEvidence> custOfEvidences, string evidenceCodes, string otherEvidenceCodes)
        {
            if (evidenceCodes != null)
            {
                var evidenceCodeList = evidenceCodes.Split(',').ToList();
                var custOfEvidencesFilter = custOfEvidences.Where(c => evidenceCodeList.Contains(c.EvidenceCode));

                if (evidenceCodes.Contains("None"))
                {
                    custOfEvidencesFilter = custOfEvidencesFilter.Concat(custOfEvidences.Where(c => c.EvidenceCode == null && !c.IsExistedInMS));
                }

                if (evidenceCodes.Contains(otherEvidenceCodes))
                {
                    custOfEvidencesFilter = custOfEvidencesFilter.Concat(custOfEvidences.Where(c => c.EvidenceCode == null && c.IsExistedInMS));
                }

                return custOfEvidencesFilter;
            }            

            return custOfEvidences;
        }

        private IEnumerable<ArbitrageGroupTicketList> GenerateArbitrageTicketsByIPAndDevice(
            IEnumerable<TicketInfo> rawData, 
            ILookup<int,string> customerSubscriberDict, 
            ILookup<int,string> customerEvidenceDict,
            bool isShowCN88Only)
        {
            Func<IEnumerable<ArbitrageTicket>, IEnumerable<ArbitrageTicket>, bool, bool> CheckArbitrageRule = (homeTickets, awayTickets, showCN88Only)
                    => homeTickets.Count() > 0 && awayTickets.Count() > 0 && homeTickets.Sum(t => t.Stake.Full) > 500 && awayTickets.Sum(t => t.Stake.Full) > 500 &&
                    ((showCN88Only ? (homeTickets.Where(t => t.Site == "CN88").Count() > 0) : true) || (!showCN88Only || (awayTickets.Where(t => t.Site == "CN88").Count() > 0)));

            return rawData
                .AsParallel()
                .GroupBy(t => Tuple.Create(t.BetTypeId, t.Handicap1 - t.Handicap2, t.LiveHomeScore, t.LiveAwayScore, t.GraphDBGroupId, t.IP), t => new ArbitrageTicket
                {
                    CustId = t.CustId,
                    BetId = t.TransId,
                    Currency = t.CurrencyName,
                    DangerousLevel = t.DangerLevel,
                    Site = customerSubscriberDict[(int)t.CustId].FirstOrDefault(),
                    Evidences = customerEvidenceDict[(int)t.CustId],
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
                    IsIPFraud = t.IsIPFraud,
                    IP = t.IP
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
                        GroupId = g.Key.Item5,
                        IP = g.Key.Item6
                    };
                }).Where(a => CheckArbitrageRule(a.Home, a.Away, isShowCN88Only));
        }

        private IEnumerable<ArbitrageGroupTicketList> GenerateArbitrageTickets(
            IEnumerable<TicketInfo> rawData,
            ILookup<int, string> customerSubscriberDict,
            ILookup<int, string> customerEvidenceDict)
        {
            return rawData
                .AsParallel()
                .GroupBy(t => Tuple.Create(t.BetTypeId, t.Handicap1 - t.Handicap2, t.LiveHomeScore, t.LiveAwayScore, t.GroupId), t => new ArbitrageTicket
                {
                    CustId = t.CustId,
                    BetId = t.TransId,
                    Currency = t.CurrencyName,
                    DangerousLevel = t.DangerLevel,
                    Site = customerSubscriberDict[(int)t.CustId].FirstOrDefault(),
                    Evidences = customerEvidenceDict[(int)t.CustId],
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
                    IsIPFraud = t.IsIPFraud,
                    IP = t.IP
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
                });
        }        

        private IEnumerable<ArbitrageGroupTicketList> FilterArbitrageTickets(
            IEnumerable<TicketInfo> rawData,
            IEnumerable<CustomerSubcriber> custSubcribers, 
            IEnumerable<CustomerEvidence> custEvidences,
            bool isByIPAndDevice, bool isShowCN88Only)
        {
            // filter by Sites
            var custIds = custSubcribers.Select(c => c.CustId);
            var customerSubscriberDict = custSubcribers.ToLookup(c => c.CustId, c => c.SubscriberName);
            bool filterBySites(long s) => custIds.Contains((int)s);

            // filter by Evidences
            var customerEvidenceDict = custEvidences.ToLookup(c => c.CustId, c => c.EvidenceCodeForDisplay + "-" + c.EvidenceName);
            var allCustomers = custEvidences.Select(c => (long)c.CustId).ToList();
            bool filterByEvidences(long e) => allCustomers.Contains(e);
            
            return isByIPAndDevice
                ? GenerateArbitrageTicketsByIPAndDevice(rawData.AsParallel().Where(c => filterByEvidences(c.CustId) && filterBySites(c.CustId) && !c.IsIPFraud),
                        customerSubscriberDict, customerEvidenceDict, isShowCN88Only)
                : GenerateArbitrageTickets(rawData.AsParallel().Where(c => filterByEvidences(c.CustId) && filterBySites(c.CustId)),
                        customerSubscriberDict, customerEvidenceDict);
        }        

        public IEnumerable<Evidence> GetListEvidences()
        {
            var allEvidences = GetAllEvidencesFromAPI();
            var mostUsedEvidences = GetMostUsedEvidences();
            var mostUsedEvidenceCodes = mostUsedEvidences.Select(e => e.EvidenceCode);
            var otherEvidenceCodes = allEvidences.AsEnumerable()
                                .Where(e => !mostUsedEvidenceCodes.Contains(e.EvidenceCode))
                                .Select(e => e.EvidenceCode).Join();

            mostUsedEvidences.Add(new Evidence { EvidenceCode = otherEvidenceCodes, EvidenceName = Consts.TICKET_BETTYPEGROUP_OTHERS, IsOther = true });

            return mostUsedEvidences;
        }
        
        private List<Evidence> GetMostUsedEvidences()
        {
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

            return lstEvidences;
        }

        private static IEnumerable<Evidence> GetAllEvidencesFromAPI()
        {
            var url = AppSettings.Get(Consts.FPS_WEBAPI_GET_ALL_EVIDENCE);

            var evidences = ApiHelper.Post<IEnumerable<Evidence>>(url, null);

            Task.WaitAll(evidences);

            return evidences.Result;
        }

        public string ReplaceFilterEvidences(string evidences)
        {
            if (!string.IsNullOrEmpty(evidences))
            {
                int totalEvidences = GetAllEvidencesFromAPI().Count();
                string[] evidenceCodes = evidences.Split(',');

                if (evidenceCodes.Count() == totalEvidences + 1)
                {
                    return Consts.FRAUD_CASE_FILTER_ALL;
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

        public IEnumerable<ScoreHDP> GetScoreHDP(GetScoreHDPCriteria criteria)
        {
            return DataService.GetData<ScoreHDP>(criteria);
        }

        public IEnumerable<Site> GetSites()
        {
            var url = AppSettings.Get("FPS_WEBAPI_CUSTOMER_SUBSCRIBER");

            if (string.IsNullOrEmpty(url))
            {
                return Enumerable.Empty<Site>();
            }

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
            var legend = GetMatchInfo(criteria.MatchId, (int)FraudCaseId.Arbitrage, criteria.EventDate);
            legend.Filter = array;

            var groupsTicket = GetArbitrageTickets(criteria, isSummary);
            int totalTicket = groupsTicket.Sum(g => g.Home.Count() + g.Away.Count());
            var wb = ExcelUtilities.GetWorkbook(templatePath)
                                    .RenderSheet(new ArbitrageConfig(totalTicket, true), legend, groupsTicket, null);

            return wb.ToBytes();
        }

        public byte[] ExportExcelArbitrageCustomers(GetArbitrageCustomerCriteria criteria, string[] array, string templatePath, IEnumerable<long> listCustIdFromTickets)
        {
            var legend = GetMatchInfo(criteria.MatchId, (int)FraudCaseId.Arbitrage, criteria.EventDate);
            legend.Filter = array;

            var customers = GetArbitrageCustomerDetails(criteria, listCustIdFromTickets.Count(), true);

            if (listCustIdFromTickets != null)
            {
                customers = customers.Where(c => listCustIdFromTickets.Contains(c.CustId));
            }
            var wb = ExcelUtilities.GetWorkbook(templatePath)
                                    .RenderSheet(new CustomerDetailConfig(customers.Count(), true), legend, customers, null);

            return wb.ToBytes();
        }
        #endregion Arbitrage
    }
}