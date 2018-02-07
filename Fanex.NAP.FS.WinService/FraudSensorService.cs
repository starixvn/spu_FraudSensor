namespace Fanex.NAP.FS.WinService
{
    using Fanex.Data;
    using Fanex.Logging;
    using Fanex.MailDelivery.Client;
    using System.Configuration;
    using System.ServiceProcess;

    public partial class FraudSensorService : ServiceBase
    {
        private ITaskLoader _taskLoader;

        private IReceivingMessageHandler _receivingMessageHandler;

        public FraudSensorService(ITaskLoader taskLoader = null, IReceivingMessageHandler receivingMessageHandler = null)
        {
            DbSettingProviderManager
               .StartNewSession()
               .UseDefaultConnectionStringProvider()
               .UseDefaultDbSettingProvider(resourcePath: "~/App_Data", enableWatching: true)
               .Run();

            _taskLoader = taskLoader ?? new TaskLoader();

            _receivingMessageHandler = receivingMessageHandler ?? new ReceivingMessageHandler();

            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            RunOnStart();
        }

        protected override void OnStop()
        {
            Logger.Log.Info("SPU FS Service Stopped");

            _receivingMessageHandler.ClearTasks();

            SendMailInform(false);
        }

        public void RunOnStart()
        {
            Logger.Log.Info("SPU FS Service Started");

            MailClientHandler.Start();

            var queue = new MessageQueueReceiver();
            queue.Clear();

            _taskLoader.LoadTasks();

            _receivingMessageHandler.TriggerReceivingMessage();

            SendMailInform(true);

            Logger.Log.Info("Loading tasks has done.");
        }

        private void SendMailInform(bool serviceStarting)
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
                    Title = subject,
                    Body = subject,
                    MailingListId = int.Parse(ConfigurationManager.AppSettings["MailingListId"])
                };

                MailClientHandler.SendFunctionMail(newMail);
            }
            catch
            {
                ////ignore comment
            }
        }
    }
}
