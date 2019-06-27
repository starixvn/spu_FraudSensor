namespace Fanex.NAP.FS.ExecutingService.Jobs
{
    using Quartz;
    public class ScannedOfficeIPJob : Jobs
    {
        public override string GetFraudCase() => "ScannedOfficeIP";

        public override IJobDetail CreateJob(string name, string groupName, string description)
        {
            return JobBuilder.Create<ScannedOfficeIPJob>()
                               .WithIdentity(name, groupName)
                               .WithDescription(description)
                               .StoreDurably()
                               .Build();
        }
    }
}
