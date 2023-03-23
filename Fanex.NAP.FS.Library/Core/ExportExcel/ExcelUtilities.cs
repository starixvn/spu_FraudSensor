namespace Fanex.NAP.FS.Library.Core.ExportExcel
{
    using NPOI.SS.UserModel;
    using NPOI.XSSF.UserModel;
    using System.Collections.Generic;
    using System.IO;

    public static class ExcelUtilities
    {
        public static void CopyRow(IRow source, IRow target)
        {
            foreach (var cell in source.Cells)
            {
                var targetCell = target.GetCell(cell.ColumnIndex) ?? target.CreateCell(cell.ColumnIndex);
                targetCell.CellStyle = cell.CellStyle;
            }

            target.Height = source.Height;
        }

        public static XSSFRichTextString WriteBold(this XSSFRichTextString target, string value)
        {
            target.Append(value, new XSSFFont { IsBold = true });

            return target;
        }

        public static XSSFRichTextString Write(this XSSFRichTextString target, string value)
        {
            target.Append(value);

            return target;
        }

        public static XSSFRichTextString WriteLegend(this XSSFRichTextString target, IDictionary<string, string> pairs)
        {
            foreach (var item in pairs)
            {
                target.WriteBold(item.Key + ": ");
                target.Write(item.Value + "\n");
            }

            return target;
        }

        public static XSSFRichTextString WriteFormat(this XSSFRichTextString target, string format, params object[] args)
        {
            target.Append(string.Format(format, args));

            return target;
        }

        public static XSSFRichTextString WriteBoldFormat(this XSSFRichTextString target, string format, params object[] args)
        {
            target.Append(string.Format(format, args), new XSSFFont { IsBold = true });

            return target;
        }

        public static XSSFRichTextString WriteWithFont(this XSSFRichTextString target, string value, XSSFFont font)
        {
            target.Append(value, font);
            return target;
        }

        public static XSSFRichTextString GetXSSF() => new XSSFRichTextString();

        public static void SetValue(this XSSFRichTextString value, ICell target)
        {
            target.SetCellValue(value);
        }

        public static IWorkbook GetWorkbook(string template)
        {
            using (var template_file = File.OpenRead(template))
            {
                return new XSSFWorkbook(template_file);
            }
        }

        public static byte[] ToBytes(this IWorkbook wb)
        {
            using (var file = new ExcelStream())
            {
                wb.Write(file, false);

                file.AllowClose = true;
                file.Position = 0;

                return file.ToArray();
            }
        }

        public static IWorkbook RenderSheet<TLegend, TData, TSummary>(this IWorkbook wb, FSExcelConfig<TLegend, TData, TSummary> config, TLegend legend, IEnumerable<TData> data, TSummary summary)
        {
            config.RenderExcel(wb, legend, data, summary);

            return wb;
        }
    }
}