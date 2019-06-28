namespace Fanex.NAP.FS.CheckBet.Console
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.FS.Service;
    using System;

    public class Service : BaseService
    {
        private readonly FS.Service.CheckBetService _service;       

        public Service() : base(Library.Models.FraudCaseId.BSNG)
        {
            _service = new FS.Service.CheckBetService();          
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

                LogService.Error($"Check Bet service - Execute Exeption {logMessage}");
            }
        }
    }
}