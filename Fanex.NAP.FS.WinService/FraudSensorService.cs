#pragma warning disable S2325 // Methods and properties that don't access instance data should be static

namespace Fanex.NAP.FS.WinService
{
    using Fanex.NAP.FS.Library.DTO;
    using Fanex.NAP.FS.Library.Models;
    using Logging;
    using MailDelivery.Client;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.ServiceProcess;
    using System.Threading;
    using System.Threading.Tasks;

    public partial class FraudSensorService : ServiceBase
    {
        private readonly IFraudCaseManager manager;
        private readonly IMessager messager;
        private Thread _thread;

        private IDictionary<FraudCaseId, Timer> timers;

        private IDictionary<FraudCaseId, bool> executionCheckers;

        public FraudSensorService(IFraudCaseManager manager, IMessager messager)
        {
            this.manager = manager;
            this.messager = messager;
        }

        protected override void OnStart(string[] args)
        {
            _thread = new Thread(RunOnStart);
            _thread.Start();
        }

        protected override void OnStop()
        {
            Logger.Log.Info("SPU FS 2 Service Stopped");

            _thread.Abort();
        }

        protected override void OnShutdown()
        {
            SendMailInform(false);

            Logger.Log.Info("SPU FS 2 Service Stopped");

            MailClientHandler.Stop();
        }

        public void RunOnStart()
        {
            try
            {
                Logger.Log.Info("SPU FS 2 Service Started");

                MailClientHandler.Start();

                Logger.Log.Info("SPU FS 2 Service - Loading fraudcases");

                var fraudCases = manager.GetCases();

                executionCheckers = fraudCases.ToDictionary(f => (FraudCaseId)f.FraudCaseId, f => false);

                timers = fraudCases.ToDictionary(f => (FraudCaseId)f.FraudCaseId, f => GetTimer(f));

                Logger.Log.Info("SPU FS 2 Service - Loading fraudcases have done.");

                messager.ListenToMessage(HandleMsg);

                SendMailInform(true);
            }
            catch (Exception ex)
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

        private Timer GetTimer(FraudCase fraudCase)
        {
            var executor = manager.GetExecutor(fraudCase);

            var timer = new Timer(o => ExecuteFraudCase(executor), null, 0, fraudCase.FraudCaseInterval * 1000);
            
            return timer;
        }

        private void ExecuteFraudCase(IFraudCaseExecutor executor)
        {
            try
            {
                if (executionCheckers[executor.FraudCaseId])
                {
                    return;
                }
                executionCheckers[executor.FraudCaseId] = true;

                executor.Execute();
            }
            catch (Exception ex)
            {
                LogService.Error(ex.Message, ex);
            }
            finally
            {
                executionCheckers[executor.FraudCaseId] = false;
            }
        }

        private void HandleMsg(MessageType message, FraudCaseId id)
        {
            switch (message)
            {
                case MessageType.Adding:
                    if (!timers.ContainsKey(id))
                    {
                        var fc = manager.GetCase(id);
                        timers.Add(id, GetTimer(fc));
                    }

                    break;

                case MessageType.Removing:
                    if (timers.ContainsKey(id))
                    {
                        timers.Remove(id);
                    }

                    break;

                case MessageType.Deactivating:
                    if (timers.ContainsKey(id))
                    {
                        timers[id].Change(Timeout.Infinite, Timeout.Infinite);
                    }

                    break;

                case MessageType.Activating:
                    if (timers.ContainsKey(id))
                    {
                        var fc = manager.GetCase(id);
                        timers[id].Change(0, fc.FraudCaseInterval * 1000);
                    }

                    break;

                default:
                    LogService.Info("Invalid message");
                    break;
            }
        }
    }
}

#pragma warning restore S2325 // Methods and properties that don't access instance data should be static