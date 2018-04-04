using Fanex.NAP.FS.Websites.Criteria.FraudCase;
using Fanex.NAP.FS.Websites.Models;
using System.Collections.Generic;
using System.Linq;
using Fanex.NAP.FS.Websites.Criteria.FraudCase.DelayedGoal;
using Fanex.NAP.FS.Library.Core.ExportExcel;
using Fanex.NAP.FS.Websites.ExcelConfig.FraudCase.DelayedGoal;

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
            
            var wb = ExcelUtilities.GetWorkbook(template);

            new DelayedGoalExcelConfig().RenderExcel(wb, searchString, data.Where(u => string.IsNullOrWhiteSpace(searchString) || u.Name.ToLowerInvariant().Contains(searchString)), null);

            return wb.ToBytes();
        }
    }
}