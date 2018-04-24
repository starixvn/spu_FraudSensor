namespace Fanex.NAP.FS.Library.Services
{
    using DTO;
    using Models;
    using System.Collections.Generic;

    public interface IFraudCaseService
    {
        FraudCase GetByFraudCaseId(int fraudCaseId);

        IEnumerable<FraudCase> Get(bool isInternal, bool isActive, int environment);

        bool CheckActiveFraudCase(FraudCaseId id);

        void ChangeFraudCaseStatus(FraudCaseId id, int userId);

        IEnumerable<Library.ViewModels.FraudCase.DelayedGoal.Customer> GetDelayedGoalCustomers();

        DBResult DeleteDelayedGoalCustomer(int custId, int userId);

        DBResult AddDelayedGoalCustomer(string username, int userId);

        DBResult AddDelayGoalAssociatedAccount(string customerAssociationXML);

        byte[] ExportDelayedCustomersToExcel(string template, string searchString);
    }
}