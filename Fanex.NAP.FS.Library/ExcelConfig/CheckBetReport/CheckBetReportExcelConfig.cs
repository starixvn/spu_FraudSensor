#pragma warning disable S1244 // Floating point numbers should not be tested for equality

namespace Fanex.NAP.FS.Library.ExcelConfig.CheckBetReport
{
    using Fanex.NAP.FS.Library.Core.ExportExcel;
    using Fanex.NAP.FS.Library.Criteria.CheckBetReport;
    using Fanex.NAP.FS.Library.ViewModels;
    using Fanex.NAP.FS.Library.ViewModels.CheckBetReport;
    using NPOI.SS.UserModel;
    using NPOI.XSSF.UserModel;
    using System;

    public class CheckBetReportExcelConfig : FSExcelConfig<CheckBetReportGetCriteria, CheckBetTicket, object>
    {
        protected override int SheetIndex => 0;

        protected override string SheetName => null;

        private bool highLight;

        private int? group = null;

        protected override void FillData(IRow row, CheckBetTicket data)
        {
            var idx = 0;

            row.GetCell(idx++).SetCellValue(data.BetId.ToString());
            row.GetCell(idx++).SetCellValue(DateTime.Parse(data.TransDate));
            row.GetCell(idx++).SetCellValue(data.Currency);
            row.GetCell(idx++).SetCellValue(data.Bettype);
            row.GetCell(idx++).SetCellValue(data.Username);
            row.GetCell(idx++).SetCellValue(RenderDouble(data.Stake));
            row.GetCell(idx++).SetCellValue(RenderString(data.Odds));
            row.GetCell(idx++).SetCellValue(RenderString(data.Choice));
            row.GetCell(idx++).SetCellValue(RenderDouble(data.PoistionTaking));
            row.GetCell(idx++).SetCellValue(RenderString(data.Status));
            row.GetCell(idx++).SetCellValue(data.Creator);
            row.GetCell(idx).SetCellValue(data.Note);
        }

        protected override void FillLegend(IRow row, CheckBetReportGetCriteria legend)
        {
            new XSSFRichTextString()
                .WriteBold("From Date: ")
                .Write($"{legend.FromDate.ToString("MMM dd, yyyy")}\n")
                .WriteBold("To Date: ")
                .Write(legend.ToDate.ToString("MMM dd, yyyy"))
                .SetValue(row.GetCell(0));
        }

        private XSSFRichTextString RenderDouble(History<decimal?> history)
        {
            if (history.Old == history.New)
            {
                return ExcelUtilities.GetXSSF().Write(history.New.HasValue ? history.New.Value.ToString("N2") : "-");
            }

            return ExcelUtilities.GetXSSF()
                .Write($"{(history.Old.HasValue ? history.Old.Value.ToString("N2") : "-")}\n")
                .WriteWithFont(history.New.HasValue ? history.New.Value.ToString("N2") : "-", new XSSFFont { Color = IndexedColors.Coral.Index });
        }

        private XSSFRichTextString RenderString(History<string> history)
        {
            if (history.Old == history.New)
            {
                return ExcelUtilities.GetXSSF().Write(history.New ?? "-");
            }

            return ExcelUtilities.GetXSSF()
                .Write($"{history.Old ?? "-"}\n")
                .WriteWithFont(history.New ?? "-", new XSSFFont { Color = IndexedColors.Coral.Index });
        }

        protected override void CleanupTemplate(ISheet sheet)
        {
            var template1 = sheet.GetRow(sheet.LastRowNum - 1);
            var template2 = sheet.GetRow(sheet.LastRowNum);

            sheet.RemoveRow(template1);
            sheet.RemoveRow(template2);
        }

        protected override int ShiftTemplate(ISheet sheet)
        {
            sheet.ShiftRows(sheet.LastRowNum - 1, sheet.LastRowNum, 1, true, false);

            return sheet.LastRowNum - 2;
        }
    }
}

#pragma warning restore S1244 // Floating point numbers should not be tested for equality