namespace Fanex.NAP.FS.ExecutingService.Jobs
{
    using Fanex.Logging;
    using Fanex.NAP.FS.Core;
    using Fanex.NAP.FS.Service;
    using Quartz;
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;

    [PersistJobDataAfterExecution]
    [DisallowConcurrentExecution]
    public class ExecuteJob : IJob
    {
        private const string FlagNotExistsFile = "FlagNotExistsFile";

        public Task Execute(IJobExecutionContext context)
        {
            try
            {
                var instance = context.JobDetail.JobDataMap.GetString("JobData");
                if (!Ultilities.CheckFileExist(instance))
                {
                    var msg = $"{context.JobDetail.Key} - Execute: does not exists a file - Path: {instance}";
                    LogService.Error(msg);

                    if (!context.JobDetail.JobDataMap.Contains(FlagNotExistsFile))
                    {
                        new LetstalkService().Error(msg);
                        context.JobDetail.JobDataMap.Put(FlagNotExistsFile, true);
                    }                    

                    return Task.FromResult(false);
                }

                var process = new Process();
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
