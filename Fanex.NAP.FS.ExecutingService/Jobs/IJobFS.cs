using Fanex.NAP.FS.Model;

namespace Fanex.NAP.FS.ExecutingService.Jobs
{
    public interface IJobFS
    {
        IJobFS CreateJob(string className, string jobName, string groupName, JobDataExecutingModel jobDataExecutingModel);

        IJobFS CreateTrigger(string triggerName, int interval);

        IJobFS CreateSchedule();

        void Run();
    }
}