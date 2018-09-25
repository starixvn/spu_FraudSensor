using Fanex.Data;
using Fanex.NAP.FS.Library.Services;
using Fanex.Logging;
using System;
using System.Configuration;
using System.ServiceProcess;

namespace Fanex.NAP.FS.WinService
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        private static void Main()
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
                    new FraudSensorService(
                        new FraudCaseManager(new FraudCaseService()), 
                        new Messager(new MessageQueueReceiver()))
                };

                ServiceBase.Run(ServicesToRun);
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