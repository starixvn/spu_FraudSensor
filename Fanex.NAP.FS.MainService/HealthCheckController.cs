using Fanex.Logging;
using System.Web.Http;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System;
using EasyNetQ;
using Fanex.NAP.FS.Model;
using Fanex.NAP.FS.Library.Models;

namespace Fanex.NAP.FS.MainService
{
    public class HealthCheckController : ApiController
    {
        private static List<Thread> threadsMonitor = new List<Thread>();
        private static Queue.EasyNetQ _rabbitMQ;

        [HttpPost]
        public void Beat(string serviceName, bool status, string exception, int timeout, int fraudcaseid)
        {
            var thread = threadsMonitor.Where(t => t.Name == serviceName).FirstOrDefault();
            if (thread != null)
            {
                threadsMonitor.Remove(thread);
                thread.Abort();
            }

            thread = new Thread(() =>
            {
                if (!status)
                {
                    string message = $"Service name: {serviceName} - Status: {status} - Exception: {exception}";
#if DEBUG
                    System.Console.WriteLine(message);
#endif
                    LogService.Error(message);
                }

                Thread.Sleep((timeout + 5) * 1000);

                RetryFraudCaseService(fraudcaseid);

                Thread.Sleep((timeout + 10) * 1000);

                var errorMessage = $"Health check - Service name: {serviceName} - Do not receive heart beat in " + timeout + " seconds";
                LogService.Error(errorMessage);

            })
            {
                Name = serviceName
            };
            thread.Start();
            threadsMonitor.Add(thread);

#if DEBUG
            System.Console.WriteLine($"{serviceName} is reported already.");
#endif
        }

        [HttpGet]
        public string Testing()
        {
            return "I'm here. Everything is ok now. (Nhìn cái gì!! OK rồi =.=')";
        }

        private void RetryFraudCaseService(int fraudCaseId)
        {
            var fraudCaseService = new Service.FraudCaseService();

            var fraudCase = fraudCaseService.Get(null, 1, Core.AppSetting.Environment).Where(x => x.FraudCaseId == fraudCaseId).FirstOrDefault();

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
                        var message = new Message<FraudCaseMessage>(new FraudCaseMessage
                        {
                            Action = Model.Action.Activating,
                            Id = fraudCase.FraudCaseId,
                            Name = fraudCase.FraudCaseName,
                            Message = "Message is sent at" + DateTime.Now
                        }, new MessageProperties { DeliveryMode = 2, Priority = 9 });

                        var log = $"Load Fraud Case - Queue: {_rabbitMQ.QueueName}, RoutingKey: {_rabbitMQ.RoutingKey}, Exchange: {_rabbitMQ.ExchangeName}";

                        _rabbitMQ.AdvanceBus.PublishAsync(_rabbitMQ.Exchange, _rabbitMQ.RoutingKey, false, message);
#if DEBUG
                        Console.WriteLine(log);
#endif

                        LogService.Info(log);
                    }

                }
            }
        }
    }
}
