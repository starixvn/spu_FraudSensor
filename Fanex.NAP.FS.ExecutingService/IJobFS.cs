using Fanex.NAP.FS.Model;

namespace Fanex.NAP.FS.ExecutingService
{
    public interface IJobFS
    {
        IJobFS CreateJob(string className, string jobName, string groupName, string jobData);

        IJobFS CreateTrigger(string triggerName, int interval);

        IJobFS CreateSchedule();

        void Run();
    }
}