using Fanex.Data;
using Fanex.Data.ConnectionStringProvider20;
using Fanex.Logging;
using Fanex.Logging.Sentry.RavenClient;
using Fanex.NAP.FS.Core;
using SharpRaven;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.FS.DelayGoalService
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
                   .UseDefaultDbSettingProvider(AppSetting.AppData, false)
                   .Run();

            LogManager.SetDefaultLogCategory(AppSetting.FanexLogCategory)
                     .Use(new List<ILogEngine>
                     {
                         new SentryLogging(new SentryEngineOptions
                         {
                             Dsn = new Dsn(AppSetting.SentryLogging)
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
