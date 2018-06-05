namespace Fanex.NAP.FS.Library.ExcelConfig.FraudCase.ScannedOfficeIP
{
    using Fanex.NAP.FS.Library.Core.ExportExcel;
    using Fanex.NAP.FS.Library.Models.FraudCase.ScannedOfficeIP;
    using NPOI.SS.UserModel;

    public class OfficeIPExcelConfig : FSExcelConfig<object, OfficeIp, object>
    {
        protected override int SheetIndex => 0;

        protected override string SheetName => null;

        protected override void FillData(IRow row, OfficeIp data)
        {
            row.GetCell(0).SetCellValue(row.RowNum - 2);
            row.GetCell(1).SetCellValue(data.FromIp);
            row.GetCell(2).SetCellValue(data.ToIp);
        }

        protected override void FillLegend(IRow row, object legend)
        {
            /// no legend
        }
    }
}