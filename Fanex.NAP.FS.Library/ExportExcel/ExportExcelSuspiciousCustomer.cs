namespace Fanex.NAP.FS.Library.ExportExcel
{
    using Fanex.NAP.FS.Library.Core;
    using Fanex.NAP.FS.Library.Models;
    using NPOI.SS.UserModel;
    using System.Collections.Generic;
    using System.Linq;

    public class ExportExcelSuspiciousCustomer : FSExportExcel
    {
        private readonly IEnumerable<SuspiciousCustomerReportModel> _data;

        public ExportExcelSuspiciousCustomer(
            string filePath,
            string sheetName,
            IEnumerable<Legend> legends,
            IEnumerable<string> headers,
            IEnumerable<SuspiciousCustomerReportModel> data)
            : base(filePath, sheetName, legends, headers)
        {
            _data = data.ToList();
        }

        protected override void GenerateReportLegend()
        {
            //// There is no Legend for Suspicious Customer report.
        }

        protected override void GenerateReportContent()
        {
            if (!_data.Any())
            {
                SetUpACell(Sheet.CreateRow(RowIndex), 0, "There is no data.", ReportContentFont(), DecorateCellAlignment(ReportContentCellStyle(), HorizontalAlignment.Center));
                Sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(RowIndex, RowIndex, 0, Headers.Count() - 1));
                return;
            }

            for (var i = 1; i <= _data.Count(); i++)
            {
                var dataRow = _data.ElementAt(i - 1);
                var row = Sheet.CreateRow(RowIndex);
                SetUpACell(row, 0, i.ToString(), ReportContentFont(), ReportContentCellStyle(HorizontalAlignment.Center));
                SetUpACell(row, 1, dataRow.IsDelinquent ? "Yes" : "No", ReportContentFont(), ReportContentCellStyle(HorizontalAlignment.Center));
                SetUpACell(row, 2, dataRow.IsHedge ? "Yes" : "No", ReportContentFont(), ReportContentCellStyle(HorizontalAlignment.Center));
                SetUpACell(row, 3, dataRow.IsDirty ? "Yes" : "No", ReportContentFont(), ReportContentCellStyle(HorizontalAlignment.Center));
                SetUpACell(row, 4, dataRow.IsVerified ? "Yes" : "No", ReportContentFont(), ReportContentCellStyle(HorizontalAlignment.Center));
                SetUpACell(row, 5, dataRow.Username, ReportContentFont(), ReportContentCellStyle());
                SetUpACell(row, 6, dataRow.Evidence, ReportContentFont(), ReportContentCellStyle(HorizontalAlignment.Right));
                SetUpACell(row, 7, dataRow.TotalViolation.ToString(), ReportContentFont(), ReportContentCellStyle(HorizontalAlignment.Right));
                SetUpACell(row, 8, dataRow.ViolatedCases.ToString(), ReportContentFont(), ReportContentCellStyle(HorizontalAlignment.Right));
                SetUpACell(row, 9, dataRow.BetCount.ToString(), ReportContentFont(), ReportContentCellStyle(HorizontalAlignment.Right));
                SetUpACell(row, 10, dataRow.Assignee == null ? "-" : dataRow.Assignee.Length > 0 ? dataRow.Assignee : "-", ReportContentFont(), ReportContentCellStyle());

                RowIndex++;
            }
        }

        protected override void SetColumnsWidth()
        {
            Sheet.SetColumnWidth(0, 2000);
            Sheet.SetColumnWidth(1, 4000);
            Sheet.SetColumnWidth(2, 3000);
            Sheet.SetColumnWidth(3, 4000);
            Sheet.SetColumnWidth(4, 4000);
            Sheet.SetColumnWidth(5, 4000);
            Sheet.SetColumnWidth(6, 4000);
            Sheet.SetColumnWidth(7, 4000);
            Sheet.SetColumnWidth(8, 4000);
            Sheet.SetColumnWidth(9, 4000);
            Sheet.SetColumnWidth(10, 8000);
        }

        protected override void AutoSizeColumnsWidth()
        {
            //// Do nothing for Columns Width of Fraud Case Details report.
        }
    }
}
