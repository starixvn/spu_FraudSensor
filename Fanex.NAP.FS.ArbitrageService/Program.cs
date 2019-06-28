using Fanex.Data;
using Fanex.Data.ConnectionStringProvider20;
using Fanex.Logging;
using Fanex.Logging.Sentry.RavenClient;
using Fanex.NAP.FS.Core;
using SharpRaven;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.ServiceProcess;

namespace Fanex.NAP.FS.ArbitrageService
{
    internal static class Program
    {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        public static void Main(string[] args)
        {
            DbSettingProviderManager
                .StartNewSession()
                .Use(new ConnectionStringProvider20())
                .UseDefaultDbSettingProvider(AppSetting.AppData, false)
                .Run();

            LogManager
                .SetDefaultLogCategory(AppSetting.FanexLogCategory)
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

            if (Environment.UserInteractive)
            {
                // run console
                var type = typeof(ServiceBase);
                const BindingFlags Flags = BindingFlags.Instance | BindingFlags.NonPublic;
                var method = type.GetMethod("OnStart", Flags);

                foreach (var service in ServicesToRun) method.Invoke(service, new object[] { args });

                Console.WriteLine("Press any key to exit!");
                Console.ReadLine();

                foreach (var service in ServicesToRun) service.Stop();
            }
            else
            {
                ServiceBase.Run(ServicesToRun);
            }
        }
    }
}