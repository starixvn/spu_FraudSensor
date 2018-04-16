namespace Fanex.NAP.FS.Library.Core.Utilities
{
    using NPOI.SS.UserModel;
    using NPOI.XSSF.UserModel;

    public static class Excel
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

        public static void SetValue(this XSSFRichTextString value, ICell target)
        {
            target.SetCellValue(value);
        }
    }
}
