namespace Fanex.NAP.FS.ExecutingService.Jobs
{
    using Fanex.NAP.FS.ExecutingService.Schedules;    
    using Quartz;

    public class FlaggedAcocuntSchedule : Schedule
    {
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
