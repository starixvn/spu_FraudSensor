namespace Fanex.NAP.FS.ExecutingService.Jobs
{   
    using Quartz;
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;

    [PersistJobDataAfterExecution]
    [DisallowConcurrentExecution]
    public class ExecuteJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            try
            {
                Process process = new Process();
                var instance = context.JobDetail.JobDataMap.GetString("JobData");

                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.FileName = instance;
                process.Start();

                process.StandardOutput.ReadToEnd();
                process.WaitForExit();
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                JobExecutionException jex = new JobExecutionException(ex);
                throw jex;
            }

            return Task.FromResult(true);
        }
    }
}
