namespace Fanex.NAP.FS.ExecutingService.Jobs
{
    using Fanex.NAP.FS.Core;
    using Quartz;
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;

    [PersistJobDataAfterExecution]
    [DisallowConcurrentExecution]
    public class ExecuteJob : IJob
    {
        private const string NotExistsFileConsole = "Does not exists a file console";

        public Task Execute(IJobExecutionContext context)
        {
            try
            {
                var process = new Process();
                var instance = context.JobDetail.JobDataMap.GetString("JobData");

                if (!Ultilities.CheckFileExist(instance))
                {
                    throw new JobExecutionException(NotExistsFileConsole);
                }

                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.FileName = instance;
                process.Start();
                
                var errorMessage = process.StandardError.ReadToEnd();

                process.WaitForExit();

                if (!string.IsNullOrWhiteSpace(errorMessage))
                {
                    throw new JobExecutionException(errorMessage);
                }
            }

            catch (Exception ex)
            {
                throw new JobExecutionException(ex);
            }

            return Task.FromResult(true);
        }
    }
}
