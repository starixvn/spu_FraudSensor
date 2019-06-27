namespace Fanex.NAP.FS.ExecutingService.Jobs
{
    using Quartz;
    public class FlaggedAccountJob : Jobs
    {
        public override string GetFraudCase() => "FlaggedAccount";

        public override IJobDetail CreateJob(string name, string groupName, string description)
        {
            return JobBuilder.Create<FlaggedAccountJob>()
                               .WithIdentity(name, groupName)
                               .WithDescription(description)
                               .StoreDurably()
                               .Build();
        }
    }
}