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
            ServiceBase[] ServicesToRun;

            ServicesToRun = new ServiceBase[]
            {
                new FraudSensorService()
            };

            ServiceBase.Run(ServicesToRun);
        }
    }
}