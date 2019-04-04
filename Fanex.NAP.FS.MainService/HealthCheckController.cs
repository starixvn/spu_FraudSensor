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
using Fanex.NAP.FS.Library.DTO;

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
        public void Beat(int fraudcaseid, bool status, string exception, int timeout, bool isActive, string message)
        {
            var fraudCaseName = Enum.GetName(typeof(FraudCaseId), fraudcaseid);
            var body = $"Service name: {fraudCaseName} - Status: {status} - Fraud case ID: {fraudcaseid} - Message: {message}";

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
                    IsActive = isActive,
                    LastTimeSignal = DateTime.Now
                };

                lstFCHealth.Add(fraudcase);
            }
            else
            {
                fcHealth.IsActive = isActive;
                fcHealth.TimeOut = timeout;
                fcHealth.LastTimeSignal = DateTime.Now;
            }

            if (!status)
            {
                message = $"Service name: {fraudCaseName} - Status: {status} - Exception: {exception}";
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
            var fraudCaseName = Enum.GetName(typeof(FraudCaseId), fraudCaseId);
            FraudCase fraudCase = null;

            if (AppSetting.Environment == 2)
            {
                fraudCase = new FraudCase()
                {
                    FraudCaseId = fraudCaseId,
                    FraudCaseName = fraudCaseName,
                    IsActive = lstFCHealth.Where(f => f.FraudCaseId == fraudCaseId).FirstOrDefault().IsActive
                };
            }
            else
            {
                fraudCase = fraudCaseService.GetByFraudCaseId(fraudCaseId, AppSetting.Environment);
            }

            if (fraudCase.IsActive)
            {
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
                        try
                        {
                            RetryFraudCaseService(fcHealth.FraudCaseId);
                            var notifyMessage = $"Health check - Service name: {fraudCaseName} - Retry service";
                            LogService.Error(notifyMessage);
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
