using Fanex.NAP.FS.Websites.Criteria.FraudCase;
using Fanex.NAP.FS.Websites.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Fanex.NAP.FS.Websites.Criteria.FraudCase.DelayedGoal;
using System.IO;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using Fanex.NAP.FS.Websites.Service.Utilities;

namespace Fanex.NAP.FS.Websites.Service.FraudCase
{
    public static class FraudCaseService
    {
        public static bool CheckActiveFraudCase(FraudCaseId id)
        {
            var criteria = new GetIsActiveCriteria
            {
                FraudCaseId = (int)id
            };

            var result = DataService.GetSingle<dynamic>(criteria);

            return result.IsActive;
        }

        public static void ChangeFraudCaseStatus(FraudCaseId id, int userId)
        {
            var criteria = new UpdateFraudCaseStatusCriteria
            {
                FraudCaseId = (int)id,
                UserId = userId
            };

            DataService.UpdateData(criteria, "FraudCase");
        }

        public static IEnumerable<ViewModels.FraudCase.DelayedGoal.Customer> GetDelayedGoalCustomers()
        {
            var criteria = new DelayedGoalCustomerGetCriteria();

            return 
                DataService
                .GetData<Models.FraudCase.DelayedGoal.Customer>(criteria)
                .Select(u => new ViewModels.FraudCase.DelayedGoal.Customer(u))
                .OrderBy(u => u.Name);
        }

        public static DBResult DeleteDelayedGoalCustomer(int custId, int userId)
        {
            return DataService.UpdateData(new DelayedGoalCustomerDeleteCriteria { CustId = custId, UserId = userId }, "FraudCase_DelayedGoal");
        }

        public static DBResult AddDelayedGoalCustomer(string username, int userId)
        {
            return DataService.UpdateData(new DelayedGoalCustomerAddCriteria { Username = username, UserId = userId }, "FraudCase_DelayedGoal");
        }

        public static byte[] ExportDelayedCustomersToExcel(string template, string searchString)
        {
            var data = GetDelayedGoalCustomers();

            var idx = 0;

            using (var file = File.OpenRead(template))
            {
                var wb = new XSSFWorkbook(file);
                var sheet = wb.GetSheetAt(0);

                var param = new XSSFRichTextString();

                param.Append("Search string: ", new XSSFFont { IsBold = true });
                param.Append(searchString);
                sheet.GetRow(1).GetCell(0).SetCellValue(param);

                foreach (var cust in data.Where(u => string.IsNullOrWhiteSpace(searchString) || u.Name.ToLowerInvariant().Contains(searchString)))
                {
                    WriteCustomer(sheet, idx, cust);
                    idx++;
                }

                sheet.RemoveRow(sheet.CreateRow(sheet.LastRowNum + 1));

                using (var mem = new NpoiStream())
                {
                    wb.Write(mem);
                    mem.Position = 0;

                    mem.AllowClose = true;

                    return mem.ToArray();
                }
            }
        }

        private static void WriteCustomer(ISheet sheet, int idx, ViewModels.FraudCase.DelayedGoal.Customer customer)
        {
            sheet.ShiftRows(sheet.LastRowNum, sheet.LastRowNum, 1, true, false);

            var lastRow = sheet.GetRow(sheet.LastRowNum);

            var row = sheet.CreateRow(sheet.LastRowNum - 1);

            Excel.CopyRow(lastRow, row);

            row.GetCell(0).SetCellValue(idx + 1);
            row.GetCell(1).SetCellValue(customer.Name);
            row.GetCell(2).SetCellValue(customer.Creator);
            row.GetCell(3).SetCellValue(customer.AddedDate);
        }
    }
}