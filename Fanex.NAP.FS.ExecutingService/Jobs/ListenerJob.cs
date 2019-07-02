namespace Fanex.NAP.FS.ExecutingService.Jobs
{
    using System.Threading;
    using System.Threading.Tasks;
    using Quartz;
    using Fanex.Logging;
    using System;
    using Fanex.NAP.FS.Service;

    public class ListenerJob : IJobListener
    {
        private const string TriesKey = "TriesKey";
        private const int MaxRetries = 3;
        private const string NotExistsFileConsole = "Does not exists a file console";
        private readonly string _jobName;
        private readonly LetstalkService _letstalkService;

        public string Name => _jobName;


        public ListenerJob(string jobName)
        {
            _jobName = jobName;
            _letstalkService = new LetstalkService();
        }

        public Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            LogService.Error($"{context.JobDetail.Key} - JobExecutionVetoed - Job is Vetoed.");

            return Task.FromResult(true);
        }

        public Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!context.JobDetail.JobDataMap.Contains(TriesKey))
            {
                context.JobDetail.JobDataMap.Put(TriesKey, 0);
            }

            var numberTries = context.JobDetail.JobDataMap.GetIntValue(TriesKey);
            context.JobDetail.JobDataMap.Put(TriesKey, ++numberTries);

            return Task.FromResult(true);
        }

        public Task JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (jobException == null)
            {
                return Task.FromResult(true);
            }

            var errorMessage = $"{context.JobDetail.Key} - JobWasExecuted - Exception: { jobException.Message }";
            LogService.Error(errorMessage);
            
            if (jobException.Message.Equals(NotExistsFileConsole) && !context.JobDetail.JobDataMap.Contains(NotExistsFileConsole))
            {
                context.JobDetail.JobDataMap.Put(NotExistsFileConsole, 1);
                _letstalkService.Error(errorMessage);
                
                return Task.FromResult(true);
            }

            var numTries = context.JobDetail.JobDataMap.GetIntValue(TriesKey);
            if (numTries < MaxRetries)
            {
                RescheduleJob(context);

                return Task.FromResult(false);
            }

            _letstalkService.Error(errorMessage);

            return Task.FromResult(false);
        }

        private void RescheduleJob(IJobExecutionContext context)
        {
            var trigger = context.Trigger.GetTriggerBuilder()
                .StartAt(DateTime.Now.AddSeconds(15))
                .Build();

            context.Scheduler.RescheduleJob(context.Trigger.Key, trigger);
        }
    }
}