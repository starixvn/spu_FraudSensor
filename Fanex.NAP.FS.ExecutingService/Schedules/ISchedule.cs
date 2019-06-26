namespace Fanex.NAP.FS.ExecutingService.Schedules
{
    public interface ISchedule
    {
        void Shutdown();

        void Start();

        void PauseAll();
    }
}
