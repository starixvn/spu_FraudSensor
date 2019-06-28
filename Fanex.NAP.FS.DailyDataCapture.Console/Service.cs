namespace Fanex.NAP.FS.DailyDataCapture.Console
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.FS.Service;
    using System;

    public class Service : BaseService
    {
        private readonly FS.Service.DailyDataCaptureService _service;

        public Service() : base(Library.Models.FraudCaseId.DailyDataCapture)
        {
            _service = new FS.Service.DailyDataCaptureService();
        }

        public override BaseService Rule()
        {
            Execute();

            return base.Rule();
        }

        private void Execute()
        {
            try
            {
                _service.Rule();
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                   .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                   .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error($"Daily Data Capture service - Execute Exeption {logMessage}");
            }
        }
    }
}