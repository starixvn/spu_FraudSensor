namespace Fanex.NAP.FS.ExecutingService.Jobs
{
    using Quartz;
    public class BetNearGoalJob : Jobs
    {
        public override string GetFraudCase() => "BetNearGoal";

        public override IJobDetail CreateJob(string name, string groupName, string description)
        {
            return JobBuilder.Create<BetNearGoalJob>()
                               .WithIdentity(name, groupName)
                               .WithDescription(description)
                               .StoreDurably()
                               .Build();
        }
    }
}