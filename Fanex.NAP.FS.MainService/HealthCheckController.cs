using Fanex.Logging;
using System.Web.Http;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace Fanex.NAP.FS.MainService
{
    public class HealthCheckController : ApiController
    {
        private static List<Thread> threadsMonitor = new List<Thread>();
        [HttpPost]
        public void Beat(string serviceName, bool status, string exception, int timeout)
        {
            var thread = threadsMonitor.Where(t => t.Name == serviceName).FirstOrDefault();
            if (thread != null)
            {
                threadsMonitor.Remove(thread);
                thread.Abort();
            }

            thread = new Thread(() =>
            {
                if (!status)
                {
                    string message = $"Service name: {serviceName} - Status: {status} - Exception: {exception}";
#if DEBUG
                        System.Console.WriteLine(message);
#endif
                        LogService.Error(message);
                }

                Thread.Sleep((timeout + 5) * 1000);

                var errorMessage = $"Service name: {serviceName} - Do not receive heart beat in " + timeout + " seconds";
                LogService.Error(errorMessage);

            });
            thread.Name = serviceName;
            threadsMonitor.Add(thread);

        }

        [HttpGet]
        public string Testing()
        {
            return "I'm here. Everything is ok now.";
        }
    }
}
