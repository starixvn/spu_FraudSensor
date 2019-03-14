using EasyNetQ;
using EasyNetQ.Topology;
using Fanex.Logging;
using Fanex.NAP.FS.Core;
using Fanex.NAP.FS.Library.DTO;
using Fanex.NAP.FS.Library.Models;
using Fanex.NAP.FS.Model;
using System;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Timers;
using System.Linq;
using System.Net.Http;

namespace Fanex.NAP.FS.Service
{
    public class MicroServiceBase : ServiceBase
    {
        private static Queue.EasyNetQ _rabbitMQ;
        private static Timer _timer;
        private static string _serviceName;
        private static string _queueName;

        protected FraudCase _fraudCase;
        protected IEnumerable<Match> _matches;
        protected List<Task> _threads;

        private string projectName = "Fraud Sensor Service";

        public MicroServiceBase(string serviceName, FraudCaseId fraudCaseId)
        {
            _rabbitMQ = new Queue.EasyNetQ(AppSetting.Host + ":" + AppSetting.Post,
               AppSetting.VirtualHost,
               AppSetting.PrefetchCount,
               AppSetting.UserName,
               AppSetting.Password);                    
            
            _timer = new Timer();
            _timer.Elapsed += OnTimedEvent;
            _timer.AutoReset = true;

            _queueName = Enum.GetName(typeof(FraudCaseId), fraudCaseId);
            _serviceName = serviceName;

            _rabbitMQ
               .CreateBus()
               .DeclareExchange(AppSetting.Exchange)
               .DeclareQueue(_queueName)
               .DeclareRoutingKey(_queueName)
               .Bind();
        }

        protected override void OnStart(string[] args)
        {
#if DEBUG
            Console.WriteLine($"-> {_serviceName} is starting...");
#endif
            if (!string.IsNullOrWhiteSpace(_queueName))
            {             
                try
                {
                    GetMessage(MessageHandler);
                }
                catch (Exception ex)
                {
                    LogService.Error($"{projectName} - {_serviceName} - Exception: {ex.Message}");
                }
            }
            else
            {
                LogService.Error($"{projectName} - {_serviceName} - Exception: {_queueName} is empty.");
            }
        }

        protected override void OnStop()
        {
            _rabbitMQ.Dispose();
            _timer.Dispose();

            LogService.Info($"{_serviceName} is stopped. Please contact with Admin.");
        }

        private void GetMessage(Action<FraudCaseMessage> handler)
        {
            Console.WriteLine($"{_serviceName} is listening....");

            _rabbitMQ.AdvanceBus.Consume(_rabbitMQ.Queue, x => x.Add<FraudCaseMessage>((message, info) =>
            {
                handler(message.Body);
            }));
        }

        private void MessageHandler(FraudCaseMessage message)
        {
            try
            {
                ActionHandler(message);
            }
            catch (Exception ex)
            {
                LogService.Error($"{projectName} - {_serviceName} - Exception: {ex.Message}");
            }           
        }
        
        private FraudCase GetFraudCase(int fraudCaseId)
        {
#if DEBUG
            Console.WriteLine("-> Get fraud case action is executing...");
#endif
            var fraudCaseService = new FraudCaseService();

            var fraudCase = fraudCaseService.GetByFraudCaseId(fraudCaseId);

            if (fraudCase != null)
            {
                _timer.Interval = fraudCase.FraudCaseInterval * 1000;

                return fraudCase;
            }
            else
            {
                LogService.Error($"{_serviceName} cannot load Fraud Case from DB with ID= { fraudCaseId }. Please contact with Admin.");
            }

            return null;
        }

        private void ActionHandler(FraudCaseMessage fraudCaseMessage)
        {
            switch (fraudCaseMessage.Action)
            {
                case Model.Action.Add:
                    {
#if DEBUG
                        Console.WriteLine("-> Add action is executing...");
#endif
                        _fraudCase = GetFraudCase(fraudCaseMessage.Id);

                        if (_timer.Enabled)
                        {
                            _timer.Stop();
                            _timer.Start();
                        }
                        else
                        {
                            _timer.Start();
                        }

                        break;
                    }
                case Model.Action.Update:
                    {
#if DEBUG
                        Console.WriteLine("-> Update action is executing...");
#endif
                        _timer.Stop();

                        _fraudCase = GetFraudCase(fraudCaseMessage.Id);

                        _timer.Start();

                        break;
                    }
                case Model.Action.Delete:
                    {
#if DEBUG
                        Console.WriteLine("-> Delete action is executing...");
#endif
                        _timer.Stop();
                        CleanResult();

                        break;
                    }
                default:
                    {                     
                        break;
                    }
            }
        }

        private void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            var exception = string.Empty;
            var status = true;

            try
            {
#if DEBUG
                Console.WriteLine("-> Fraud Case is executing at " + DateTime.Now);
#endif
                CleanResult();
                Precondition();
                Rule();
                NextAction();

#if DEBUG
                Console.WriteLine("=> Fraud Case is executed at " + DateTime.Now);
#endif
            }
            catch (Exception ex)
            {
                exception = ex.Message;
                status = false;

                LogService.Critical($"{projectName} - {_serviceName} - Handle Action Executing - Exception: {ex.Message}");
            }

            Report(status, exception);            
        }

        protected virtual void Report(bool status, string exception) { }          

        protected virtual void Precondition() { }

        protected virtual void Rule() { }

        protected virtual void NextAction() { }

        protected virtual void CleanResult() { }
    }
}
