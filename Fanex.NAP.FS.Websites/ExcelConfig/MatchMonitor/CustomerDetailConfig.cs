using Fanex.NAP.FS.Library.Core.ExportExcel;
using Fanex.NAP.FS.Websites.ViewModels.MatchMonitor.FraudCaseDetails;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Fanex.NAP.FS.Websites.ExcelConfig.MatchMonitor
{
    public class CustomerDetailConfig : FSExcelConfig<MatchFraudCaseInfo, CustomerDetails, object>
    {
        public CustomerDetailConfig(int customerNo)
        {
            SheetName = string.Format("By Customers ({0})", customerNo);
        }

        protected override int SheetIndex => 1;

        protected override string SheetName { get;  }

        protected override void FillData(IRow row, CustomerDetails data)
        {
            var customer = data;
            var cell_idx = 0;

            row.GetCell(cell_idx++).SetCellValue(row.RowNum - 2);
            row.GetCell(cell_idx++).SetCellValue(customer.Member);
            row.GetCell(cell_idx++).SetCellValue(customer.CurrencyName);
            row.GetCell(cell_idx++).SetCellValue(customer.Tickets);
            row.GetCell(cell_idx++).SetCellValue(customer.TotalStake);
            row.GetCell(cell_idx++).SetCellValue(customer.TotalWinLoss);
            row.GetCell(cell_idx++).SetCellValue(customer.Evidence);
#pragma warning disable S1854 // ok here
            row.GetCell(cell_idx++).SetCellValue(customer.AssociatedAccounts);
#pragma warning restore S1854 // ok here
        }

        protected override void FillLegend(IRow row, MatchFraudCaseInfo legend)
        {
            var cell1 = row.GetCell(0);

            new XSSFRichTextString()
                .WriteBold("Event: ").Write(string.Format("{0} - vs - {1}\n", legend.Home as string, legend.Away as string))
                .WriteBold("Date: ").Write(legend.EventDate.ToString("MMM dd, yyyy") + "\n")
                .WriteBold("Fraud Case: ").Write(legend.FraudCase + "\n")
                .WriteBold("HT/FT Score: ").Write(legend.HTScore + " / " + legend.FTScore + "\n")
                .SetValue(cell1);
        }
    }
}