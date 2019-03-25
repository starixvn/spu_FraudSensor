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
using RestSharp;

namespace Fanex.NAP.FS.Service
{
    public class MicroService
    {
        private static Queue.EasyNetQ _rabbitMQ;
        private static Timer _timer;
        private static string _serviceName;
        private static int _fraudCaseId;
        private static string _queueName;
        protected FraudCase _fraudCase;       

        private readonly string projectName = "Fraud Sensor Service";

        public FraudCase FraudCase
        {
            get {
                return _fraudCase;
            }
        }

        public MicroService(string serviceName, FraudCaseId fraudCaseId)
        {
            _rabbitMQ = new Queue.EasyNetQ(AppSetting.Host + ":" + AppSetting.Post,
               AppSetting.VirtualHost,
               AppSetting.PrefetchCount,
               AppSetting.UserName,
               AppSetting.Password);                               

            _queueName = Enum.GetName(typeof(FraudCaseId), fraudCaseId);
            _fraudCaseId = Convert.ToInt32(fraudCaseId);
            _serviceName = serviceName;

            _rabbitMQ
               .CreateBus()
               .DeclareExchange(AppSetting.Exchange)
               .DeclareQueue(_queueName)
               .DeclareRoutingKey(_queueName)
               .Bind();
        }

        public void Start(ElapsedEventHandler onTimedEvent)
        {
#if DEBUG
            Console.WriteLine($"-> {_serviceName} is starting...");
#endif
            _timer = new Timer();
            _timer.Elapsed += onTimedEvent;
            _timer.AutoReset = true;

            if (!string.IsNullOrWhiteSpace(_queueName))
            {
                try
                {
                   var fraudCaseMessage = GetMessage(MessageHandler);

                    if(fraudCaseMessage == null)
                    {
                        var fraudCase = GetFraudCase(_fraudCaseId);

                        MessageHandler(new FraudCaseMessage {
                            Action = Model.Action.Add,
                            Id = fraudCase.FraudCaseId,
                            Name = fraudCase.FraudCaseName,
                            Message = "Add message",
                            ServiceName = _serviceName                            
                        });
                    }
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

        public void Hold()
        {
            _rabbitMQ.Dispose();
            _timer.Dispose();

            LogService.Info($"{_serviceName} is stopped. Please contact with Admin.");
        }
          
        public void Clean(IEnumerable<Match> matches, List<Task> threads)
        {
            if (matches != null && matches.Any())
            {
                matches = Enumerable.Empty<Match>();
                threads.Clear();
            }
        }

        public void Report(bool status, string exception)
        {
            var client = new RestClient(AppSetting.HeartBeatUrl);
            var request = new RestRequest("/api/HealthCheck/beat", Method.POST);

            request.AddQueryParameter("serviceName", _serviceName);
            request.AddQueryParameter("status", status.ToString());
            request.AddQueryParameter("exception", exception);

            var result = client.Execute(request);

            if (!result.IsSuccessful)
            {
                LogService.Error(result.ErrorMessage);
            }
        }

        private FraudCaseMessage GetMessage(Action<FraudCaseMessage> handler)
        {
            Console.WriteLine($"{_serviceName} is listening....");

            FraudCaseMessage fraudCaseMessage = null;

            _rabbitMQ.AdvanceBus.Consume(_rabbitMQ.Queue, x => x.Add<FraudCaseMessage>((message, info) =>
            {
                fraudCaseMessage = message.Body;
                handler(fraudCaseMessage);
            }));

            return fraudCaseMessage;
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

                        if (!_timer.Enabled)
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

                        break;
                    }
                default:
                    {                     
                        break;
                    }
            }
        }
    }
}
