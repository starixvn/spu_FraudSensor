namespace Fanex.NAP.FS.ExecutingService.Jobs
{
    using Fanex.NAP.FS.ExecutingService.Schedules;    
    using Quartz;

    public class FlaggedAcocuntSchedule : IScheduleJob, ISchedule
    {
        private IScheduler _scheduler;

        public FlaggedAcocuntSchedule(IScheduler scheduler)
        {
            _scheduler = scheduler;
        }

        public IJobDetail CreateJob(string name, string groupName, string description)
        {
            return JobBuilder.Create<FlaggedAccountJob>()
                               .WithIdentity(name, groupName)
                               .WithDescription(description)
                               .StoreDurably()
                               .Build();
        }

        public ITrigger CreateTrigger(string name, int interval)
        {
            return TriggerBuilder.Create()
                .WithIdentity(name)
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(interval)
                    .RepeatForever())
                .Build();
        }

        public void Schedule(IJobDetail jobDetail, ITrigger trigger)
        {
            _scheduler.ScheduleJob(jobDetail, trigger);
        }

        public void PauseAll()
        {
            _scheduler.PauseAll();
        }

        public void Shutdown()
        {
            _scheduler.Shutdown();
        }

        public void Start()
        {
            _scheduler.Start();
        }
    }
}
