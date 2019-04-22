namespace Fanex.NAP.FS.Library.ExcelConfig.MatchMonitor
{
    using Fanex.NAP.FS.Library.Core.ExportExcel;
    using Fanex.NAP.FS.Library.ViewModels.MatchMonitor.FraudCaseDetails;
    using NPOI.SS.UserModel;
    using NPOI.XSSF.UserModel;
    using System;

    public class ArbitrageConfig : FSExcelConfig<MatchFraudCaseInfo, ArbitrageTicket, object>
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

        protected override void FillData(IRow row, ArbitrageTicket data)
        {
            var idx = 0;
            var fraudByIP = data.IsIPFraud ? "Yes" : "";

            row.GetCell(idx++).SetCellValue(data.Username);
            row.GetCell(idx++).SetCellValue(string.Join(",", data.Evidences));
            row.GetCell(idx++).SetCellValue(data.DangerousLevel);
            row.GetCell(idx++).SetCellValue(data.Currency);
            row.GetCell(idx++).SetCellValue(fraudByIP);
            row.GetCell(idx++).SetCellValue(data.BetId.ToString());
            row.GetCell(idx++).SetCellValue(data.BetTeam == "h" ? "Home" : "Away");
            row.GetCell(idx++).SetCellValue(data.BettypeName);
            row.GetCell(idx++).SetCellValue(data.Handicap);
            row.GetCell(idx++).SetCellValue(data.Score);
            row.GetCell(idx++).SetCellValue((double)data.Odds);
            row.GetCell(idx++).SetCellValue(data.Stake.Virtual);
            row.GetCell(idx++).SetCellValue(data.Stake.ComPT);
            row.GetCell(idx++).SetCellValue(data.Stake.Full);
            row.GetCell(idx).SetCellValue(DateTime.Parse(data.TransDate.Replace("<br/>", "")));
        }

        protected override void FillLegend(IRow row, MatchFraudCaseInfo legend)
        {
            var cell1 = row.GetCell(0);
            var cell2 = row.GetCell(3);
            var cell3 = row.GetCell(9);
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

        protected override void CleanupTemplate(ISheet sheet)
        {
            base.CleanupTemplate(sheet);

            if (delete2Nd)
            {
                sheet.Workbook.RemoveSheetAt(1);
            }
        }
    }
}