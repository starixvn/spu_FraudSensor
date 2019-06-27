namespace Fanex.NAP.FS.ExecutingService.Schedules
{
    using Quartz;
    public interface IScheduleJob
    {
        ITrigger CreateTrigger(string name, int interval);
        void Schedule(IJobDetail jobDetail, ITrigger trigger);
        void PauseAll();
        void Shutdown();
        void Start();
    }
}
