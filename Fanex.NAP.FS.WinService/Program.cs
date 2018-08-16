using Fanex.Data;
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
                   .UseDefaultDbSettingProvider(resourcePath: environment == "1" ? "~/App_Data": "~/App_Data/UAT", enableWatching: true)
                   .Run();

                ServiceBase[] ServicesToRun;

                ServicesToRun = new ServiceBase[]
                {
                    new FraudSensorService()
                };

                ServiceBase.Run(ServicesToRun);
            }
            catch (System.Exception ex)
            {
                Logging.Logger.Log.Error(ex.Message, ex);
            }
        }
    }
}