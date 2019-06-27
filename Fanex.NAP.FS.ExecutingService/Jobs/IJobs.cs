namespace Fanex.NAP.FS.ExecutingService.Jobs
{
    using Quartz;
    public interface IJobs : IJob
    {
        string GetFraudCase();

        IJobDetail CreateJob(string name, string groupName, string description);
        
    }
}
