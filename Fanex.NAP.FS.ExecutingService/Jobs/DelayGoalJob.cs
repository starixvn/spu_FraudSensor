namespace Fanex.NAP.FS.ExecutingService.Jobs
{
    using Quartz;
    public class DelayGoalJob : Jobs
    {
        public override string GetFraudCase() => "DelayGoal";

        public override IJobDetail CreateJob(string name, string groupName, string description)
        {
            return JobBuilder.Create<DelayGoalJob>()
                               .WithIdentity(name, groupName)
                               .WithDescription(description)
                               .StoreDurably()
                               .Build();
        }
    }
}