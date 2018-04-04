using NPOI.SS.UserModel;
using System.Collections.Generic;

namespace Fanex.NAP.FS.Library.Core.ExportExcel
{
    public abstract class FSExcelConfig<TLegend, TData, TSummary>
    {
        protected abstract int SheetIndex { get; }

        protected abstract string SheetName { get; }

        protected virtual bool HasSummary { get; }

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

            IRow data_template;
            IRow summary_template;

            if (HasSummary)
            {
                data_template = sheet.GetRow(sheet.LastRowNum - 1);
                summary_template = sheet.GetRow(sheet.LastRowNum);
            }
            else
            {
                data_template = sheet.GetRow(sheet.LastRowNum);
                summary_template = null;
            }

            foreach (var item in data)
            {
                WriteBodyRow(data_template, item);
            }

            if (summary_template != null)
            {
                WriteSummary(summary_template, summary);
                sheet.RemoveRow(summary_template);
            }

            sheet.RemoveRow(data_template);
        }

        protected virtual void WriteBodyRow(IRow template, TData data)
        {
            var sheet = template.Sheet;

            sheet.ShiftRows(template.RowNum, sheet.LastRowNum, 1, true, false);

            var row = sheet.CreateRow(template.RowNum - 1);

            ExcelUtilities.CopyRow(template, row);

            FillData(row, data);
        }

        protected virtual void WriteSummary(IRow template, TSummary summary)
        {
            var row = template;

            FillSummary(row, summary);
        }
    }
}
