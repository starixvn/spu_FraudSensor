namespace Fanex.NAP.FS.ExecutingService.Schedules
{
    using Quartz;
    public interface ISchedulesFS
    {
        ISchedulesFS CreateTrigger(string name, int interval);
        ISchedulesFS Schedule(IJobDetail jobDetail);
        ISchedulesFS PauseAll();
        ISchedulesFS Shutdown();
        ISchedulesFS Start();
    }
}