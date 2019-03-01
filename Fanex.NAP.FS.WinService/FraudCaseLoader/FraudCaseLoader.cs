namespace Fanex.NAP.FS.WinService
{
    using Fanex.Logging;
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
            var fraudCases = _fraudCaseService.Get(null, 1, int.Parse(ConfigurationManager.AppSettings["Environment"]));

            foreach (var fraudCase in fraudCases)
            {              
                if (fraudCase.IsActive)
                {
                    _sendingCase.Send(fraudCase.FraudCaseId);                

                    //Logger.Log.Info("FS Service - " + fraudCase.FraudCaseId + " - " + fraudCase.FraudCaseName);
                }
            }
        }
    }
}