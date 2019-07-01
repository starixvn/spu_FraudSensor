namespace Fanex.NAP.FS.ExecutingService.Jobs
{
    using Fanex.NAP.FS.Core;
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
                string fraudCase = "";
                Process process = new Process();

                var rootPath = string.Format(AppSetting.FraudCaseConsoleRootPath, fraudCase);
                var instance = string.Format(AppSetting.FraudCaseConsoleInstance, fraudCase);

                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.FileName = rootPath + instance;
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
