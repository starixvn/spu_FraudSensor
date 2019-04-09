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

            new XSSFRichTextString()
                .WriteBold("Event: ").Write(string.Format("{0} - vs - {1}\n", legend.Home as string, legend.Away as string))
                .WriteBold("Date: ").Write(legend.EventDate.ToString("MMM dd, yyyy") + "\n")
                .WriteBold("Fraud Case: ").Write(legend.FraudCase + "\n")
                .WriteBold("HT/FT Score: ").Write(legend.HTScore + " / " + legend.FTScore + "\n")
                .SetValue(cell1);
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