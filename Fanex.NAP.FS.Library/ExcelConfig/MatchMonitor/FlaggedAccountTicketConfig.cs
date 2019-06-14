namespace Fanex.NAP.FS.Library.ExcelConfig.MatchMonitor
{
    using Fanex.NAP.FS.Library.Core.ExportExcel;
    using Fanex.NAP.FS.Library.Core.Helpers;
    using Fanex.NAP.FS.Library.ViewModels.MatchMonitor.FraudCaseDetails;
    using NPOI.SS.UserModel;
    using NPOI.XSSF.UserModel;
    using System;
    using System.Text;

    public class FlaggedAccountTicketConfig : FSExcelConfig<MatchFraudCaseInfo, FlaggedAccountTicket, object>
    {
        protected override int SheetIndex => 0;

        protected override string SheetName { get; }

        public FlaggedAccountTicketConfig(int ticketNo)
        {
            SheetName = string.Format("By Tickets ({0})", ticketNo);
        }      

        protected override void FillData(IRow row, FlaggedAccountTicket data)
        {
            var ticket = data;
            var cell_idx = 0;

            row.GetCell(cell_idx++).SetCellValue(row.RowNum - 2);
            row.GetCell(cell_idx++).SetCellValue(ticket.Member);
            row.GetCell(cell_idx++).SetCellValue(string.Join(",", ticket.Evidences));
            row.GetCell(cell_idx++).SetCellValue(ticket.DangerousLevel);
            row.GetCell(cell_idx++).SetCellValue(ticket.Site);
            row.GetCell(cell_idx++).SetCellValue(ticket.CurrencyName);
            row.GetCell(cell_idx++).SetCellValue(ticket.TransDate);
            row.GetCell(cell_idx++).SetCellValue(ticket.BetID);
            row.GetCell(cell_idx++).SetCellValue(ticket.BettypeRealName);
            GetChoiceExcel(ticket, row.GetCell(cell_idx++));
            row.GetCell(cell_idx++).SetCellValue(ticket.Score);
            GetOddsExcel(ticket, row.GetCell(cell_idx++));
            row.GetCell(cell_idx++).SetCellValue((double)ticket.Stake);

            if (ticket.WinLoss == null)
            {
                row.GetCell(cell_idx++).SetCellValue("-");
            }
            else
            {
                row.GetCell(cell_idx++).SetCellValue((double)ticket.WinLoss);
            }

            row.GetCell(cell_idx++).SetCellValue(ticket.Status);
            row.GetCell(cell_idx).SetCellValue(ticket.IP);
        }

        protected override void FillLegend(IRow row, MatchFraudCaseInfo legend)
        {
            var cell1 = row.GetCell(0);
            var cell2 = row.GetCell(5);
            var cell3 = row.GetCell(9);
            string[] arrFilter = legend.Filter;

            new XSSFRichTextString()
                .WriteBold("Event: ").Write(string.Format("{0} - vs - {1}\n", legend.Home as string, legend.Away as string))
                .WriteBold("Date: ").Write(legend.EventDate.ToString("MMM dd, yyyy") + "\n")
                .WriteBold("Fraud Case: ").Write(legend.FraudCase + "\n")
                .WriteBold("HT/FT Score: ").Write(legend.HTScore + " / " + legend.FTScore + "\n")
                .SetValue(cell1);

            if (arrFilter != null && arrFilter.Length >= 7)
            {
                new XSSFRichTextString()
                   .WriteBold("Site: ").Write(arrFilter[6] + "\n")
                   .WriteBold("Currency: ").Write(arrFilter[1] + "\n")
                   .WriteBold("Evidence: ").Write(arrFilter[2] + "\n")
                   .SetValue(cell2);

                new XSSFRichTextString()
                    .WriteBold("Market: ").Write(arrFilter[0] + "\n")
                    .WriteBold("Trans Date: ").Write(arrFilter[3] + " - " + arrFilter[4] + "\n")
                    .WriteBold("Bet Type: ").Write(arrFilter[5] + "\n")
                    .SetValue(cell3);
            }
        }

        private static void GetChoiceExcel(FlaggedAccountTicket t, ICell cell)
        {
            var bet_team = new StringBuilder();
            var handicap = new StringBuilder();
            var score = string.Empty;

            if (t.ChoiceExcel != null)
            {
                bet_team.Append(t.ChoiceExcel.betTeam);
                handicap.Append(t.ChoiceExcel.Handicap);
                score = BetListHelper.ExtractStringFromHtml(t.ChoiceExcel.Score);
            }

            bet_team
                .Replace("&nbsp;", "")
                .Replace("<span class=\"underdog\">", "")
                .Replace("</span>", "")
                .Replace("<span class=\"favorite\">", "");

            handicap
                .Replace("<span class='handicap'>", "")
                .Replace("</span>", "")
                .Replace("<span class=\"favorite\">", "");

            cell.SetCellValue(string.Format("{0}{1}{2}", bet_team, handicap, score));
        }

        private static void GetOddsExcel(FlaggedAccountTicket t, ICell cell)
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

            if (arr.Length == 1)
            {
                cell.SetCellValue(arr[0]);
            }
            else if (arr.Length == 2)
            {
                cell.SetCellValue(arr[0] + "(" + arr[1] + ")");
            }
            else
            {
                cell.SetCellValue("");
            }
        }        
    }
}
