using EasyNetQ;
using EasyNetQ.Topology;
using Fanex.Logging;
using Fanex.NAP.FS.Core;
using Fanex.NAP.FS.Library.Models;
using Fanex.NAP.FS.Library.Services;
using Fanex.NAP.FS.Model;
using Fanex.NAP.FS.Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Timers;

using Microsoft.Owin.Hosting;
using System.Net.Http;

namespace Fanex.NAP.FS.MainService
{
    public partial class Runner : ServiceBase
    {
        private string baseAddress = "http://localhost:9000";
        private IDisposable _server = null;

        private string projectName = "Fraud Sensor Service";

        private static Queue.EasyNetQ _rabbitMQ;           
        
        public Runner()
        {
            InitializeComponent();

            _rabbitMQ = new Queue.EasyNetQ(AppSetting.Host + ":" + AppSetting.Post,
                AppSetting.VirtualHost,
                AppSetting.PrefetchCount,
                AppSetting.UserName,
                AppSetting.Password);

            _rabbitMQ
                .CreateBus()
                .DeclareExchange(AppSetting.Exchange);                          
        }       

        protected override void OnStart(string[] args)
        {
            try
            {
                GetFraudCase();

                _server = WebApp.Start<Startup>(url: baseAddress);

                LogService.Info($"{projectName} - Main service is started.");
            }
            catch (Exception ex)
            {
                LogService.Error($"{projectName} - Main Service - Exception: {ex.Message}");
            }           
        }

        protected override void OnStop()
        {
            _rabbitMQ.Dispose();
            _server.Dispose();

            LogService.Info($"{projectName} - Main service is stopped.");
        }             

        private void GetFraudCase()
        {
            var fraudCaseService = new Service.FraudCaseService();

            var fraudCases = fraudCaseService.Get(null, 1, Core.AppSetting.Environment);                       

            foreach (var fraudCase in fraudCases)
            {
                if (fraudCase.IsActive)
                {
                    var fraudCaseName = Enum.GetName(typeof(FraudCaseId), fraudCase.FraudCaseId);

                    if (!string.IsNullOrWhiteSpace(fraudCaseName))
                    {
                        _rabbitMQ
                            .DeclareQueue(fraudCaseName)
                            .DeclareRoutingKey(fraudCaseName)
                            .Bind();                       
                        
                        if (!string.IsNullOrWhiteSpace(_rabbitMQ.QueueName))
                        {
                            var message = new Message<FraudCaseMessage>(new FraudCaseMessage {
                                Action = Model.Action.Add,
                                Id = fraudCase.FraudCaseId,
                                Name = fraudCase.FraudCaseName,                                
                                Message = "Message is sent at" + DateTime.Now
                            }, new MessageProperties { DeliveryMode = 2, Priority = 9 });

                            var log = $"{projectName} - Load Fraud Case - Queue: {_rabbitMQ.QueueName}, RoutingKey: {_rabbitMQ.RoutingKey}, Exchange: {_rabbitMQ.ExchangeName}";                                                       
                                
                            _rabbitMQ.AdvanceBus.PublishAsync(_rabbitMQ.Exchange, _rabbitMQ.RoutingKey, false, message);

                            Console.WriteLine(log);
                            LogService.Info(log);
                        }
                    }
                }
            }
        }
    }
}
