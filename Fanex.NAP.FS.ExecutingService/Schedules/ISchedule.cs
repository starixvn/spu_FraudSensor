namespace Fanex.NAP.FS.ExecutingService.Schedules
{
    public interface ISchedule
    {
        ISchedule Create();

        ISchedule Shutdown();

        ISchedule Start();

        ISchedule Pause();
    }
}
