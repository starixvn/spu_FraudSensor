namespace Fanex.NAP.FS.Arbitrage.Console
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.Logging.Sentry.RavenClient;
    using Fanex.NAP.FS.Core;
    using SharpRaven;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    static class Program
    {
        static void Main(string[] args)
        {
            try
            {
                LogManager
                    .SetDefaultLogCategory(AppSetting.FanexLogCategory)
                    .Use(new List<ILogEngine>
                    {
                        new SentryLogging(new SentryEngineOptions
                        {
                            Dsn = new Dsn(AppSetting.SentryLogging)
                        })
                    });

                var _service = new Service();

                if (_service.CheckUM())
                {
                    return;
                }

                _service
                    .CleanResult()
                    .Precondition()
                    .Rule();

                Thread.Sleep(600000);
                
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                     .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                     .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error($"Arbitrage service - Start Exeption {logMessage}");
            }
        }
    }
}
