using Fanex.Data;
using Fanex.Data.ConnectionStringProvider20;
using Fanex.Logging;
using Fanex.Logging.Sentry;
using Fanex.NAP.FS.Core;
using Sentry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.FS.ArbitrageService2
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            DbSettingProviderManager
                   .StartNewSession()
                   .Use(new ConnectionStringProvider20())
                   .UseDefaultDbSettingProvider("~/App_Data", false)
                   .Run();

            LogManager
                     .SetDefaultLogCategory(AppSetting.FanexLogCategory)
                     .Use(new List<ILogEngine>
                     {
                         new SentryLogging(new SentryEngineOptions
                         {
                             Dsn = new Dsn("https://14293c00a15d46cbbd3ef5ca909e160d@sentry.nexdev.net/27")
                         })
                     });

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new Runner()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
