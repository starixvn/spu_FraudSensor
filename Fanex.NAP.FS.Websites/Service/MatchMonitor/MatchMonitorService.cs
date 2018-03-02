using Fanex.Data;
using Fanex.NAP.FS.Websites.Criteria.MatchMonitor;
using Fanex.NAP.FS.Websites.Models;
using Fanex.NAP.FS.Websites.ViewModels.MatchMonitor;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using RemotingRef;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fanex.NAP.FS.Websites.Service.MatchMonitor
{
    public static class MatchMonitorService
    {
        public static MatchList GetMatches(MatchGetAllCriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (var query = db.QueryMultiple(criteria))
                {
                    var fraudCases = query.Read<Models.MatchMonitor.FraudCase>();
                    var matches = query.Read<Models.MatchMonitor.Match>();

                    return new MatchList
                    {
                        Matches =
                            matches
                            .GroupJoin(fraudCases, m => m.MatchId, f => f.MatchId, GetMatch)
                            .OrderBy(m => m.Status == "Running" ? 0 : 1)
                            .ThenByDescending(m => m.FraudCases.Sum(f => f.Value.Violations.Value))
                            .ThenBy(m => m.MatchId),
                        FraudCases = fraudCases.GroupBy(f => f.FraudCaseName).Select(g => g.Key)
                    };
                }
            }
        }

        public static IEnumerable<Models.MatchMonitor.League> GetLeagues(LeagueGetCriteria criteria)
        {
            return DataService.GetData<Models.MatchMonitor.League>(criteria);
        }

        public static object VerifyMatch(VerifyMatchCriteria criteria)
        {
            DataService.UpdateData(criteria);

            return null;
        }

        public static Models.MatchMonitor.BalanceUpDate GetBalanceUpDate()
        {
            return DataService.GetData<Models.MatchMonitor.BalanceUpDate>(new BalanceUpDateGetCriteria()).First();
        }

        public static byte[] ExportExcel(MatchGetAllCriteria criteria, string template)
        {
            var data = GetMatches(criteria);

            using (var stream = File.Open(template, FileMode.Open, FileAccess.Read))
            {
                var excel = new XSSFWorkbook(stream);
                var sheet = excel.GetSheetAt(0);

                sheet = data.Matches.Aggregate(sheet, WriteMatch);

                sheet.RemoveRow(sheet.CreateRow(sheet.LastRowNum + 1));

                using (var mem = new NpoiStream())
                {
                    excel.Write(mem);
                    mem.Position = 0;

                    mem.AllowClose = true;

                    return mem.ToArray();
                }
            }
        }

        public static ISheet WriteMatch(ISheet sheet, Match m)
        {
            sheet.ShiftRows(sheet.LastRowNum, sheet.LastRowNum, 1, true, false);

            var lastRow = sheet.GetRow(sheet.LastRowNum);

            var row = sheet.CreateRow(sheet.LastRowNum - 1);

            CopyRow(lastRow, row);

            row.GetCell(0).SetCellValue(row.RowNum);
            row.GetCell(1).SetCellValue(m.Verified ? "Yes" : "No");
            row.GetCell(2).SetCellValue(m.VerifiedBy ?? "-");
            row.GetCell(3).SetCellValue(WriteEvent(m.Event));
            row.GetCell(4).SetCellValue(m.Score);
            row.GetCell(5).SetCellValue(m.Status);
            row.GetCell(6).SetCellValue(WriteFraudCase(m.FraudCases.ContainsKey("1") ? m.FraudCases["1"] : null));
            row.GetCell(7).SetCellValue(WriteFraudCase(m.FraudCases.ContainsKey("6") ? m.FraudCases["6"] : null));
            row.GetCell(8).SetCellValue(WriteFraudCase(m.FraudCases.ContainsKey("8") ? m.FraudCases["8"] : null));

            return sheet;
        }

        private static void CopyRow(IRow source, IRow target)
        {
            foreach (var cell in source.Cells)
            {
                var targetCell = target.CreateCell(cell.ColumnIndex);
                targetCell.CellStyle = cell.CellStyle;
            }

            target.Height = source.Height;
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

        private static IRichTextString WriteFraudCase(FraudCase fraudCase)
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

        private static Match GetMatch(Models.MatchMonitor.Match match, IEnumerable<Models.MatchMonitor.FraudCase> fraudCases)
        {
            var userService = new Core.Service.Impl.UserService();

            var homeName = RefData.GetTeamName(match.HomeId.ToString());
            var awayName = RefData.GetTeamName(match.AwayId.ToString());

            return new Match
            {
                Event = new Event
                {
                    HomeName = homeName == null ? "-" : homeName[1].ToString(),
                    AwayName = awayName == null ? "-" : awayName[1].ToString(),
                    LeagueName = match.LeagueName
                },
                FraudCases =
                    fraudCases
                    .GroupBy(f => Tuple.Create(f.FraudCaseId, f.FraudCaseName))
                    .Select(g => new FraudCase(g.Key.Item1, g.Key.Item2, g))
                    .ToDictionary(f => f.FraudCaseId.ToString()),
                MatchId = match.MatchId,
                Verified = match.IsVerified,
                VerifiedBy = match.VerifiedBy == null ? null : userService.Get(match.VerifiedBy.Value).Username,
                Status = (char)(match.EventStatus[0] - 32) + match.EventStatus.Remove(0, 1),
                Score = string.Format("{0} - {1}", match.LiveHomeScore, match.LiveAwayScore)
            };
        }
    }
}