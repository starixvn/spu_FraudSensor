namespace Fanex.NAP.FS.WinService
{
    using Fanex.NAP.FS.Library.Models;

    public interface IFraudCaseExecutor
    {
        FraudCaseId FraudCaseId { get; }

        void Execute();
    }
}