namespace Fanex.NAP.FS.ExecutingService.Schedules
{
    using Fanex.NAP.FS.ExecutingService.Schedules;    
    using Quartz;

    public class ScheduleJob : IScheduleJob
    {
        private readonly IScheduler _scheduler;
        private ITrigger _trigger;

        public ScheduleJob(IScheduler scheduler)
        {
            _scheduler = scheduler;
        }

        public IScheduleJob CreateTrigger(string name, int interval)
        {
            _trigger = TriggerBuilder.Create()
                .WithIdentity(name)
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(interval)
                    .RepeatForever())
                .Build();

            return this;
        }

        public IScheduleJob Schedule(IJobDetail jobDetail)
        {
            _scheduler.ScheduleJob(jobDetail, _trigger);

            return this;
        }

        public IScheduleJob PauseAll()
        {
            _scheduler.PauseAll();

            return this;
        }

        public IScheduleJob Shutdown()
        {
            _scheduler.Shutdown();

            return this;
        }

        public IScheduleJob Start()
        {
            _scheduler.Start();

            return this;
        }
    }
}