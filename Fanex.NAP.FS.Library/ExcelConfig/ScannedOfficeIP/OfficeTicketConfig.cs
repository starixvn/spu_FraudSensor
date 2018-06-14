#pragma warning disable S3881 // "IDisposable" should be implemented correctly

namespace Fanex.NAP.FS.Library.ExcelConfig.ScannedOfficeIP
{
    using Fanex.BetList.Core.Builder;
    using Fanex.BetList.Core.TicketHelper;
    using Fanex.NAP.FS.Library.Core.ExportExcel;
    using Fanex.NAP.FS.Library.Core.Helpers;
    using Fanex.NAP.FS.Library.Criteria.ScannedOfficeIP;
    using Fanex.NAP.FS.Library.Models.MatchMonitor.FraudCaseDetails;
    using NPOI.SS.UserModel;
    using NPOI.XSSF.UserModel;
    using System;

    public class OfficeTicketConfig : FSExcelConfig<TicketFromOfficeIpGetCriteria, TicketInfo, object>, IDisposable
    {
        protected override int SheetIndex => 0;

        protected override string SheetName => null;

        private readonly BetListExcelBuilder excelBuilder = new BetListExcelBuilder();

        private readonly TicketHelper ticketHelper = new TicketHelper();

        protected override void FillData(IRow row, TicketInfo data)
        {
            var cell_idx = 0;

            row.GetCell(cell_idx++).SetCellValue(row.RowNum - 2);
            row.GetCell(cell_idx++).SetCellValue(data.UserName);
            row.GetCell(cell_idx++).SetCellValue(data.CurrencyName);
            row.GetCell(cell_idx++).SetCellValue(data.TransDate.ToString("MMM dd, yyyy hh:mm:ss tt"));
            row.GetCell(cell_idx++).SetCellValue(data.TransId);
            RenderChoice(row.GetCell(cell_idx++), data);
            RenderOdds(row.GetCell(cell_idx++), data);
            row.GetCell(cell_idx++).SetCellValue((double)data.Stake);
            row.GetCell(cell_idx++).SetCellValue((double)data.Winlost);
            row.GetCell(cell_idx++).SetCellValue(data.Status);
            row.GetCell(cell_idx).SetCellValue(data.IP);
        }

        protected override void FillLegend(IRow row, TicketFromOfficeIpGetCriteria legend)
        {
            new XSSFRichTextString()
                .WriteBold("From Date: ")
                .Write($"{legend.FromDate.ToString("MMM dd, yyyy")}\n")
                .WriteBold("To Date: ")
                .Write(legend.ToDate.ToString("MMM dd, yyyy"))
                .SetValue(row.GetCell(0));
        }

        private void RenderChoice(ICell cell, TicketInfo data)
        {
            var template = excelBuilder.CreateChoiceBuilder(data.BetTypeId).Render(data, ticketHelper, null, false);

            var choice = BetListHelper.ExtractStringFromHtml(template.betTeam) + BetListHelper.ExtractStringFromHtml(template.Handicap);

            var score = BetListHelper.ExtractStringFromHtml(template.Score);

            var bettype = BetListHelper.ExtractStringFromHtml(template.BetType);

            var betteam = string.Format("{0} - vs - {1}", template.Match.homeTeam, template.Match.awayTeam);

            var league = BetListHelper.ExtractStringFromHtml(template.League.LeagueName);

            new XSSFRichTextString()
                .WriteFormat("{0}{1}\n", choice, score)
                .Write(bettype + "\n")
                .Write(betteam + "\n")
                .Write(league + "\n")
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

#pragma warning restore S3881 // "IDisposable" should be implemented correctly