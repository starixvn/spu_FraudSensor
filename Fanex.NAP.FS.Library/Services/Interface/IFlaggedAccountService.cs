namespace Fanex.NAP.FS.Library.Services
{
    using Data.Repository;
    using Fanex.NAP.FS.Library.Core.BaseClasses;
    using Models;

    public interface IFlaggedAccountService
    {
        DBResult InsertAccounts(CommandBase criteria);

        DBResult DeleteAccounts(CommandBase criteria);
    }
}