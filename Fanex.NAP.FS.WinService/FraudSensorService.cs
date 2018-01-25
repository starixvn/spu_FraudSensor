namespace Fanex.NAP.FS.WinService
{
    using Fanex.Logging;
    using System.ServiceProcess;

    public partial class FraudSensorService : ServiceBase
    {
        private ITaskLoader _taskLoader;

        private IReceivingMessageHandler _receivingMessageHandler;

        public FraudSensorService(ITaskLoader taskLoader = null, IReceivingMessageHandler receivingMessageHandler = null)
        {
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
        }

        public void RunOnStart()
        {
            Logger.Log.Info("SPU FS Service Started");

            var queue = new MessageQueueReceiver();
            queue.Clear();

            _taskLoader.LoadTasks();

            _receivingMessageHandler.TriggerReceivingMessage();

            Logger.Log.Info("Loading tasks has done.");
        }
    }
}
