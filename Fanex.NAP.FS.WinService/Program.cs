using Fanex.Data;
using Fanex.NAP.FS.Library.Services;
using Fanex.Logging;
using System;
using System.Configuration;
using System.ServiceProcess;
using System.Reflection;

namespace Fanex.NAP.FS.WinService
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        private static void Main(string[] args)
        {
            try
            {
                var environment = ConfigurationManager.AppSettings["Environment"];

                DbSettingProviderManager
                   .StartNewSession()
                   .UseDefaultConnectionStringProvider()
                   .UseDefaultDbSettingProvider(resourcePath: environment != "2" ? "~/App_Data" : "~/App_Data/UAT", enableWatching: true)
                   .Run();

                ServiceBase[] ServicesToRun;

                ServicesToRun = new ServiceBase[]
                {
                    new FraudSensorService()
                };

                if (Environment.UserInteractive)
                {
                    // run console
                    var type = typeof(ServiceBase);
                    const BindingFlags Flags = BindingFlags.Instance | BindingFlags.NonPublic;
                    var method = type.GetMethod("OnStart", Flags);

                    foreach (var service in ServicesToRun)
                    {
                        method.Invoke(service, new object[] { args });
                    }

                    Console.WriteLine("Press any key to exit!");
                    Console.ReadLine();

                    foreach (var service in ServicesToRun)
                    {
                        service.Stop();
                    }
                }
                else
                { 
                    ServiceBase.Run(ServicesToRun);
                }
            }
            catch (System.Exception ex)
            {
                Logging.Logger.Log.Error(ex.Message, ex);
            }
        }

        private static void LogException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = (Exception)e.ExceptionObject;

            Logger.Log.Error($"{sender.GetType().FullName}", exception);
        }
    }
}