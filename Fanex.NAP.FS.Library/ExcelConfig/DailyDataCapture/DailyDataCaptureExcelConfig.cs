#pragma warning disable S1244 // Floating point numbers should not be tested for equality

namespace Fanex.NAP.FS.Library.ExcelConfig.DailyDataCapture
{
    using Fanex.NAP.FS.Library.Core.ExportExcel;
    using Fanex.NAP.FS.Library.Criteria.DailyDataCapture;
    using Fanex.NAP.FS.Library.ViewModels;
    using Fanex.NAP.FS.Library.ViewModels.DailyDataMonitor;
    using NPOI.SS.UserModel;
    using NPOI.XSSF.UserModel;
    using System;

    public class DailyDataCaptureExcelConfig : FSExcelConfig<DailyDataGetCriteria, CapturedMatch, object>
    {
        protected override int SheetIndex => 0;

        protected override string SheetName => null;

        protected override void FillData(IRow row, CapturedMatch data)
        {
            var idx = 0;

            row.GetCell(idx++).SetCellValue($"{data.Home} - vs - {data.Away}");
            row.GetCell(idx++).SetCellValue(DateTime.Parse(data.EventDate).ToString("MMM dd,yyyy hh:mm:ss tt"));
            row.GetCell(idx++).SetCellValue(DateTime.Parse(data.ScannedDate).ToString("MMM dd, yyyy hh:mm:ss tt"));
            row.GetCell(idx++).SetCellValue(RenderDouble(data.CompanyTO));
            row.GetCell(idx++).SetCellValue(RenderDouble(data.SuperTO));
            row.GetCell(idx++).SetCellValue(RenderDouble(data.MasterTO));
            row.GetCell(idx++).SetCellValue(RenderDouble(data.AgentTO));
            row.GetCell(idx++).SetCellValue(RenderInt(data.Tickets));
            row.GetCell(idx++).SetCellValue(RenderInt(data.SettledTickets));
            row.GetCell(idx).SetCellValue(RenderInt(data.CancelledTickets));
        }

        protected override void FillLegend(IRow row, DailyDataGetCriteria legend)
        {
            new XSSFRichTextString()
                .WriteBold("From Date: ")
                .Write($"{legend.FromDate.ToString("MMM dd, yyyy")}\n")
                .WriteBold("To Date: ")
                .Write(legend.ToDate.ToString("MMM dd, yyyy"))
                .SetValue(row.GetCell(0));
        }

        private XSSFRichTextString RenderDouble(History<double> history)
        {
            if (history.Old == history.New)
            {
                return ExcelUtilities.GetXSSF().Write(history.New.ToString("N2"));
            }

            return ExcelUtilities.GetXSSF()
                .Write($"{history.Old.ToString("N2")}\n")
                .WriteWithFont(history.New.ToString("N2"), new XSSFFont { Color = IndexedColors.Coral.Index });
        }

        private XSSFRichTextString RenderInt(History<int> history)
        {
            if (history.Old == history.New)
            {
                return ExcelUtilities.GetXSSF().Write(history.New.ToString("N0"));
            }

            return ExcelUtilities.GetXSSF()
                .Write($"{history.Old.ToString("N0")}\n")
                .WriteWithFont(history.New.ToString("N0"), new XSSFFont { Color = IndexedColors.Coral.Index });
        }
    }
}

#pragma warning restore S1244 // Floating point numbers should not be tested for equality