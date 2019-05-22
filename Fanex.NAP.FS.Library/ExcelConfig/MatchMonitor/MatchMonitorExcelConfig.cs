namespace Fanex.NAP.FS.Library.ExcelConfig.MatchMonitor
{
    using Fanex.NAP.FS.Library.Core.BaseClasses;
    using Fanex.NAP.FS.Library.Core.ExportExcel;
    using Fanex.NAP.FS.Library.Criteria.MatchMonitor;
    using Fanex.NAP.FS.Library.ViewModels.MatchMonitor;
    using NPOI.SS.UserModel;
    using NPOI.XSSF.UserModel;
    using System.Collections.Generic;

    public class MatchMonitorExcelConfig : FSExcelConfig<MatchGetAllCriteria, Match, object>
    {
        protected override int SheetIndex => 0;

        protected override string SheetName => null;

        protected override bool HasSummary => false;

        protected override void FillData(IRow row, Match data)
        {
            row.GetCell(0).SetCellValue(row.RowNum - 2);
            row.GetCell(1).SetCellValue(data.Verified ? "Yes" : "No");
            row.GetCell(2).SetCellValue(data.VerifiedBy ?? "-");
            row.GetCell(3).SetCellValue(WriteEvent(data.Event));
            row.GetCell(4).SetCellValue(data.Score);
            row.GetCell(5).SetCellValue(data.Status);
            row.GetCell(6).SetCellValue(WriteFraudCase(data.FraudCases.ContainsKey("1") ? data.FraudCases["1"] : null));
            row.GetCell(7).SetCellValue(WriteFraudCase(data.FraudCases.ContainsKey("6") ? data.FraudCases["6"] : null));
            row.GetCell(8).SetCellValue(WriteFraudCase(data.FraudCases.ContainsKey("8") ? data.FraudCases["8"] : null));
            row.GetCell(9).SetCellValue(WriteFraudCase(data.FraudCases.ContainsKey("10") ? data.FraudCases["10"] : null));
            row.GetCell(10).SetCellValue(data.Notes != null ? ReplaceSpecialCharacter(data.Notes) : "-");
        }

        private static IRichTextString WriteEvent(Event e)
        {
            var result = new XSSFRichTextString();

            var font = new XSSFFont
            {
                IsBold = true
            };

            result.Append(e.HomeName, font);
            result.Append(" - vs - ");
            result.Append(e.AwayName, font);
            result.Append("\n");

            var leagueFont = new XSSFFont
            {
                Color = IndexedColors.RoyalBlue.Index
            };

            result.Append(e.LeagueName, leagueFont);

            return result;
        }

        private static IRichTextString WriteFraudCase(ViewModels.MatchMonitor.FraudCase fraudCase)
        {
            if (fraudCase == null)
            {
                return new XSSFRichTextString("-");
            }

            var result = new XSSFRichTextString();

            if (fraudCase.Violations == 0)
            {
                result.Append("0", new XSSFFont { Color = IndexedColors.Green.Index });
            }
            else
            {
                var violations = fraudCase.Violations.Value;

                var font = new XSSFFont { Color = IndexedColors.Red.Index };

                result.Append(violations < 1000 ? violations.ToString() : FormatNumber(violations), font);
            }

            return result;
        }

        private static string FormatNumber(int num)
        {
            var stack = new Stack<string>();

            var a = num;

            while (a / 1000 != 0)
            {
                var modResult = a % 1000;

                stack.Push(string.Format("{0}{1}{2}", modResult / 100, modResult % 100 / 10, modResult % 10));
                a = a / 1000;
            }

            if (a != 0)
            {
                stack.Push(a.ToString());
            }

            return string.Join(",", stack);
        }

        protected override void FillLegend(IRow row, MatchGetAllCriteria legend)
        {
            var legend0 = new XSSFRichTextString();
            var legend1 = new XSSFRichTextString();

            WriteLegend(legend0, "Event Date", legend.EventDate.ToString("MMM dd, yyyy"));
            WriteLegend(legend0, "Sport", "Soccer");
            WriteLegend(legend0, "Match Status", legend.MatchStatus);
            WriteLegend(legend1, "League", legend.LeagueIds.Contains(",") ? "All Leagues" : CachedRef.GetLeagueName(int.Parse(legend.LeagueIds)));
            WriteLegend(legend1, "Verified", legend.IsVerified == null ? "All" : legend.IsVerified.Value ? "Yes" : "No");
            WriteLegend(legend1, "Main Markets Only", legend.IsMainMarket ? "Yes" : "No");

            row.GetCell(0).SetCellValue(legend0);
            row.GetCell(3).SetCellValue(legend1);
        }

        private static void WriteLegend(XSSFRichTextString legend, string prop, string value)
        {
            legend.Append(prop + ": ", new XSSFFont { IsBold = true });
            legend.Append(value + "\n");
        }

        private static string ReplaceSpecialCharacter(string obj)
        {
            if (!string.IsNullOrEmpty(obj))
            {
                return obj.Replace("&amp;", "&").Replace("&gt;", ">").Replace("&lt;", "<");
            }
            return obj;
        }
    }
}