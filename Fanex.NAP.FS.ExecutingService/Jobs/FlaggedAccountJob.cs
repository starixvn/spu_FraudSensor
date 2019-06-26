using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.FS.ExecutingService.Jobs
{
    public  class FlaggedAccountJob : IJob
    {
        private static readonly Random Random = new Random();

        public async Task Execute(IJobExecutionContext context)
        {
            Debug.WriteLine("Flagged Account Job > " + DateTime.Now);

            try
            {
                var rootPath = @"D:\Projects\fraudsensor\Fanex.NAP.FS.FlaggedAccountLibrary\bin\Debug\";

                Assembly assembly = Assembly.LoadFrom(rootPath + "Fanex.NAP.FS.FlaggedAccountLibrary.dll");
                Type type = assembly.GetType("Fanex.NAP.FS.FlaggedAccountLibrary.Program");
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