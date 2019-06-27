namespace Fanex.NAP.FS.ExecutingService.Jobs
{
    using Quartz;

    public class DailyDataCaptureJob : Jobs
    {
        public override string GetFraudCase() => "DailyDataCapture";

        public override IJobDetail CreateJob(string name, string groupName, string description)
        {
            return JobBuilder.Create<DailyDataCaptureJob>()
                               .WithIdentity(name, groupName)
                               .WithDescription(description)
                               .StoreDurably()
                               .Build();
        }
    }
}