namespace Fanex.NAP.FS.Library.ExcelConfig.MatchMonitor
{
    using Fanex.NAP.FS.Library.Core.ExportExcel;
    using Fanex.NAP.FS.Library.ViewModels.MatchMonitor.FraudCaseDetails;
    using NPOI.SS.UserModel;
    using NPOI.XSSF.UserModel;

    public class CustomerDetailConfig : FSExcelConfig<MatchFraudCaseInfo, CustomerDetails, object>
    {
        public CustomerDetailConfig(int customerNo)
        {
            SheetName = string.Format("By Customers ({0})", customerNo);
        }

        protected override int SheetIndex => 1;

        protected override string SheetName { get; }

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
            row.GetCell(cell_idx++).SetCellValue(customer.AssociatedAccounts);
        }

        protected override void FillLegend(IRow row, MatchFraudCaseInfo legend)
        {
            var cell1 = row.GetCell(0);
            var cell2 = row.GetCell(2);
            var cell3 = row.GetCell(6);
            string[] arrFilter = legend.Filter;

            new XSSFRichTextString()
                .WriteBold("Event: ").Write(string.Format("{0} - vs - {1}\n", legend.Home as string, legend.Away as string))
                .WriteBold("Date: ").Write(legend.EventDate.ToString("MMM dd, yyyy") + "\n")
                .WriteBold("Fraud Case: ").Write(legend.FraudCase + "\n")
                .WriteBold("HT/FT Score: ").Write(legend.HTScore + " / " + legend.FTScore + "\n")
                .SetValue(cell1);

            if (arrFilter != null && arrFilter.Length >= 6)
            {
                new XSSFRichTextString()
                   .WriteBold("Market: ").Write(arrFilter[0] + "\n")
                   .WriteBold("Currency: ").Write(arrFilter[1] + "\n")
                   .WriteBold("Evidence Code List: ").Write(arrFilter[2] + "\n")
                   .SetValue(cell2);

                new XSSFRichTextString()
                    .WriteBold("Trans Date From: ").Write(arrFilter[3] + "\n")
                    .WriteBold("Trans Date To: ").Write(arrFilter[4] + "\n")
                    .WriteBold("Score/HDP: ").Write(arrFilter[5] + "\n")
                    .SetValue(cell3);
            }
        }
    }
}