namespace Fanex.NAP.FS.WinService.Core
{
    public interface ITaskExecutor
    {
        void Start();

        void Stop();

        void Execute();
    }
}
