namespace Fanex.NAP.FS.Library.Services
{
    using Data.Repository;
    using NAP.Core.Command;

    public interface IFlaggedAccountService
    {
        ICommandResult InsertAccounts(ICriteria criteria);

        ICommandResult DeleteAccounts(ICriteria criteria);
    }
}