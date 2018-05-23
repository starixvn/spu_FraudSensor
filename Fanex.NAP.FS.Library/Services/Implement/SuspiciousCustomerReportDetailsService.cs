namespace Fanex.NAP.FS.Library.Services
{
    using BetList.Core.Builder;
    using Core.BaseClasses;
    using Criteria.SuspiciousCustomer;
    using Criteria.SuspiciousInfo;
    using Fanex.BetList.Core.TicketHelper;
    using Fanex.Data;
    using Fanex.NAP.FS.Library.Criteria.SuspiciousCustomerDetails;
    using Fanex.NAP.FS.Library.Models.MatchMonitor.FraudCaseDetails;
    using Fanex.NAP.FS.Library.Models.SuspiciousCustomerDetails;
    using Fanex.NAP.FS.Library.ViewModels.SuspiciousCustomerDetails;
    using Models;
    using Models.SuspiciousCustomer;
    using Models.SuspiciousInfo;
    using RemotingRef;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class SuspiciousCustomerReportDetailsService : ISuspiciousCustomerReportDetailsService
    {
        #region Customer Info

        public string GetAssignee(int custId)
        {
            return DataService.GetSingle<string>(new SuspiciousCustomerAssigneeGetCriteria(custId));
        }

        public DBResult UpdateAssignee(int custId, int? assignee, int userId)
        {
            var criteria = new SuspiciousCustomerAssigneeUpdateCriteria
            {
                CustId = custId,
                AssigneeId = assignee,
                UserId = userId
            };

            return DataService.UpdateData(criteria, "Update assignee");
        }

        public SuspiciousCustomerModel GetSuspiciousCustomerInfo(int custId)
        {
            var criteria = new SuspiciousCustomerGetCriteria
            {
                CustId = custId,
                IsDelinquent = null,
                IsDirty = null,
                IsHedge = null
            };

            return DataService.GetData<SuspiciousCustomerModel>(criteria).First();
        }

        public IEnumerable<SuspiciousCustomerFraudCaseModel> GetSuspiciousCustomerFraudCase(int custId)
        {
            return DataService.GetData<SuspiciousCustomerFraudCaseModel>(new SuspiciousInfoGetCriteria(custId));
        }

        public DBResult VerifySuspiciousCustomerFraudCase(int custId, int userId, int fraudCaseId)
        {
            var criteria = new SuspiciousInfoUpdateCriteria
            {
                CustId = custId,
                UserId = userId,
                FraudCaseId = fraudCaseId,
                IsVerified = true
            };

            return DataService.UpdateData(criteria, "Verify Suspicious Customer - Fraud Case");
        }

        public DBResult InsertCustomerLogNotes(int custId, int userId, string logDesc)
        {
            var criteria = new SuspiciousCustomerNoteInsertCriteria
            {
                CustId = custId,
                UserId = userId,
                CustLogType = 1, //note
                CaseId = null,
                LogDesc = logDesc
            };

            return DataService.UpdateData(criteria, "Insert Customer Note");
        }

        public IEnumerable<SuspiciousCustomerNoteModel> GetCustomerLogNotes(int custId)
        {
            return DataService.GetData<SuspiciousCustomerNoteModel>(new SuspiciousCustomerLogNoteGetCriteria(custId));
        }

        public DBResult DeleteCustomerLogNotes(int userId, int customerLogId)
        {
            var criteria = new SuspiciousCustomerNoteDeleteCriteria
            {
                UserId = userId,
                CustomerLogId = customerLogId
            };

            return DataService.UpdateData(criteria, "Delete Customer Note");
        }

        public DBResult UpdateSuspiciousCustomerInfo(int custId, int userId, bool? isDelinquent, bool? isDirty, bool? isHedge)
        {
            var criteria = new SuspiciousCustomerUpdateCriteria
            {
                CustId = custId,
                UserId = userId,
                IsDelinquent = isDelinquent,
                IsDirty = isDirty,
                IsHedge = isHedge
            };

            return DataService.UpdateData(criteria, "Update Suspicious Customer Info");
        }

        #endregion Customer Info

        #region Statistics

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
                var summaries = queries.Read<Summary>();

                var total = summaries.Aggregate(new Summary { RptMonth = -1 }, (acc, a) => acc + a);

                return new Statistics
                {
                    Summaries = summaries.Concat(new[] { total }),
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
                    .Where(f => !string.IsNullOrEmpty(f.TransIDs))
                    .OrderBy(f => f.FraudCaseName);

                var tickets =
                    queries.Read<TicketInfo>()
                    .ToDictionary(t => t.TransId);

                var null_winloss = new HashSet<string>
                {
                    "running",
                    "reject"
                };

                foreach (var fc in fraudCases)
                {
                    var source = fc.Ids.Where(tickets.ContainsKey).Select(id => tickets[id]);

                    if (source.Any())
                    {
                        yield return new { key = fc.FraudCaseName };

                        foreach (var t in source.OrderBy(t => t.TransDate))
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
                                WinLoss = null_winloss.Contains(t.Status.ToLowerInvariant()) ? null : new decimal?(t.Winlost),
                            };

                            yield return new { key = fc.FraudCaseName, value = obj };
                        }
                    }
                }
            }
        }

        #endregion Statistics
    }
}