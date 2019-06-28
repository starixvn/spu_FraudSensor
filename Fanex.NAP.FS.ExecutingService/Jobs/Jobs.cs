namespace Fanex.NAP.FS.ExecutingService.Jobs
{
    using Fanex.NAP.FS.Core;
    using Quartz;
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using System.Threading.Tasks;

    [DisallowConcurrentExecution]
    public abstract class Jobs : IJobs
    {
        private const string Method = "Start";
        public abstract string GetFraudCase();

        public abstract IJobDetail CreateJob(string name, string groupName, string description);

        public Task Execute(IJobExecutionContext context)
        {
            try
            {
                string fraudCase = GetFraudCase();

                var rootPath = string.Format(AppSetting.FraudCaseConsoleRootPath, fraudCase);
                var assemblyName = string.Format(AppSetting.FraudCaseConsoleAssembly, fraudCase);
                var assembly = Assembly.LoadFrom(rootPath + assemblyName);

                var type = assembly.GetType(string.Format(AppSetting.FraudCaseConsoleType, fraudCase));
                var methodInfo = type.GetMethod(Method);

                methodInfo.Invoke(null, new object[] { });
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }

            return Task.FromResult(true);
        }
    }
}