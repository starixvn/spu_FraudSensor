using NPOI.SS.UserModel;

namespace Fanex.NAP.FS.Websites.Service.Utilities
{
    public static class Excel
    {
        public static void CopyRow(IRow source, IRow target)
        {
            foreach (var cell in source.Cells)
            {
                var targetCell = target.CreateCell(cell.ColumnIndex);
                targetCell.CellStyle = cell.CellStyle;
            }

            target.Height = source.Height;
        }
    }
}