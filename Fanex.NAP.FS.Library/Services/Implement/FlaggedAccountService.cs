namespace Fanex.NAP.FS.Library.Services
{
    using Core.BaseClasses;
    using Data.Repository;
    using Models;

    public class FlaggedAccountService : IFlaggedAccountService
    {
        public DBResult InsertAccounts(CriteriaBase criteria)
        {
            return DataService.UpdateData(criteria, "Insert Flagged Account");
        }

        public DBResult DeleteAccounts(CriteriaBase criteria)
        {
            return DataService.UpdateData(criteria, "Delete Flagged Account");
        }
    }
}