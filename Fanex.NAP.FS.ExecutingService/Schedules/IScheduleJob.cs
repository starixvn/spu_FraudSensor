namespace Fanex.NAP.FS.ExecutingService.Schedules
{
    using Quartz;
    public interface IScheduleJob
    {
        IScheduleJob CreateTrigger(string name, int interval);
        IScheduleJob Schedule(IJobDetail jobDetail);
        IScheduleJob PauseAll();
        IScheduleJob Shutdown();
        IScheduleJob Start();
    }
}