namespace Fanex.NAP.FS.ExecutingService.Schedules
{
    using Quartz;

    public class SchedulesFS : ISchedulesFS
    {
        private readonly IScheduler _scheduler;
        private ITrigger _trigger;

        public SchedulesFS(IScheduler scheduler)
        {
            _scheduler = scheduler;
        }

        public ISchedulesFS CreateTrigger(string name, int interval)
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

        public ISchedulesFS Schedule(IJobDetail jobDetail)
        {
            _scheduler.ScheduleJob(jobDetail, _trigger);

            return this;
        }

        public ISchedulesFS PauseAll()
        {
            _scheduler.PauseAll();

            return this;
        }

        public ISchedulesFS Shutdown()
        {
            _scheduler.Shutdown();

            return this;
        }

        public ISchedulesFS Start()
        {
            _scheduler.Start();

            return this;
        }
    }
}