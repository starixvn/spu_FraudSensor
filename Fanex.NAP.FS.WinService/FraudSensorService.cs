namespace Fanex.NAP.FS.WinService
{
    using Logging;
    using MailDelivery.Client;
    using System.Configuration;
    using System.ServiceProcess;
    using System.Threading;
    using System.Threading.Tasks;

    public partial class FraudSensorService : ServiceBase
    {
        private IFraudCaseLoader _fraudCaseLoader;

        private IReceivingMessageHandler _receivingMessageHandler;

        private Thread _thread;

        public FraudSensorService(IFraudCaseLoader fraudCaseLoader = null, IReceivingMessageHandler receivingMessageHandler = null)
        {
            _fraudCaseLoader = fraudCaseLoader ?? new FraudCaseLoader();

            _receivingMessageHandler = receivingMessageHandler ?? new ReceivingMessageHandler();

            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _thread = new Thread(RunOnStart);
            _thread.Start();
        }

        protected override void OnStop()
        {
            Logger.Log.Info("SPU FS 2 Service Stopped");

            _receivingMessageHandler.ClearCases();

            SendMailInform(false);

            _thread.Abort();
        }

        protected override void OnShutdown()
        {
            SendMailInform(false);

            Logger.Log.Info("SPU FS 2 Service Stopped");

            _receivingMessageHandler.ClearCases();

            MailClientHandler.Stop();
        }

        public void RunOnStart()
        {
            try
            {
                Logger.Log.Info("SPU FS 2 Service Started");

                if (AppSettings.Environment != "1")
                {
                    MailClientHandler.Start();

                    var queue = new MessageQueueReceiver();
                    queue.Clear();
                }

                Logger.Log.Info("SPU FS 2 Service - Loading fraudcases");

                _fraudCaseLoader.LoadFraudCases();

                _receivingMessageHandler.TriggerReceivingMessage();

                if (AppSettings.Environment != "1")
                {
                    SendMailInform(true);
                }

                Logger.Log.Info("SPU FS 2 Service - Loading fraudcases have done.");
            }
            catch (System.Exception ex)
            {
                Logger.Log.Error(ex.Message, ex);
            }
        }

        private void SendMailInform(bool serviceStarting)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    var subject = "SPU - New FS Service Started";
                    if (!serviceStarting)
                    {
                        subject = "SPU - New FS Service Stopped";
                    }

                    var newMail = new Fanex.MailDelivery.Core.Models.Mail
                    {
                        Subject = subject,
                        Title = "SPU Fraud Sensor",
                        Body = subject,
                        MailingListId = int.Parse(ConfigurationManager.AppSettings["MailingListId"])
                    };

                    MailClientHandler.SendFunctionMail(newMail);
                }
                catch
                {
                    ////ignore comment
                }
            });
        }
    }
}