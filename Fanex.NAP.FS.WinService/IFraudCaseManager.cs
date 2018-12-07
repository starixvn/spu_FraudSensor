namespace Fanex.NAP.FS.WinService
{
    using Fanex.NAP.FS.Library.DTO;
    using Fanex.NAP.FS.Library.Models;
    using System.Collections.Generic;

    public interface IFraudCaseManager
    {
        IEnumerable<FraudCase> GetCases();

        FraudCase GetCase(FraudCaseId id);

        IFraudCaseExecutor GetExecutor(FraudCase fraudCase);
    }
}