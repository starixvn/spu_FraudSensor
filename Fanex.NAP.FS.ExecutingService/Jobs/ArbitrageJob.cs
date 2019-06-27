namespace Fanex.NAP.FS.ExecutingService.Jobs
{
    using Quartz;
    public class ArbitrageJob : Jobs
    {
        public override string GetFraudCase() => "Arbitrage";

        public override IJobDetail CreateJob(string name, string groupName, string description)
        {
            return JobBuilder.Create<ArbitrageJob>()
                               .WithIdentity(name, groupName)
                               .WithDescription(description)
                               .StoreDurably()
                               .Build();
        }
    }
}