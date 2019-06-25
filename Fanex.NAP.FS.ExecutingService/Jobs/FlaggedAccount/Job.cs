using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.FS.ExecutingService.Jobs.FlaggedAccount
{
    public  class Job : IJob
    {
        private static readonly Random Random = new Random();

        public async Task Execute(IJobExecutionContext context)
        {
#if DEBUG
            Console.WriteLine("Flagged Account Job > " + DateTime.Now);
#endif  
            try
            {
                var rootPath = @"D:\Projects\fraudsensor\Fanex.NAP.FS.FlaggedAccountService\bin\Debug\\";

                Assembly assembly = Assembly.LoadFrom(rootPath + "Fanex.NAP.FS.FlaggedAccountService.exe");
                Type type = assembly.GetType("Fanex.NAP.FS.FlaggedAccountService.Runner");
                MethodInfo methodInfo = type.GetMethod("Start");

                var classInstance = Activator.CreateInstance(type, null);

                methodInfo.Invoke(classInstance, new object[] { });
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            await Task.Delay(TimeSpan.FromSeconds(Random.Next(1, 20)));
        }
    }
}