namespace Fanex.NAP.FS.Library.ExcelConfig.CustomerMonitor
{
    using Fanex.NAP.FS.Library.Core.ExportExcel;
    using Fanex.NAP.FS.Library.Core.Helpers;
    using Fanex.NAP.FS.Library.ViewModels.CustomerMonitor.CustomerDetails;
    using NPOI.SS.UserModel;
    using NPOI.XSSF.UserModel;
    using System;
    using System.Text;

    public class CustomerMonitorTicketDetailConfig : FSExcelConfig<CustomerDetailsInfo, TicketDetails, object>
    {
        public CustomerMonitorTicketDetailConfig(int ticketNo)
        {
            SheetName = "report";
        }

        protected override int SheetIndex => 0;

        protected override string SheetName { get; }

        protected override void FillData(IRow row, TicketDetails data)
        {
            var ticket = data;
            var cell_idx = 0;

            row.GetCell(cell_idx++).SetCellValue(row.RowNum - 2);
            row.GetCell(cell_idx++).SetCellValue(ticket.TransDate);
            row.GetCell(cell_idx++).SetCellValue(ticket.BetID);
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

        protected override void FillLegend(IRow row, CustomerDetailsInfo legend)
        {
            var cell1 = row.GetCell(0);

            new XSSFRichTextString()
                .WriteBold("Customer: ").Write(legend.Username + "\n")
                .WriteBold("Fraud Case: ").Write("BNG \n")
                .WriteBold("Date Range: ").Write(legend.FromDate.ToString("MMM dd, yyyy") + " - " + legend.FromDate.ToString("MMM dd, yyyy") + "\n")
                .WriteBold("Tickets: ").Write(legend.TicketCount + "\n")
                .WriteBold("Violations: ").Write(legend.ViolationCount + "\n")
                .WriteBold("Matches: ").Write(legend.MatchCount + "\n")
                .SetValue(cell1);
        }

        private static void GetChoiceExcel(TicketDetails t, ICell cell)
        {
            var bet_team = new StringBuilder(t.ChoiceExcel.betTeam);

            var score = BetListHelper.ExtractStringFromHtml(t.ChoiceExcel.Score);

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

            cell.SetCellValue(string.Format("{0}{1}{2}{3}", bet_team, handicap, score, t.BettypeRealName));
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