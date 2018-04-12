namespace Fanex.NAP.FS.WinService
{
    using System.Collections.Generic;

    public interface IFraudCaseManager
    {
        IList<FraudCaseExecutor> FraudCaseExecutors { get; set; }

        void AddCase(int fraudCaseId);

        void UpdateCase(int fraudCaseId);

        void RemoveCase(int fraudCaseId);

        void ClearCases();
    }
}