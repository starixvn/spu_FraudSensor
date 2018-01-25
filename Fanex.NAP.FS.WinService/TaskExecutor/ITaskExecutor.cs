namespace Fanex.NAP.FS.WinService
{
    public interface ITaskExecutor
    {
        void Start();

        void Stop();

        void Execute();
    }
}
