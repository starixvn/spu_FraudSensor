namespace Fanex.NAP.FS.WinService
{
    using Logging;
    using MailDelivery.Client;
    using System;
    using System.Configuration;
    using System.ServiceProcess;
    using System.Threading.Tasks;
    using System.Timers;

    public partial class FraudSensorService : ServiceBase
    {
        private IFraudCaseLoader _fraudCaseLoader;
        private IReceivingMessageHandler _receivingMessageHandler;
        private System.Timers.Timer timer;

        public FraudSensorService(IFraudCaseLoader fraudCaseLoader = null, IReceivingMessageHandler receivingMessageHandler = null)
        {
            InitializeComponent();

            _fraudCaseLoader = fraudCaseLoader ?? new FraudCaseLoader();

            _receivingMessageHandler = receivingMessageHandler ?? new ReceivingMessageHandler();

            timer = new System.Timers.Timer();            
        }

        protected override void OnStart(string[] args)
        {
            timer.Elapsed += RunOnStart;
            timer.Interval = 10000;
            timer.Start();

            SendMailInform("SPU - New FPS - Service is starting. Waiting for 10s for starting. Now is " + DateTime.Now);            
        }

        protected override void OnStop()
        {
            Logger.Log.Info("SPU FS 2 Service Stopped");

            _receivingMessageHandler.ClearCases();

            timer.Dispose();

            SendMailInform("SPU - New FS Service Stopped. Now is " + DateTime.Now);
        }

        protected override void OnShutdown()
        {
            SendMailInform("SPU - New FS - Server is rebooting. Now is " + DateTime.Now);

            Logger.Log.Info("SPU FS 2 Service Stopped");

            _receivingMessageHandler.ClearCases();

            MailClientHandler.Stop();

            timer.Dispose();
        }

        public void RunOnStart(object source, ElapsedEventArgs e)       
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

                //_fraudCaseLoader.LoadFraudCases();

                //_receivingMessageHandler.TriggerReceivingMessage();

                if (AppSettings.Environment != "1")
                {
                    SendMailInform("SPU - New FS Service Started. Now is " + DateTime.Now);
                }

                timer.Stop();               

                Logger.Log.Info("SPU FS 2 Service - Loading fraudcases have done.");
            }
            catch (System.Exception ex)
            {
                Logger.Log.Error(ex.Message, ex);
            }
        }

        private void SendMailInform(string subject)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    var newMail = new Fanex.MailDelivery.Core.Models.Mail
                    {
                        Subject = subject,
                        Title = "SPU Fraud Sensor",
                        Body = subject,
                        MailingListId = int.Parse(ConfigurationManager.AppSettings["MailingListId"])
                    };

                    MailClientHandler.SendFunctionMail(newMail);
                }
                catch (System.Exception ex)
                {
                    Logger.Log.Error(ex.Message, ex);
                }
            });
        }
    }
}