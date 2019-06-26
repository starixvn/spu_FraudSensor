namespace Fanex.NAP.FS.ExecutingService.Jobs
{
    using Fanex.NAP.FS.Core;
    using Quartz;
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using System.Threading.Tasks;

    public  class FlaggedAccountJob : IJob
    {
        private static readonly Random Random = new Random();

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                var rootPath = string.Format(AppSetting.FraudCaseConsoleRootPath, "FlaggedAccount");
                var assemblyName = string.Format(AppSetting.FraudCaseConsoleAssembly, "FlaggedAccount");
                var assembly = Assembly.LoadFrom(rootPath + assemblyName);

                var type = assembly.GetType(string.Format(AppSetting.FraudCaseConsoleType, "FlaggedAccount"));
                var methodInfo = type.GetMethod("Start");
                var classInstance = Activator.CreateInstance(type, null);

                methodInfo.Invoke(classInstance, new object[] { });
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }

            await Task.Delay(TimeSpan.FromSeconds(Random.Next(1, 10)));
        }
    }
}