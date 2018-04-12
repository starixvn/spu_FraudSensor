namespace Fanex.NAP.FS.Websites.ExcelConfig.SuspiciousCustomer
{
    using Fanex.NAP.FS.Library.Core.ExportExcel;
    using Fanex.NAP.FS.Library.Criteria;
    using Fanex.NAP.FS.Library.Models;
    using NPOI.SS.UserModel;
    using NPOI.XSSF.UserModel;

    public class SuspiciousCustomerConfig : FSExcelConfig<SuspiciousCustomerReportGetCriteria, SuspiciousCustomerReportModel, object>
    {
        private readonly string fraudCaseName;

        private readonly string assignee;

        public SuspiciousCustomerConfig(string fraudCaseName, string assignee)
        {
            this.fraudCaseName = fraudCaseName;
            this.assignee = assignee;
        }

        protected override int SheetIndex => 0;

        protected override string SheetName => null;

        protected override void FillData(IRow row, SuspiciousCustomerReportModel data)
        {
            var dataRow = data;

            row.GetCell(0).SetCellValue(row.RowNum - 2);
            row.GetCell(1).SetCellValue(dataRow.IsDirty ? "Yes" : "No");
            row.GetCell(2).SetCellValue(dataRow.IsDelinquent ? "Yes" : "No");
            row.GetCell(3).SetCellValue(dataRow.IsHedge ? "Yes" : "No");
            row.GetCell(4).SetCellValue(dataRow.IsVerified ? "Yes" : "No");
            row.GetCell(5).SetCellValue(dataRow.HasNote ? "Yes" : "No");
            row.GetCell(6).SetCellValue(dataRow.Username);
            row.GetCell(7).SetCellValue(dataRow.TotalViolation);
            row.GetCell(8).SetCellValue(dataRow.ViolatedCases);
            row.GetCell(9).SetCellValue(dataRow.BetCount);
            row.GetCell(10).SetCellValue(dataRow.Assignee == null ? "-" : dataRow.Assignee.Length > 0 ? dataRow.Assignee : "-");
        }

        protected override void FillLegend(IRow row, SuspiciousCustomerReportGetCriteria legend)
        {
            var criteria = legend;

            var cell_value = new XSSFRichTextString();

            var cell_value1 = new XSSFRichTextString();

            cell_value
                .GetLegend("From Date", criteria.FromDate.ToString("MMM dd, yyyy") + "\n")
                .GetLegend("To Date", criteria.ToDate.ToString("MMM dd, yyyy") + "\n")
                .GetLegend("Member", criteria.Username + "\n")
                .GetLegend("Fraud Case", fraudCaseName + "\n");

            cell_value1
                .GetLegend("Verified", criteria.IsVerified == null ? "All\n" : (criteria.IsVerified.Value ? "Yes\n" : "No\n"))
                .GetLegend("Classification", criteria.Classification == 0 ? "All\n" : (criteria.Classification == 1 ? "Delinquency\n" : (criteria.Classification == 2 ? "Dirty\n" : "Hedge\n")))
                .GetLegend("Assignees", assignee);

            row.GetCell(0).SetCellValue(cell_value);
            row.GetCell(5).SetCellValue(cell_value1);
        }
    }
}