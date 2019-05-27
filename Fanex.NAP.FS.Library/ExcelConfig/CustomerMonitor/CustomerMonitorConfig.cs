namespace Fanex.NAP.FS.Library.ExcelConfig.CustomerMonitor
{
    using Fanex.NAP.FS.Library.Core.ExportExcel;
    using Fanex.NAP.FS.Library.Criteria.CustomerMonitor;
    using Fanex.NAP.FS.Library.Models.CustomerMonitor;
    using NPOI.SS.UserModel;
    using NPOI.XSSF.UserModel;

    public class CustomerMonitorConfig : FSExcelConfig<CustomerGetCriteria, Customer, object>
    {
        protected override int SheetIndex => 0;

        protected override string SheetName => null;

        protected override void FillData(IRow row, Customer data)
        {
            var idx = 0;

            row.GetCell(idx++).SetCellValue(row.RowNum - 2);
            row.GetCell(idx++).SetCellValue(data.Username);
            row.GetCell(idx++).SetCellValue($"{data.TicketCount} / {data.ViolationCount} / {data.MatchCount}");
            row.GetCell(idx).SetCellValue(data.Note != null ? ReplaceSpecialCharacter(data.Note) : "");
        }

        protected override void FillLegend(IRow row, CustomerGetCriteria legend)
        {
            var cell = row.GetCell(0);

            new XSSFRichTextString()
                .WriteBold("From: ").Write(legend.FromDate.ToString("MMM dd, yyyy") + "\n")
                .WriteBold("To: ").Write(legend.ToDate.ToString("MMM dd, yyyy") + "\n")
                .SetValue(cell);
        }

        private static string ReplaceSpecialCharacter(string obj)
        {
            if (!string.IsNullOrEmpty(obj))
            {
                return obj.Replace("&amp;", "&").Replace("&gt;", ">").Replace("&lt;", "<");
            }
            return obj;
        }
    }
}