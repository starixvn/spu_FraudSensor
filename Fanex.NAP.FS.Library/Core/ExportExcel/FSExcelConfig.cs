namespace Fanex.NAP.FS.Library.Core.ExportExcel
{
    using NPOI.SS.UserModel;
    using NPOI.SS.Util;
    using System.Collections.Generic;
    using System.Linq;

    public abstract class FSExcelConfig<TLegend, TData, TSummary>
    {
        protected abstract int SheetIndex { get; }

        protected abstract string SheetName { get; }

        protected virtual bool HasSummary { get; }

        protected virtual IRow GetTemplateRow(ISheet sheet, TData data) => sheet.GetRow(sheet.LastRowNum);

        protected virtual void ModifyReportName(IRow row)
        {
        }

        protected abstract void FillLegend(IRow row, TLegend legend);

        protected virtual void ModifyHeader(IRow row)
        {
        }

        protected abstract void FillData(IRow row, TData data);

        protected virtual void FillSummary(IRow row, TSummary summary)
        {
        }

        public virtual void RenderExcel(IWorkbook wb, TLegend legend, IEnumerable<TData> data, TSummary summary)
        {
            if (SheetName != null)
            {
                wb.SetSheetName(SheetIndex, SheetName);
            }

            var sheet = wb.GetSheetAt(SheetIndex);

            WriteSheet(sheet, legend, data, summary);
        }

        private void WriteSheet(ISheet sheet, TLegend legend, IEnumerable<TData> data, TSummary summary)
        {
            ModifyReportName(sheet.GetRow(0));

            FillLegend(sheet.GetRow(1), legend);

            ModifyHeader(sheet.GetRow(2));

            var summary_template = HasSummary ? sheet.GetRow(sheet.LastRowNum) : null;

            if (data.Any())
            {
                foreach (var item in data)
                {
                    var data_template = GetTemplateRow(sheet, item);
                    WriteBodyRow(data_template, item);
                }
            }
            else
            {
                var data_template = GetTemplateRow(sheet, default(TData));
                WriteEmpty(data_template);
            }

            if (summary_template != null)
            {
                WriteSummary(summary_template, summary);
                sheet.RemoveRow(summary_template);
            }

            CleanupTemplate(sheet);
        }

        protected virtual void WriteBodyRow(IRow template, TData data)
        {
            var sheet = template.Sheet;

            var newRow = ShiftTemplate(sheet);

            var row = sheet.CreateRow(newRow);

            ExcelUtilities.CopyRow(template, row);

            FillData(row, data);
        }

        protected virtual void WriteSummary(IRow template, TSummary summary)
        {
            var row = template;

            FillSummary(row, summary);
        }

        protected virtual void WriteEmpty(IRow template)
        {
            var sheet = template.Sheet;

            var newRow = ShiftTemplate(sheet);

            var row = sheet.CreateRow(newRow);

            ExcelUtilities.CopyRow(template, row);

            row.Sheet.AddMergedRegion(new CellRangeAddress(row.RowNum, row.RowNum, row.FirstCellNum, row.LastCellNum - 1));

            row.GetCell(0).SetCellValue("There's no data");
        }

        protected virtual void CleanupTemplate(ISheet sheet)
        {
            var template = GetTemplateRow(sheet, default(TData));
            sheet.RemoveRow(template);
        }

        protected virtual int ShiftTemplate(ISheet sheet)
        {
            sheet.ShiftRows(sheet.LastRowNum, sheet.LastRowNum, 1, true, false);

            return sheet.LastRowNum - 1;
        }
    }
}