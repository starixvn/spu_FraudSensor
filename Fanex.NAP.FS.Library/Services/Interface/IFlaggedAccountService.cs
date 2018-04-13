namespace Fanex.NAP.FS.Library.Services
{
    using Data.Repository;
    using Models;

    public interface IFlaggedAccountService
    {
        DBResult InsertAccounts(CriteriaBase criteria);

        DBResult DeleteAccounts(CriteriaBase criteria);
    }
}