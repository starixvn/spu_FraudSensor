namespace Fanex.NAP.FS.ExecutingService.Jobs
{
    using Fanex.NAP.FS.ExecutingService.Schedules;    
    using Quartz;

    public class FlaggedAcocuntSchedule : Schedule
    {
        private IScheduler scheduler;
        public override ISchedule Create()
        {
            //NameValueCollection props = new NameValueCollection
            //    {
            //        { "quartz.serializer.type", "binary" }
            //    };

            //var sf = new StdSchedulerFactory(props);
            //scheduler = sf.GetScheduler().Result;
            
            //var job = JobBuilder.Create<Job>()
            //  .WithIdentity("FlaggedAccountJob", "FSGroup")
            //  .Build();

            //var trigger = TriggerBuilder.Create()
            //    .WithIdentity("FlaggedAccountTrigger", "FSGroup")
            //    .StartNow()
            //    .WithSimpleSchedule(x => x
            //        .WithIntervalInSeconds(10)
            //        .RepeatForever())
            //    .Build();

            //scheduler.ScheduleJob(job, trigger);

            return this;
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
    }
}
