namespace Fanex.NAP.FS.Library.ExcelConfig.MatchMonitor
{
    using Fanex.NAP.FS.Library.Core.ExportExcel;
    using Fanex.NAP.FS.Library.ViewModels.MatchMonitor.FraudCaseDetails;
    using NPOI.SS.UserModel;
    using NPOI.XSSF.UserModel;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class ArbitrageConfig : FSExcelConfig<MatchFraudCaseInfo, ArbitrageGroupTicketList, object>
    {
        private readonly bool delete2Nd;

        public ArbitrageConfig(int ticketNo)
        {
            SheetName = string.Format("By Tickets ({0})", ticketNo);
        }

        public ArbitrageConfig(int ticketNo, bool delete2nd) : this(ticketNo)
        {
            delete2Nd = delete2nd;
        }        

        protected override int SheetIndex => 0;

        protected override string SheetName { get; }

        protected override void FillData(IRow row, ArbitrageGroupTicketList data)
        {
            if (row.RowNum > 5)
            {
                row = CreateRow(row);
                IRow lastRow = row.Sheet.GetRow(row.RowNum - 1);
                lastRow.GetCell(0).SetCellValue("Score/HDP: " + data.Score + " " + data.BettypeName + " " + data.Handicap.ToString("0.##"));
                lastRow.GetCell(0).CellStyle = row.Sheet.GetRow(4).GetCell(0).CellStyle;
                MergeRange(lastRow.Sheet, lastRow);
            }
            else
            {
                row.Sheet.GetRow(4).GetCell(0).SetCellValue("Score/HDP: " + data.Score + " " + data.BettypeName + " " + data.Handicap.ToString("0.##"));
            }            

            IEnumerator<ArbitrageTicket> dataHome = data.Home.GetEnumerator();
            IEnumerator<ArbitrageTicket> dataAway = data.Away.GetEnumerator();
            bool hasHome = dataHome.MoveNext();
            bool hasAway = dataAway.MoveNext();
           
            while (hasHome || hasAway)
            {
                var idx = 0;

                if (hasHome)
                {
                    var fraudByIP = dataHome.Current.IsIPFraud ? "Yes" : "";
                    row.CreateCell(idx++).SetCellValue(dataHome.Current.Username);
                    row.CreateCell(idx++).SetCellValue(string.Join(",", dataHome.Current.Evidences));
                    row.CreateCell(idx++).SetCellValue(dataHome.Current.DangerousLevel);
                    row.CreateCell(idx++).SetCellValue(dataHome.Current.Site);
                    row.CreateCell(idx++).SetCellValue(dataHome.Current.Currency);
                    row.CreateCell(idx++).SetCellValue(fraudByIP);
                    row.CreateCell(idx++).SetCellValue(dataHome.Current.BetId.ToString());
                    row.CreateCell(idx++).SetCellValue((double)dataHome.Current.Odds);
                    row.CreateCell(idx++).SetCellValue(dataHome.Current.Stake.Virtual);
                    row.CreateCell(idx++).SetCellValue(dataHome.Current.Stake.ComPT);
                    row.CreateCell(idx++).SetCellValue(dataHome.Current.Stake.Full);
                    row.CreateCell(idx).SetCellValue(DateTime.Parse(dataHome.Current.TransDate.Replace("<br/>", "")));
                }

                idx = 13;
                if (hasAway)
                {
                    var fraudByIP = dataAway.Current.IsIPFraud ? "Yes" : "";
                    row.CreateCell(idx++).SetCellValue(dataAway.Current.Username);
                    row.CreateCell(idx++).SetCellValue(string.Join(",", dataAway.Current.Evidences));
                    row.CreateCell(idx++).SetCellValue(dataAway.Current.DangerousLevel);
                    row.CreateCell(idx++).SetCellValue(dataAway.Current.Site);
                    row.CreateCell(idx++).SetCellValue(dataAway.Current.Currency);
                    row.CreateCell(idx++).SetCellValue(fraudByIP);
                    row.CreateCell(idx++).SetCellValue(dataAway.Current.BetId.ToString());
                    row.CreateCell(idx++).SetCellValue((double)dataAway.Current.Odds);
                    row.CreateCell(idx++).SetCellValue(dataAway.Current.Stake.Virtual);
                    row.CreateCell(idx++).SetCellValue(dataAway.Current.Stake.ComPT);
                    row.CreateCell(idx++).SetCellValue(dataAway.Current.Stake.Full);
                    row.CreateCell(idx).SetCellValue(DateTime.Parse(dataAway.Current.TransDate.Replace("<br/>", "")));
                }

                hasHome = dataHome.MoveNext();
                hasAway = dataAway.MoveNext();

                if (hasHome || hasAway)
                {
                    row = CreateRow(row);
                }
            }
        }

        protected override void FillLegend(IRow row, MatchFraudCaseInfo legend)
        {
            var cell1 = row.GetCell(0);
            var cell2 = row.GetCell(5);
            var cell3 = row.GetCell(9);
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

        protected override void CleanupTemplate(ISheet sheet)
        {
            base.CleanupTemplate(sheet);

            if (delete2Nd)
            {
                sheet.Workbook.RemoveSheetAt(1);
            }
        }

        private IRow CreateRow(IRow rSource)
        {
            var sheet = rSource.Sheet;
            var rowIndex = ShiftTemplate(sheet);
            var newRow = sheet.CreateRow(rowIndex);

            return newRow;
        }

        private void MergeRange(ISheet sheet, IRow row)
        {
            sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(row.RowNum, row.RowNum, 0, 24));
        }
    }
}