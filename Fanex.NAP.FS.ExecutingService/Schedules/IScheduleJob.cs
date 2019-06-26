namespace Fanex.NAP.FS.ExecutingService.Schedules
{
    using Quartz;
    public interface IScheduleJob
    {
        IJobDetail CreateJob(string name, string groupName, string description);
        ITrigger CreateTrigger(string name, int interval);
        void Schedule(IJobDetail jobDetail, ITrigger trigger);
    }
}
