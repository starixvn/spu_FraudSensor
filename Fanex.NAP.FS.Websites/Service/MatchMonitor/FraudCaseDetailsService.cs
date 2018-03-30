using Fanex.BetList.Core.Builder;
using Fanex.BetList.Core.Entities;
using Fanex.BetList.Core.TicketHelper;
using Fanex.Data;
using Fanex.NAP.FS.Websites.Criteria.MatchMonitor.FraudCaseDetails;
using Fanex.NAP.FS.Websites.Models;
using Fanex.NAP.FS.Websites.Models.MatchMonitor.FraudCaseDetails;
using Fanex.NAP.FS.Websites.Service.Utilities;
using Fanex.NAP.FS.Websites.ViewModels.MatchMonitor.FraudCaseDetails;
using HtmlAgilityPack;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using RemotingRef;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml.Linq;

namespace Fanex.NAP.FS.Websites.Service.MatchMonitor
{
    public static class FraudCaseDetailsService
    {
        public static IEnumerable<TicketDetails> GetTicketDetails(int matchId, int fraudCaseId, DateTime eventDate, bool saveChoice = false)
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
                .Select(a => {
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

        public static IEnumerable<CustomerDetails> GetCustomerDetails(int matchId, int fraudCaseId, DateTime eventDate)
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
                var custEvidence = query.Read<CustEvidence>();

                return custInfo.GroupJoin(
                    custEvidence,
                    info => info.CustId,
                    evid => evid.CustId,
                    (info, evidence) => new CustomerDetails
                    {
                        Member = info.Username,
                        TotalWinLoss = (double)info.TotalWinlost,
                        TotalStake = (double)info.TotalStake,
                        Tickets = info.NoOfTickets,
                        Evidence = string.Join(", ", evidence.Select(e => e.EvidenceCode + "-" + e.EvidenceName)),
                        AssociatedAccounts = 0,
                        CurrencyName = info.CurrencyName
                    }).OrderBy(c => c.Member);
            }
        }

        public static byte[] ExportExcel(int matchId, int fraudCaseId, DateTime eventDate, string templatePath)
        {
            using (var file = File.Open(templatePath, FileMode.Open))
            {
                var wb = new XSSFWorkbook(file);

                var tickets = wb.GetSheetAt(0);
                var customers = wb.GetSheetAt(1);

                var legend = GetInfo(matchId, fraudCaseId, eventDate);

                WriteTickets(tickets, GetTicketDetails(matchId, fraudCaseId, eventDate, true), legend);
                WriteCustomers(customers, GetCustomerDetails(matchId, fraudCaseId, eventDate), legend);

                using (var mem = new NpoiStream())
                {
                    wb.Write(mem);
                    mem.Position = 0;

                    mem.AllowClose = true;

                    return mem.ToArray();
                }
            }
        }

        public static MatchFraudCaseInfo GetInfo(int matchId, int fraudCaseId, DateTime eventDate)
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

        private static void WriteTickets(ISheet sheet, IEnumerable<TicketDetails> data, MatchFraudCaseInfo legend)
        {
            var idx = 0;

            WriteLegend(sheet, legend);

            foreach (var item in data)
            {
                WriteTicket(sheet, item, idx++);
            }

            sheet.RemoveRow(sheet.GetRow(sheet.LastRowNum));
            sheet.Workbook.SetSheetName(0, string.Format("By Tickets ({0})", data.Count()));
        }

        private static void WriteCustomers(ISheet sheet, IEnumerable<CustomerDetails> data, MatchFraudCaseInfo legend)
        {
            var idx = 0;

            WriteLegend(sheet, legend);

            foreach (var item in data)
            {
                WriteCustomer(sheet, item, idx++);
            }

            sheet.RemoveRow(sheet.GetRow(sheet.LastRowNum));
            sheet.Workbook.SetSheetName(1, string.Format("By Customers ({0})", data.Count()));
        }

        private static void WriteLegend(ISheet sheet, MatchFraudCaseInfo legend)
        {
            var row = sheet.GetRow(1);

            var cell1 = row.GetCell(0);

            new XSSFRichTextString()
                .WriteBold("Event: ").Write(string.Format("{0} - vs - {1}\n", legend.Home as string, legend.Away as string))
                .WriteBold("Date: ").Write(legend.EventDate.ToString("MMM dd, yyyy") + "\n")
                .WriteBold("Fraud Case: ").Write(legend.FraudCase + "\n")
                .WriteBold("HT/FT Score: ").Write(legend.HTScore + " / " + legend.FTScore + "\n")
                .SetValue(cell1);
        }

