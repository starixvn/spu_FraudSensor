namespace Fanex.NAP.FS.Library.ExcelConfig.MatchMonitor
{
    using Fanex.NAP.FS.Library.Core.ExportExcel;
    using Fanex.NAP.FS.Library.Models;
    using Fanex.NAP.FS.Library.ViewModels.MatchMonitor.FraudCaseDetails;
    using NPOI.SS.UserModel;
    using NPOI.XSSF.UserModel;

    public class CustomerDetailConfig : FSExcelConfig<MatchFraudCaseInfo, CustomerDetails, object>
    {
        private readonly bool delete2Nd;

        public CustomerDetailConfig(int customerNo)
        {
            SheetName = string.Format("By Customers ({0})", customerNo);
        }

        public CustomerDetailConfig(int customerNo, bool delete2nd) : this(customerNo)
        {
            delete2Nd = delete2nd;
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
            row.GetCell(cell_idx).SetCellValue(customer.AssociatedAccounts);
        }

        protected override void FillLegend(IRow row, MatchFraudCaseInfo legend)
        {
            if (legend.FraudCaseId == (int)FraudCaseId.FlaggedAccount)
            {
                FillLegendFlaggedAccount(row, legend);

                return;
            }
            var cell1 = row.GetCell(0);
            var cell2 = row.GetCell(2);
            var cell3 = row.GetCell(6);
            string[] arrFilter = legend.Filter;

            new XSSFRichTextString()
               .WriteBold("Match: ").Write(string.Format("{0} - vs - {1}\n", legend.Home as string, legend.Away as string))
               .WriteBold("Date: ").Write(legend.EventDate.ToString("MMM dd, yyyy") + "\n")
               .WriteBold("HT/FT: ").Write(legend.HTScore + " / " + legend.FTScore + "\n")
               .WriteBold("Fraud Case: ").Write(legend.FraudCase + "\n")
               .SetValue(cell1);                

            if (arrFilter != null && arrFilter.Length >= 7)
            {
                new XSSFRichTextString()
                   .WriteBold("Site: ").Write(arrFilter[6] + "\n")
                   .WriteBold("Currency: ").Write(arrFilter[1] + "\n")
                   .WriteBold("Evidence: ").Write(arrFilter[2] + "\n")
                   .SetValue(cell2);

                new XSSFRichTextString()
                   .WriteBold("Market: ").Write(arrFilter[0] + "\n")
                   .WriteBold("Trans Date: ").Write(arrFilter[3] + " - " + arrFilter[4] + "\n")
                   .WriteBold("Score/HDP: ").Write(arrFilter[5] + "\n")
                   .SetValue(cell3);
            }
        }

        private void FillLegendFlaggedAccount(IRow row, MatchFraudCaseInfo legend)
        {
            var cell1 = row.GetCell(0);
            var cell2 = row.GetCell(3);
            var cell3 = row.GetCell(6);
            string[] arrFilter = legend.Filter;

            new XSSFRichTextString()
               .WriteBold("Match: ").Write(string.Format("{0} - vs - {1}\n", legend.Home as string, legend.Away as string))
               .WriteBold("Date: ").Write(legend.EventDate.ToString("MMM dd, yyyy") + "\n")
               .WriteBold("HT/FT: ").Write(legend.HTScore + " / " + legend.FTScore + "\n")
               .WriteBold("Fraud Case: ").Write(legend.FraudCase + "\n")
               .SetValue(cell1);

            if (arrFilter != null && arrFilter.Length >= 7)
            {
                new XSSFRichTextString()
                   .WriteBold("Site: ").Write(arrFilter[6] + "\n")
                   .WriteBold("Currency: ").Write(arrFilter[1] + "\n")
                   .WriteBold("Evidence: ").Write(arrFilter[2] + "\n")
                   .SetValue(cell2);

                new XSSFRichTextString()
                   .WriteBold("Market: ").Write(arrFilter[0] + "\n")
                   .WriteBold("Trans Date: ").Write(arrFilter[3] + " - " + arrFilter[4] + "\n")
                   .WriteBold("Bet Type: ").Write(arrFilter[5] + "\n")
                   .SetValue(cell3);
            }
        }

        protected override void CleanupTemplate(ISheet sheet)
        {
            base.CleanupTemplate(sheet);

            if (delete2Nd)
            {
                sheet.Workbook.RemoveSheetAt(0);
            }
        }
    }
}