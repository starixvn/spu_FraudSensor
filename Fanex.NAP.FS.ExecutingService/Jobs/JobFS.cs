namespace Fanex.NAP.FS.ExecutingService.Jobs
{
    using Quartz;
    using Quartz.Impl;

    [DisallowConcurrentExecution]
    public class JobFS : IJobFS
    {
        public IJobDetail Job { get; set; }

        public ITrigger Trigger { get; set; }

        public IScheduler Schedule { get; set; }

        public IJobFS CreateJob(string className, string jobName, string groupName)
        {
            var jobDetail = new JobDetailImpl(className, typeof(ExecuteJob));

            var job = JobBuilder.Create(jobDetail.JobType)
               .WithIdentity(jobName, groupName)
               .Build();

            this.Job = job;

            return this;
        }       

        public IJobFS CreateTrigger(string triggerName, int interval)
        {
            var trigger = TriggerBuilder.Create()
            .WithIdentity(triggerName)
            .StartNow()
            .WithSimpleSchedule(x => x
                .WithIntervalInSeconds(interval)
                .RepeatForever())
            .Build();

            this.Trigger = trigger;

            return this;
        }

        public IJobFS CreateSchedule()
        {
            this.Schedule = StdSchedulerFactory.GetDefaultScheduler().Result;

            this.Schedule.ScheduleJob(this.Job, this.Trigger);

            return this;
        }

        public void Run()
        {
            this.Schedule.Start();
        }
    }
}