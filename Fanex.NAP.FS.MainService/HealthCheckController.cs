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
using Fanex.NAP.FS.Core;
using System.Threading.Tasks;

namespace Fanex.NAP.FS.MainService
{
    public class HealthCheckController : ApiController
    {
        private static List<FraudCaseHealth> lstFCHealth;
        private static Queue.EasyNetQ _rabbitMQ;

        static HealthCheckController()
        {
            lstFCHealth = new List<FraudCaseHealth>();
            _rabbitMQ = new Queue.EasyNetQ(AppSetting.Host + ":" + AppSetting.Post,
                AppSetting.VirtualHost,
                AppSetting.PrefetchCount,
                AppSetting.UserName,
                AppSetting.Password);

            _rabbitMQ
                .CreateBus()
                .DeclareExchange(AppSetting.Exchange);

            var thread = new Thread(CheckFraudCaseStatus);
            thread.Start();
        }

        [HttpPost]
        public void Beat(string serviceName, bool status, string exception, int timeout, int fraudcaseid, string message)
        {
            var body = $"Service name: {serviceName} - Status: {status} - Fraud case ID: {fraudcaseid} - Message: {message}";
            
#if DEBUG
            System.Console.WriteLine(body);
#else
            LogService.Info(body);
#endif

            var fcHealth = lstFCHealth.Where(f => f.FraudCaseId == fraudcaseid).FirstOrDefault();
            if (fcHealth == null)
            {
                var fraudcase = new FraudCaseHealth()
                {
                    FraudCaseId = fraudcaseid,
                    IsLive = true,
                    TimeOut = timeout,
                    LastTimeSignal = DateTime.Now
                };

                lstFCHealth.Add(fraudcase);
            }
            else
            {
                fcHealth.TimeOut = timeout;
                fcHealth.LastTimeSignal = DateTime.Now;
            }

            if (!status)
            {
                message = $"Service name: {serviceName} - Status: {status} - Exception: {exception}";
#if DEBUG
                System.Console.WriteLine(message);
#else
                LogService.Error(message);
#endif

            }
        }

        [HttpGet]
        public string Testing()
        {
            return "I'm here. Everything is ok now. (Nhìn cái gì!! OK rồi =.=')";
        }

        private static void RetryFraudCaseService(int fraudCaseId)
        {
            var fraudCaseService = new Service.FraudCaseService();
            var fraudCase = fraudCaseService.GetByFraudCaseId(fraudCaseId, AppSetting.Environment);

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
                            Action = Model.Action.Retry,
                            Id = fraudCase.FraudCaseId,
                            Name = fraudCase.FraudCaseName,
                            Message = "Message is sent at" + DateTime.Now
                        }, new MessageProperties { DeliveryMode = 2, Priority = 9 });

                        var log = $"Reload Fraud Case - Queue: {_rabbitMQ.QueueName}, RoutingKey: {_rabbitMQ.RoutingKey}, Exchange: {_rabbitMQ.ExchangeName}";

                        _rabbitMQ.AdvanceBus.PublishAsync(_rabbitMQ.Exchange, _rabbitMQ.RoutingKey, false, message);
#if DEBUG
                        Console.WriteLine(log);
#endif
                    }

                }
            }
        }

        private static async void CheckFraudCaseStatus()
        {
            do
            {
                var startTime = DateTime.Now;
                Parallel.ForEach(lstFCHealth, fcHealth =>
                {
                    var timeout = fcHealth.TimeOut;
                    var period = (DateTime.Now - fcHealth.LastTimeSignal).TotalSeconds;
                    var fraudCaseName = Enum.GetName(typeof(FraudCaseId), fcHealth.FraudCaseId);
                    if (period > fcHealth.TimeOut)
                    {
                        var notifyMessage = $"Health check - Service name: {fraudCaseName} - Retry service";
                        LogService.Error(notifyMessage);
                        try
                        {
                            RetryFraudCaseService(fcHealth.FraudCaseId);
                        }
                        catch (Exception ex)
                        {
                            LogService.Error($"Health check - Error occurs: {ex.Message}");
                        }
                        
                        period = (DateTime.Now - fcHealth.LastTimeSignal).TotalSeconds;
                        
                        var errorMessage = $"Health check - Service name: {fraudCaseName} - Do not receive heart beat in " + (int)period + " seconds";
                        LogService.Error(errorMessage);
                    }
                });

                await Task.Delay(65000);
            } while (true);
        }
    }
}