        private static void WriteTicket(ISheet sheet, TicketDetails ticket, int idx)
        {
            sheet.ShiftRows(sheet.LastRowNum, sheet.LastRowNum, 1, true, false);

            var lastRow = sheet.GetRow(sheet.LastRowNum);

            var row = sheet.CreateRow(sheet.LastRowNum - 1);

            Excel.CopyRow(lastRow, row);

            var cell_idx = 0;

            row.GetCell(cell_idx++).SetCellValue(idx + 1);
            row.GetCell(cell_idx++).SetCellValue(ticket.Member);
            row.GetCell(cell_idx++).SetCellValue(ticket.CurrencyName);
            row.GetCell(cell_idx++).SetCellValue(ticket.TransDate);
            row.GetCell(cell_idx++).SetCellValue(ticket.BetID);
            row.GetCell(cell_idx++).SetCellValue(ticket.BettypeRealName);
            GetChoiceExcel(ticket, row.GetCell(cell_idx++));
            row.GetCell(cell_idx++).SetCellValue(ticket.Score);
            GetOddsExcel(ticket, row.GetCell(cell_idx++));
            row.GetCell(cell_idx++).SetCellValue((double)ticket.Stake);
            
            if(ticket.WinLoss == null)
            {
                row.GetCell(cell_idx++).SetCellValue("-");
            }
            else
            {
                row.GetCell(cell_idx++).SetCellValue((double)ticket.WinLoss);
            }

            row.GetCell(cell_idx++).SetCellValue(ticket.Status);
#pragma warning disable S1854 // Dead stores should be removed
            row.GetCell(cell_idx++).SetCellValue(ticket.IP);
#pragma warning restore S1854 // Dead stores should be removed
        }

        private static void WriteCustomer(ISheet sheet, CustomerDetails customer, int idx)
        {
            sheet.ShiftRows(sheet.LastRowNum, sheet.LastRowNum, 1, true, false);

            var lastRow = sheet.GetRow(sheet.LastRowNum);

            var row = sheet.CreateRow(sheet.LastRowNum - 1);

            Excel.CopyRow(lastRow, row);

            var cell_idx = 0;

            row.GetCell(cell_idx++).SetCellValue(idx + 1);
            row.GetCell(cell_idx++).SetCellValue(customer.Member);
            row.GetCell(cell_idx++).SetCellValue(customer.CurrencyName);
            row.GetCell(cell_idx++).SetCellValue(customer.Tickets);
            row.GetCell(cell_idx++).SetCellValue(customer.TotalStake);
            row.GetCell(cell_idx++).SetCellValue(customer.TotalWinLoss);
            row.GetCell(cell_idx++).SetCellValue(customer.Evidence);
#pragma warning disable S1854 // ok here
            row.GetCell(cell_idx++).SetCellValue(customer.AssociatedAccounts);
#pragma warning restore S1854 // ok here
        }

        private static void GetChoiceExcel(TicketDetails t, ICell cell)
        {
            var bet_team = new StringBuilder(t.ChoiceExcel.betTeam);

            bet_team
                .Replace("&nbsp;", "")
                .Replace("<span class=\"underdog\">", "")
                .Replace("</span>", "")
                .Replace("<span class=\"favorite\">", "");

            var handicap = new StringBuilder(t.ChoiceExcel.Handicap);

            handicap
                .Replace("<span class='handicap'>", "")
                .Replace("</span>", "")
                .Replace("<span class=\"favorite\">", "");

            cell.SetCellValue(string.Format("{0}{1}", bet_team, handicap));
        }

        private static void GetOddsExcel(TicketDetails t, ICell cell)
        {
            var splitter = new[] 
            {
                "<span class=\"underdog\">",
                "</span>",
                "<br />",
                "<span class=\"oddstype\">",
                "<font color='#B50000'>",
                "</font>"
            };

            var arr = t.Odds.Html.Split(splitter, StringSplitOptions.RemoveEmptyEntries);

            cell.SetCellValue(string.Format("{0} ({1})", arr.Length == 0 ? "" : arr[0], arr.Length == 1 ? "" : arr[1]));
        }

        private static BettypeOrder GetOrder(string bettype)
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
    }
}