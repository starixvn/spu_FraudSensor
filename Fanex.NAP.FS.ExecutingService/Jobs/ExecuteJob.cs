namespace Fanex.NAP.FS.ExecutingService.Jobs
{
    using Fanex.NAP.FS.Core;
    using Fanex.NAP.FS.Model;
    using Quartz;
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
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
                Debug.WriteLine(ex.ToString());
            }

            return Task.FromResult(true);
        }
    }
}
