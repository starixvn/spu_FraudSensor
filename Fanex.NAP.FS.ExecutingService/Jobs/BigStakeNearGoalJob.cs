namespace Fanex.NAP.FS.ExecutingService.Jobs
{
    using Quartz;
    public class BigStakeNearGoalJob : Jobs
    {
        public override string GetFraudCase() => "BigStakeNearGoal";

        public override IJobDetail CreateJob(string name, string groupName, string description)
        {
            return JobBuilder.Create<BigStakeNearGoalJob>()
                               .WithIdentity(name, groupName)
                               .WithDescription(description)
                               .StoreDurably()
                               .Build();
        }
    }
}