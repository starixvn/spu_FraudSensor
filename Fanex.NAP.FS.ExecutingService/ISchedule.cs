namespace Fanex.NAP.FS.ExecutingService
{
    public interface ISchedule
    {
        ISchedule Create();

        ISchedule Shutdown();

        ISchedule Start();

        ISchedule Pause();
    }
}
