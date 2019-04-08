namespace Fanex.NAP.FS.Library.Service
{
    using Core.BaseClasses;
    using Fanex.BetList.Core.Builder;
    using Fanex.BetList.Core.TicketHelper;
    using Fanex.Data;
    using Fanex.Logging;
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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Data.Repository;

    public class FraudCaseDetailsService : IFraudCaseDetailsService
    {
        public int environment { get; set; }

        public int mailingListId { get; set; }

        public FraudCaseDetailsService(int env, int mailId)
        {
            environment = env;
            mailingListId = mailId;
        }

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
                        Evidence = string.Join(", ", evidence.Select(e => e.EvidenceCode + "-" + e.EvidenceName)),
                        AssociatedAccounts = totalAssociation.ContainsKey(info.CustId) ? totalAssociation[info.CustId] : 0,
                        CurrencyName = info.CurrencyName
                    }).OrderBy(c => c.Member);
            }
        }

        public IEnumerable<CustomerDetails> GetCustomerArbitrageDetails(GetCustomerArbitrageCriteria criteria, IEnumerable<ScoreHDP> lstScoreHDP)
        {
            FillCustomerArbitrageCriteria(criteria, lstScoreHDP);

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            using (var query = db.QueryMultiple(criteria))
            {
                var custInfo = query.Read<CustInfo>();

                IEnumerable<CustomerEvidence> custEvidence;

                if (criteria.FraudCaseId == (int)FraudCaseId.Arbitrage)
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
                        Evidence = string.Join(", ", evidence.Select(e => e.EvidenceCode + "-" + e.EvidenceName)),
                        AssociatedAccounts = totalAssociation.ContainsKey(info.CustId) ? totalAssociation[info.CustId] : 0,
                        CurrencyName = info.CurrencyName
                    }).OrderBy(c => c.Member);
            }
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

                    Logger.Log.Debug($"finish reading db - homeid: {info.HomeId} - awayid: {info.AwayId}");

                    var home = CachedRef.GetTeamName(info.HomeId);

                    Logger.Log.Debug("read home from refdata");

                    var away = CachedRef.GetTeamName(info.AwayId);

                    Logger.Log.Debug("read away from refdata");

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

        private IEnumerable<CustomerEvidence> GetEvidences(IEnumerable<int> custIds)
        {
            var url = AppSettings.Get("FPS_WEBAPI_GET_CUSTOMER_EVIDENCE");

            var tsk = custIds.Distinct().Chunk(500).Select(a => new CustomerEvidenceList
            {
                CustIdList = string.Join(",", custIds)
            }).Select(p => ApiHelper.Post<IEnumerable<CustomerEvidence>>(url, p)).ToArray();

            Task.WaitAll(tsk);

            return tsk.Where(t => t.Result != null).SelectMany(t => t.Result);
        }

        public IEnumerable<ArbitrageGroupTicketList> GetArbitrageTickets(int matchId, DateTime eventDate, bool isShowSpecialCurrencies)
        {
            var rawData = DataService.GetData<TicketInfo>(new GetArbitrageTicketCriteria { MatchId = matchId, EventDate = eventDate, IsShowSpecialCurrencies = isShowSpecialCurrencies });

            var url = AppSettings.Get("FPS_WEBAPI_GET_CUSTOMER_EVIDENCE");

            var tsks =
                rawData.Select(t => t.CustId).Distinct()
                .Chunk(500)
                .Select(q => ApiHelper.Post<IEnumerable<CustomerEvidence>>(url, new CustomerEvidenceList { CustIdList = string.Join(",", q) }))
                .ToArray();

            Task.WaitAll(tsks);

            var evidences = tsks.SelectMany(t => t.Result);

            var evidenceDict = evidences.ToLookup(c => c.CustId, c => c.EvidenceName);

            var data =
                rawData
                .AsParallel()
                .GroupBy(t => Tuple.Create(t.BetTypeId, t.Handicap1 - t.Handicap2, t.LiveHomeScore, t.LiveAwayScore, t.GroupId), t => new ArbitrageTicket
                {
                    BetId = t.TransId,
                    Currency = t.CurrencyName,
                    DangerousLevel = t.DangerLevel,
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
                    BettypeName = CachedRef.GetBettypeName(t.BetTypeId),
                    Handicap = (double)(t.Handicap1 - t.Handicap2),
                    Score = $"{t.LiveHomeScore} - {t.LiveAwayScore}"
                })
                .OrderBy(g => GetOrder(CachedRef.GetBettypeName(g.Key.Item1)))
                .Select(g =>
                {
                    var tmp = g.OrderBy(t => t.RawTransDate).ThenBy(t => t.Username).ToList();

                    return new ArbitrageGroupTicketList
                    {
                        BettypeName = CachedRef.GetBettypeName(g.Key.Item1),
                        Away = tmp.Where(t => t.BetTeam == "a"),
                        Home = tmp.Where(t => t.BetTeam == "h"),
                        Handicap = g.Key.Item2,
                        Score = $"{g.Key.Item3} - {g.Key.Item4}",
                        GroupId = g.Key.Item5
                    };
                }).ToList();

            return data;
        }

        public IEnumerable<ArbitrageGroupTicketList> GetArbitrageTickets(GetArbitrageTicketCriteria criteria, IEnumerable<ScoreHDP> lstScoreHDP)
        {
            FillArbitrageTicketCriteria(criteria, lstScoreHDP);
                     
            var rawData = DataService.GetData<TicketInfo>(criteria);

            var url = AppSettings.Get("FPS_WEBAPI_GET_CUSTOMER_EVIDENCE");

            var tsks =
                rawData.Select(t => t.CustId).Distinct()
                .Chunk(500)
                .Select(q => ApiHelper.Post<IEnumerable<CustomerEvidence>>(url, new CustomerEvidenceList { CustIdList = string.Join(",", q), EvidenceCodeList = criteria.EvidenceCodeList }))
                .ToArray();

            Task.WaitAll(tsks);
            
            criteria.TotalTicket = tsks.Count();

            var evidences = tsks.SelectMany(t => t.Result);

            var evidenceDict = evidences.ToLookup(c => c.CustId, c => c.EvidenceName);

            var data =
                rawData
                .AsParallel()
                .GroupBy(t => Tuple.Create(t.BetTypeId, t.Handicap1 - t.Handicap2, t.LiveHomeScore, t.LiveAwayScore, t.GroupId), t => new ArbitrageTicket
                {
                    BetId = t.TransId,
                    Currency = t.CurrencyName,
                    DangerousLevel = t.DangerLevel,
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
                    BettypeName = CachedRef.GetBettypeName(t.BetTypeId),
                    Handicap = (double)(t.Handicap1 - t.Handicap2),
                    Score = $"{t.LiveHomeScore} - {t.LiveAwayScore}"
                })
                .OrderBy(g => GetOrder(CachedRef.GetBettypeName(g.Key.Item1)))
                .Select(g =>
                {
                    var tmp = g.OrderBy(t => t.RawTransDate).ThenBy(t => t.Username).ToList();

                    return new ArbitrageGroupTicketList
                    {
                        BettypeName = CachedRef.GetBettypeName(g.Key.Item1),
                        Away = tmp.Where(t => t.BetTeam == "a"),
                        Home = tmp.Where(t => t.BetTeam == "h"),
                        Handicap = g.Key.Item2,
                        Score = $"{g.Key.Item3} - {g.Key.Item4}",
                        GroupId = g.Key.Item5
                    };
                }).ToList();

            return data;
        }

        /// <summary>
        /// Get list evidence from 5.1 to 7.1
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Evidence> GetListEvidences()
        {
            List<Evidence> lstEvidences = new List<Evidence>();
            lstEvidences.Add(new Evidence { EvidenceCode = "5.1", EvidenceName = "5.1 - Danger 10" });
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

            return lstEvidences;
        }

        /// <summary>
        /// Get all customer currency
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Currency> GetAllCurrency()
        {
            var criteria = new GetCurrencyCriteria();
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

        public byte[] ExportArbitrageExcel(int matchId, DateTime eventDate, bool isShowSpecialCurrencies, string templatePath)
        {
            var fraudCaseId = (int)FraudCaseId.Arbitrage;

            var legend = GetInfo(matchId, (int)FraudCaseId.Arbitrage, eventDate);

            var tickets = GetArbitrageTickets(matchId, eventDate, isShowSpecialCurrencies).SelectMany(obj => obj.Home.Concat(obj.Away));
            var customers = Enumerable.Empty<CustomerDetails>();

            var wb =
                ExcelUtilities.GetWorkbook(templatePath)
                    .RenderSheet(new ArbitrageConfig(tickets.Count()), legend, tickets, null)
                    .RenderSheet(new CustomerDetailConfig(0), legend, customers, null);

            return wb.ToBytes();
        }

        public byte[] ExportArbitrageExcel(int matchId, DateTime eventDate, bool isShowSpecialCurrencies, string templatePath, Func<ArbitrageGroupTicketList, bool> filter)
        {
            var legend = GetInfo(matchId, (int)FraudCaseId.Arbitrage, eventDate);

            var tickets = GetArbitrageTickets(matchId, eventDate, isShowSpecialCurrencies).Where(filter).SelectMany(obj => obj.Home.Concat(obj.Away));

            var wb =
                ExcelUtilities.GetWorkbook(templatePath)
                    .RenderSheet(new ArbitrageConfig(tickets.Count(), true), legend, tickets, null);

            return wb.ToBytes();
        }

        public byte[] ExportArbitrageExcel(GetArbitrageTicketCriteria criteria, IEnumerable<ScoreHDP> lstScoreHDP, string templatePath)
        {
            var tickets = GetArbitrageTickets(criteria, lstScoreHDP).SelectMany(obj => obj.Home.Concat(obj.Away));
            var legend = GetInfo(criteria.MatchId, (int)FraudCaseId.Arbitrage, criteria.EventDate);
            var customers = Enumerable.Empty<CustomerDetails>();

            var wb =
                ExcelUtilities.GetWorkbook(templatePath)
                    .RenderSheet(new ArbitrageConfig(tickets.Count()), legend, tickets, null)
                    .RenderSheet(new CustomerDetailConfig(0), legend, customers, null);

            return wb.ToBytes();
        }

        public string ExportArbitrageCsv(int matchId, DateTime eventDate, bool isShowSpecialCurrencies)
        {
            var tickets = GetArbitrageTickets(matchId, eventDate, isShowSpecialCurrencies).SelectMany(obj => obj.Home.Concat(obj.Away));
            var info = GetInfo(matchId, (int)FraudCaseId.Arbitrage, eventDate);

            var txt = new StringBuilder();

            txt.AppendLine($"match: {info.Home} - vs - {info.Away}, date: {info.EventDate.ToString("MMM/dd/yyyy")}, HT/FT: {info.HTScore}/{info.FTScore}");
            txt.AppendLine("member,evidence,dangerous level,currency,bet id, bet team, bet type, hdp, score, odds, v.stake, Com.Stake, Full. Stake, TransDate");

            foreach (var item in tickets)
            {
                txt.AppendLine($"{item.Username},{item.Evidences.Join(" /")},{item.DangerousLevel}," +
                    $"{item.Currency},{item.BetId},{item.BetTeam},{item.BettypeName},{item.Handicap},'{item.Score}," +
                    $"{item.Odds},{item.Stake.Virtual},{item.Stake.ComPT}," +
                    $"{item.Stake.Full},{item.TransDate.Replace(",", " -").Replace("<br/>", " ")}");
            }

            return txt.ToString();
        }

        public string ExportArbitrageCsvFromList(int matchId, DateTime eventDate, IEnumerable<ArbitrageGroupTicketList> lstArbitrageTicket)
        {
            var txt = new StringBuilder();
            var info = GetInfo(matchId, (int)FraudCaseId.Arbitrage, eventDate);

            txt.AppendLine($"match: {info.Home} - vs - {info.Away}, date: {info.EventDate.ToString("MMM/dd/yyyy")}, HT/FT: {info.HTScore}/{info.FTScore}");
            txt.AppendLine("member,evidence,dangerous level,currency,bet id, bet team, bet type, hdp, score, odds, v.stake, Com.Stake, Full. Stake, TransDate");

            foreach (var item in lstArbitrageTicket.SelectMany(obj => obj.Home.Concat(obj.Away)))
            {
                txt.AppendLine($"{item.Username},{item.Evidences.Join(" /")},{item.DangerousLevel}," +
                       $"{item.Currency},{item.BetId},{item.BetTeam},{item.BettypeName},{item.Handicap},'{item.Score}," +
                       $"{item.Odds},{item.Stake.Virtual},{item.Stake.ComPT}," +
                       $"{item.Stake.Full},{item.TransDate.Replace(",", " -").Replace("<br/>", " ")}");

            }
            return txt.ToString();
        }

        private void FillCustomerArbitrageCriteria(GetCustomerArbitrageCriteria criteria, IEnumerable<ScoreHDP> lstScoreHDP)
        {
            if (criteria != null && criteria.Bettype == -1)
            {
                criteria.Handicap = -1;
                criteria.TotalScores = -1;
            }
            else
            {
                if (lstScoreHDP != null)
                {
                    var objValue = lstScoreHDP.FirstOrDefault(c => c.BetType == criteria.Bettype);
                    if (objValue != null)
                    {
                        criteria.Handicap = objValue.Handicap;
                        criteria.TotalScores = objValue.TotalScore;
                    }
                }
            }
        }

        private void FillArbitrageTicketCriteria(GetArbitrageTicketCriteria criteria, IEnumerable<ScoreHDP> lstScoreHDP)
        {
            if (criteria != null && criteria.Bettype == -1)
            {
                criteria.Handicap = -1;
                criteria.TotalScores = -1;
            }
            else
            {
                if (lstScoreHDP != null)
                {
                    var objValue = lstScoreHDP.FirstOrDefault(c => c.BetType == criteria.Bettype);
                    if (objValue != null)
                    {
                        criteria.Handicap = objValue.Handicap;
                        criteria.TotalScores = objValue.TotalScore;
                    }
                }
            }
        }
    }
}