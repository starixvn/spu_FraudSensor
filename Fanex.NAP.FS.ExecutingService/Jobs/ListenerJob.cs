namespace Fanex.NAP.FS.ExecutingService.Jobs
{
    using System.Threading;
    using System.Threading.Tasks;
    using Quartz;
    using Fanex.Logging;
    using System;

    public class ListenerJob : IJobListener
    {
        private readonly string _jobName;
        public string Name => _jobName;


        public ListenerJob(string jobName)
        {
            _jobName = jobName;
        }

        public Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            var logMessage = context.JobDetail.Key + " - " + context.JobDetail.Description;
            
            LogService.Info($"Executing service - ListenerJob JobExecutionVetoed message: { logMessage }");

            return Task.FromResult(true);
        }

        public Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(true);
        }

        public Task JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (jobException == null)
            {
                return Task.FromResult(true);
            }

            var logMessage = context.JobDetail.Key + " - " + jobException.Message;
            
            LogService.Info($"Executing service - ListenerJob JobWasExecuted message: { logMessage }");

            Console.WriteLine(logMessage);

            return Task.FromResult(false);
        }
    }
}