namespace Fanex.NAP.FS.WinService
{
    using Library.FSMQ;
    using Library.Services;
    using System.Configuration;

    public class FraudCaseLoader : IFraudCaseLoader
    {
        private readonly ISendingMessage _sendingCase;

        private readonly IFraudCaseService _fraudCaseService;

        public FraudCaseLoader(IMessageQueueSender messageQueue = null)
        {
            var queue = messageQueue ?? new MessageQueueSender();
            _sendingCase = new AddingCase(queue);
            _fraudCaseService = new FraudCaseService();
        }

        public void LoadFraudCases()
        {
            //var fraudCases = _fraudCaseService.Get(null, 1, int.Parse(ConfigurationManager.AppSettings["Environment"]));

            Logging.Logger.Log.Info($"fraudcase: Arbitrage - active: Active");

            _sendingCase.Send(10);
            /*
            foreach (var fraudCase in fraudCases)
            {
                Logging.Logger.Log.Info($"fraudcase: {fraudCase.FraudCaseName} - active: {fraudCase.IsActive}");

                if (fraudCase.IsActive)
                {
                    _sendingCase.Send(fraudCase.FraudCaseId);
                }
            }*/
        }
    }
}