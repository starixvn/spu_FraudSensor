namespace Fanex.NAP.FS.Library.Services.Implement
{
    using Fanex.BetList.Core.Builder;
    using Fanex.BetList.Core.TicketHelper;
    using Fanex.NAP.FS.Library.Core.BaseClasses;
    using Fanex.NAP.FS.Library.Criteria.ScannedOfficeIP;
    using Fanex.NAP.FS.Library.Models.MatchMonitor.FraudCaseDetails;
    using Fanex.NAP.FS.Library.Services.Interface;
    using Fanex.NAP.FS.Library.ViewModels.MatchMonitor.FraudCaseDetails;
    using RemotingRef;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class ScannedOfficeIpReport : IScannedOfficeIpReportService
    {
        public IEnumerable<TicketDetails> GetTickets(DateTime fromDate, DateTime toDate, bool isShowSpecialCurrencies)
        {
            var html_builder = new BetListHTMLBuilder();
            var ticketHelper = new TicketHelper();

            var null_winloss = new HashSet<string>
            {
                "running",
                "reject"
            };

            var data = GetRawTickets(fromDate, toDate, isShowSpecialCurrencies);

            return data.Select(a =>
                    {
                        var bettype = CachedRef.GetBettypeName(a.BetTypeId);

                        return new TicketDetails
                        {
                            Member = a.UserName,
                            TransDate = a.TransDate.ToString("MMM dd, yyyy hh:mm:ss tt"),
                            BetID = a.TransId != 0 ? a.TransId.ToString() : "-",
                            IP = a.IP,
                            Odds = new OddsDetails { Value = (double)a.Odds, Type = CachedRef.GetOddsTypeName(a.OddsType), Html = html_builder.BuildOdds(a, null, ticketHelper.GetOddsTypeNameById) },
                            Score = string.Format("{0}-{1}", a.LiveHomeScore, a.LiveAwayScore),
                            Stake = a.Stake,
                            Status = a.Status,
                            WinLoss = a.TransId != 0 ? null_winloss.Contains(a.Status.ToLowerInvariant()) ? null : new decimal?(a.Winlost) : null,
                            Choice = a.TransId != 0 ? html_builder.BuildChoice(a, ticketHelper, null, false) : "Login Transaction",
                            ChoiceExcel = null,
                            BettypeRealName = bettype,
                            CurrencyName = a.CurrencyName,
                            RawData = null
                        };
                    });
        }

        public IEnumerable<TicketInfo> GetRawTickets(DateTime fromDate, DateTime toDate, bool isShowSpecialCurrencies)
        {
            var criteria = new TicketFromOfficeIpGetCriteria
            {
                FromDate = fromDate,
                ToDate = toDate,
                IsShowSpecialCurrencies = isShowSpecialCurrencies
            };

            return DataService.GetData<TicketInfo>(criteria)
                .OrderBy(t => t.UserName)
                .ThenBy(t => t.TransDate);
        }
    }
}