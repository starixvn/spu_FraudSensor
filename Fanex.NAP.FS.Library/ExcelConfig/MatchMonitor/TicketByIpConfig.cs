#pragma warning disable S1854 // Dead stores should be removed

namespace Fanex.NAP.FS.Library.ExcelConfig.MatchMonitor
{
    using Fanex.BetList.Core.Builder;
    using Fanex.BetList.Core.TicketHelper;
    using Fanex.NAP.FS.Library.Core.ExportExcel;
    using Fanex.NAP.FS.Library.Core.Helpers;
    using Fanex.NAP.FS.Library.Models.MatchMonitor.FraudCaseDetails;
    using NPOI.SS.UserModel;
    using NPOI.XSSF.UserModel;
    using System;

    public sealed class TicketByIpConfig : FSExcelConfig<object, TicketInfo, object>, IDisposable
    {
        protected override int SheetIndex => 0;

        protected override string SheetName => null;

        private readonly BetListExcelBuilder excelBuilder = new BetListExcelBuilder();

        private readonly TicketHelper ticketHelper = new TicketHelper();

        private readonly string ip;

        private readonly DateTime buDate;

        public TicketByIpConfig(string ip, DateTime buDate)
        {
            this.ip = ip;
            this.buDate = buDate;
        }

        protected override void ModifyReportName(IRow row)
        {
            var value = string.Format("Tickets of IP: {0} from {1}", ip, buDate.ToString("MMM dd, yyyy"));

            row.Cells[0].SetCellValue(value);
        }

        protected override void FillData(IRow row, TicketInfo data)
        {
            var cell_idx = 0;

            row.GetCell(cell_idx++).SetCellValue(row.RowNum - 2);
            row.GetCell(cell_idx++).SetCellValue(data.UserName);
            row.GetCell(cell_idx++).SetCellValue(data.CurrencyName);
            row.GetCell(cell_idx++).SetCellValue(data.TransDate.ToString("MMM dd, yyyy hh:mm:ss tt"));
            row.GetCell(cell_idx++).SetCellValue(data.TransId);
            RenderChoice(row.GetCell(cell_idx++), data);
            RenderResult(row.GetCell(cell_idx++), data);
            RenderOdds(row.GetCell(cell_idx++), data);
            row.GetCell(cell_idx++).SetCellValue((double)data.Stake);
            row.GetCell(cell_idx++).SetCellValue((double)data.Winlost);
            row.GetCell(cell_idx++).SetCellValue(data.Status);
        }

        protected override void FillLegend(IRow row, object legend)
        {
        }

        private void RenderChoice(ICell cell, TicketInfo data)
        {
            var template = excelBuilder.CreateChoiceBuilder(data.BetTypeId).Render(data, ticketHelper, null, false);

            var choice = template.betTeam.Replace("&nbsp;", "") + BetListHelper.ExtractStringFromHtml(template.Handicap);

            var bettype = BetListHelper.ExtractStringFromHtml(template.BetType);

            var betteam = string.Format("{0} - vs - {1}", template.Match.homeTeam, template.Match.awayTeam);

            var league = BetListHelper.ExtractStringFromHtml(template.League.LeagueName);

            new XSSFRichTextString()
                .Write(choice + "\n")
                .Write(bettype + "\n")
                .Write(betteam + "\n")
                .Write(league + "\n")
                .SetValue(cell);
        }

        private static void RenderResult(ICell cell, TicketInfo data)
        {
            new XSSFRichTextString()
                .WriteFormat("HT: {0} - {1}\n", data.HTHomeScore, data.HTAwayScore)
                .WriteBoldFormat("FT: {0} - {1}", data.HTHomeScore, data.FinalAwayScore)
                .SetValue(cell);
        }

        private void RenderOdds(ICell cell, TicketInfo data)
        {
            var template = excelBuilder.CreateOddsBuilder(data.BetTypeId).Render(data, null, CacheRef.GetOddsTypeName);

            var odds = template.odds.Contains(">") ? BetListHelper.ExtractStringFromHtml(template.odds) : template.odds;

            var oddsType = template.oddsType;

            cell.SetCellValue(string.IsNullOrEmpty(oddsType) ? odds : string.Format("{0}({1})", odds, oddsType));
        }

        public void Dispose()
        {
            excelBuilder.Dispose();
        }
    }
}

#pragma warning restore S1854 // Dead stores should be removed