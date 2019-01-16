namespace Fanex.NAP.FS.WinService
{
    public interface IFraudCaseExecutor
    {
        void Start();

        void Stop();

        void Execute();
    }
}