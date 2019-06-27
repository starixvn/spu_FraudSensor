namespace Fanex.NAP.FS.ExecutingService.Jobs
{
    using Fanex.NAP.FS.ExecutingService.Schedules;    
    using Quartz;
    using System;

    public class ScheduleJob : IScheduleJob, ISchedule
    {
        private IScheduler _scheduler;

        public ScheduleJob(IScheduler scheduler)
        {
            _scheduler = scheduler;
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
