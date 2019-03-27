namespace Fanex.NAP.FS.Library.Services
{
    using Core.BaseClasses;
    using Core.ExportExcel;
    using Criteria;
    using Criteria.FraudCase;
    using Criteria.FraudCase.DelayedGoal;
    using DTO;
    using ExcelConfig.FraudCase.DelayedGoal;
    using Models;
    using System.Collections.Generic;
    using System.Linq;

    public class FraudCaseService : IFraudCaseService
    {
        public IEnumerable<FraudCase> Get(int? isInternal, int? isActive, int environment)
        {
            return DataService.GetData<FraudCase>(new FraudCaseGetCriteria(isInternal, isActive, environment));
        }

        public FraudCase GetByFraudCaseId(int fraudCaseId)
        {
            return DataService.GetSingle<FraudCase>(new FraudCaseGetByIdCriteria
            {
                FraudCaseId = fraudCaseId
            });
        }

        public bool CheckActiveFraudCase(FraudCaseId id, int envConfig)
        {
            var criteria = new GetIsActiveCriteria
            {
                FraudCaseId = (int)id,
                Environment = envConfig
            };

            var result = DataService.GetSingle<dynamic>(criteria);

            return result.IsActive;
        }

        public void ChangeFraudCaseStatus(FraudCaseId id, int userId)
        {
            var criteria = new UpdateFraudCaseStatusCriteria
            {
                FraudCaseId = (int)id,
                UserId = userId
            };

            DataService.UpdateData(criteria, "FraudCase");
        }

        public IEnumerable<Library.ViewModels.FraudCase.DelayedGoal.Customer> GetDelayedGoalCustomers()
        {
            var criteria = new DelayedGoalAccountGetCriteria();

            return
                DataService
                .GetData<Library.Models.FraudCase.DelayedGoal.Customer>(criteria)
                .Select(u => new Library.ViewModels.FraudCase.DelayedGoal.Customer(u))
                .OrderBy(u => u.Name);
        }

        public DBResult DeleteDelayedGoalCustomer(int custId, int userId)
        {
            return DataService.UpdateData(new DelayedGoalAccountDeleteCriteria { CustId = custId, UserId = userId }, "FraudCase_DelayedGoal");
        }

        public DBResult AddDelayedGoalCustomer(string username, int userId)
        {
            return DataService.UpdateData(new DelayedGoalAccountInsertCriteria { Username = username, UserId = userId }, "FraudCase_DelayedGoal");
        }

        public DBResult AddDelayGoalAssociatedAccount(string customerAssociationXML)
        {
            return DataService.UpdateData(new DelayGoalAssociationAccountInsertCriteria { CustomerAssociationXML = customerAssociationXML }, "FraudCase_DelayedGoal");
        }

        public byte[] ExportDelayedCustomersToExcel(string template, string searchString)
        {
            var data = GetDelayedGoalCustomers();
            var wb = ExcelUtilities.GetWorkbook(template);
            new DelayedGoalExcelConfig().RenderExcel(wb, searchString, data.Where(u => string.IsNullOrWhiteSpace(searchString) || u.Name.ToLowerInvariant().Contains(searchString)), null);

            return wb.ToBytes();
        }
    }
}