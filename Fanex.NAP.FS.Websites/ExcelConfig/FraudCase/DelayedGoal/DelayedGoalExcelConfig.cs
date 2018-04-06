using Fanex.NAP.FS.Library.Core.ExportExcel;
using Fanex.NAP.FS.Websites.ViewModels.FraudCase.DelayedGoal;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Fanex.NAP.FS.Websites.ExcelConfig.FraudCase.DelayedGoal
{
    public class DelayedGoalExcelConfig : FSExcelConfig<string, Customer, object>
    {
        protected override int SheetIndex => 0;

        protected override string SheetName => null;

        protected override bool HasSummary => false;

        protected override void FillData(IRow row, Customer data)
        {
            row.GetCell(0).SetCellValue(row.RowNum - 2);
            row.GetCell(1).SetCellValue(data.Name);
            row.GetCell(2).SetCellValue(data.Creator);
            row.GetCell(3).SetCellValue(data.AddedDate);
        }

        protected override void FillLegend(IRow row, string legend)
        {
            //// no need to fill legend in this config
        }
    }
}