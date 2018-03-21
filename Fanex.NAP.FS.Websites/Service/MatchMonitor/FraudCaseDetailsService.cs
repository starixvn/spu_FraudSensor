using Fanex.Data;
using Fanex.NAP.FS.Websites.Criteria.MatchMonitor.FraudCaseDetails;
using Fanex.NAP.FS.Websites.Models;
using Fanex.NAP.FS.Websites.Models.MatchMonitor.FraudCaseDetails;
using Fanex.NAP.FS.Websites.Service.Utilities;
using Fanex.NAP.FS.Websites.ViewModels.MatchMonitor.FraudCaseDetails;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using RemotingRef;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Fanex.NAP.FS.Websites.Service.MatchMonitor
{
    public static class FraudCaseDetailsService
    {
        public static IEnumerable<TicketDetails> GetTicketDetails(int matchId, int fraudCaseId, DateTime eventDate)
        {
            var criteria = new GetTicketCriteria
            {
                EventDate = eventDate,
                FraudCaseId = fraudCaseId,
                MatchId = matchId
            };

            return DataService.GetData<TicketInfo>(criteria)
                .Select(a => new TicketDetails
                {
                    Member = a.Username,
                    Bettype = RefData.GetBetTypeName(a.Bettype.ToString(), "en"),
                    TransDate = a.TransDate,
                    BetID = a.TransId.ToString(),
                    IP = a.IP,
                    Odds = new OddsDetails { Value = a.Odds, Type = RefData.GetOddsTypeName(a.OddsType.ToString(), "en") },
                    Score = string.Format("{0}-{1}", a.LiveHomeScore, a.LiveAwayScore),
                    Stake = a.Stake,
                    Status = a.Status,
                    WinLoss = a.Winlost,
                    Choice = "a"
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
            using(var query = db.QueryMultiple(criteria))
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
                         AssociatedAccounts = 0
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

                WriteTickets(tickets, GetTicketDetails(matchId, fraudCaseId, eventDate), legend);
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

            using(var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using(var query = db.QueryMultiple(criteria))
                {
                    var info = query.ReadSingle();
                    var fraudCaseName = query.ReadSingle<string>();

                    var home = RefData.GetTeamName(info.homeid.ToString())[1];
                    var away = RefData.GetTeamName(info.awayid.ToString())[1];

                    var eventName = string.Format("{0} -vs- {1}", home, away);

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
                        FTScore = ftScore
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
                .WriteBold("Event: ").Write(legend.Event + "\n")
                .WriteBold("Date: ").Write(legend.EventDate.ToString("MMM dd, yyyy") + "\n")
                .WriteBold("Fraud Case: ").Write(legend.FraudCase + "\n")
                .WriteBold("HT/FT Score: ").Write(legend.HTScore + " / " +  legend.FTScore + "\n")
                .SetValue(cell1);
        }

        private static void WriteTicket(ISheet sheet, TicketDetails ticket, int idx)
        {
            sheet.ShiftRows(sheet.LastRowNum, sheet.LastRowNum, 1, true, false);

            var lastRow = sheet.GetRow(sheet.LastRowNum);

            var row = sheet.CreateRow(sheet.LastRowNum - 1);

            Excel.CopyRow(lastRow, row);

            row.GetCell(0).SetCellValue(idx + 1);
            row.GetCell(1).SetCellValue(ticket.Member);
            row.GetCell(2).SetCellValue(ticket.TransDate);
            row.GetCell(3).SetCellValue(ticket.BetID);
            row.GetCell(4).SetCellValue(ticket.Choice);
            row.GetCell(5).SetCellValue(ticket.Bettype);
            row.GetCell(6).SetCellValue(ticket.Score);
            new XSSFRichTextString().Write(ticket.Odds.Value + "\n").Write(ticket.Odds.Type).SetValue(row.GetCell(7));
            row.GetCell(8).SetCellValue((double)ticket.Stake);
            row.GetCell(9).SetCellValue((double)ticket.WinLoss);
            row.GetCell(10).SetCellValue(ticket.Status);
            row.GetCell(11).SetCellValue(ticket.IP);
        }

        private static void WriteCustomer(ISheet sheet, CustomerDetails customer, int idx)
        {
            sheet.ShiftRows(sheet.LastRowNum, sheet.LastRowNum, 1, true, false);

            var lastRow = sheet.GetRow(sheet.LastRowNum);

            var row = sheet.CreateRow(sheet.LastRowNum - 1);

            Excel.CopyRow(lastRow, row);

            row.GetCell(0).SetCellValue(idx + 1);
            row.GetCell(1).SetCellValue(customer.Member);
            row.GetCell(2).SetCellValue(customer.Tickets);
            row.GetCell(3).SetCellValue(customer.TotalStake);
            row.GetCell(4).SetCellValue(customer.TotalWinLoss);
            row.GetCell(5).SetCellValue(customer.Evidence);
            row.GetCell(6).SetCellValue(customer.AssociatedAccounts);
        }

        private static BettypeOrder GetOrder(string bettype)
        {
            if (bettype.Contains("1H"))
            {
                if (bettype.Contains("Handicap"))
                {
                    return BettypeOrder.FHDP;
                }

                if (bettype.Contains("Over/Under"))
                {
                    return BettypeOrder.FOU;
                }
            }

            if (bettype.Contains("Handicap"))
            {
                return BettypeOrder.HDP;
            }

            if (bettype.Contains("Over/Under"))
            {
                return BettypeOrder.OU;
            }

            return BettypeOrder.Other;
        }
    }
}