namespace Fanex.NAP.FS.ExecutingService.Jobs
{
    using Quartz;
    public class CheckBetJob : Jobs
    {
        public override string GetFraudCase() => "CheckBet";

        public override IJobDetail CreateJob(string name, string groupName, string description)
        {
            return JobBuilder.Create<CheckBetJob>()
                               .WithIdentity(name, groupName)
                               .WithDescription(description)
                               .StoreDurably()
                               .Build();
        }
    }
}
