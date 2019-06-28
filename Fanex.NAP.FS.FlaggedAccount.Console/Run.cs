namespace Fanex.NAP.FS.FlaggedAccount.Console
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using System;

    public static class Run
    {
        public static void Start()
        {
            try
            {
                var _service = new Service();

                if (_service.CheckUM())
                {
                    return;
                }

                _service
                    .CleanResult()
                    .Precondition()
                    .Rule();
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                    .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                    .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error($"Flagged Account service - Start Exeption {logMessage}");
            }
        }
    }
}
