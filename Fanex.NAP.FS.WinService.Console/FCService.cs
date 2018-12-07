using Fanex.Logging;
using Fanex.NAP.FS.Library.DTO;
using Fanex.NAP.FS.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Fanex.NAP.FS.WinService.Console
{
    public interface IFraudCaseExecutor
    {
        void Execute();
    }

    public interface IFraudCaseManager
    {
        IEnumerable<FraudCase> GetCases();

        FraudCase GetCase(FraudCaseId id);

        IFraudCaseExecutor GetExecutor(FraudCase fraudCase);
    }

    public interface IMessageHandler
    {
        void ListenToMessage(Action<MessageType, FraudCaseId> handler);
    }

    public enum MessageType
    {
        Add = 0,
        Remove = 1,
        Deactivate = 2,
        Activate = 3
    }

    public class FCService
    {
        private IFraudCaseManager manager;

        private IMessageHandler messager;

        private IDictionary<FraudCaseId, Timer> threads;

        public void Execute()
        {
            threads = manager.GetCases().ToDictionary(f => (FraudCaseId)f.FraudCaseId, f => GetTimer(f));
            messager.ListenToMessage(HandleMsg);
        }

        public Timer GetTimer(FraudCase fraudCase)
        {
            var executor = manager.GetExecutor(fraudCase);

            var timer = new Timer(ExecuteFraudCase, executor, 0, fraudCase.FraudCaseInterval * 1000);

            return timer;
        }

        public static void ExecuteFraudCase(object executor)
        {
            try
            {
                (executor as IFraudCaseExecutor).Execute();
            }
            catch (Exception ex)
            {
                LogService.Error(ex.Message, ex);
            }
        }

        private void HandleMsg(MessageType message, FraudCaseId id)
        {
            switch (message)
            {
                case MessageType.Add:
                    if (!threads.ContainsKey(id))
                    {
                        var fc = manager.GetCase(id);
                        threads.Add(id, GetTimer(fc));
                    }

                    break;

                case MessageType.Remove:
                    if (threads.ContainsKey(id))
                    {
                        threads.Remove(id);
                    }

                    break;

                case MessageType.Deactivate:
                    if (threads.ContainsKey(id))
                    {
                        threads[id].Change(Timeout.Infinite, Timeout.Infinite);
                    }

                    break;

                case MessageType.Activate:
                    if (threads.ContainsKey(id))
                    {
                        var fc = manager.GetCase(id);
                        threads[id].Change(0, fc.FraudCaseInterval * 1000);
                    }

                    break;

                default:
                    LogService.Info("Invalid message");
                    break;
            }
        }
    }
}