using Fanex.Logging;
using System.Collections.Generic;
using System.Web.Http;

namespace Fanex.NAP.FS.MainService
{
    public class HealthCheckController : ApiController
    {
        [HttpPost]        
        public void Beat(string serviceName, bool status, string exception)
        {
            if (!status)
            {
                string message = $"Service name: {serviceName} - Status: {status} - Exception: {exception}";
#if DEBUG
                System.Console.WriteLine(message);
#endif
                LogService.Error(message);
            }
        }

        [HttpGet]
        public string Testing()
        {
            return "I'm here. Everything is ok now.";
        }
    }
}
