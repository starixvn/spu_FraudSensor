namespace Fanex.NAP.FS.WinService
{
    using Fanex.NAP.FS.Library.FSMQ;

    public class TaskLoader : ITaskLoader
    {
        private readonly ISendingMessage _sendingTask;

        public TaskLoader()
        {
            _sendingTask = new AddingTask(new MessageQueueSender());
        }

        public TaskLoader(IMessageQueueSender messageQueue)
        {
            _sendingTask = new AddingTask(messageQueue);
        }

        public void LoadTasks()
        {
            ////??
        }
    }
}