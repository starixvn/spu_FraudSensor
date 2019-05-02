namespace Fanex.NAP.FS.MainService
{
    using EasyNetQ;
    using Fanex.Logging;
    using Fanex.NAP.FS.Core;
    using Fanex.NAP.FS.Library.DTO;
    using Fanex.NAP.FS.Library.Models;
    using Fanex.NAP.FS.Model;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http;

    public class HealthCheckController : ApiController
    {
        private static readonly List<FraudCaseHealth> LstFcHealth;
        private static readonly Queue.EasyNetQ RabbitMq;
        private static readonly Service.LetstalkService LetstalkService;
        private const int MaximumLetstalkConfig = 5;

        static HealthCheckController()
        {
            LetstalkService = new Service.LetstalkService();
            LstFcHealth = new List<FraudCaseHealth>();
            RabbitMq = new Queue.EasyNetQ(AppSetting.Host + ":" + AppSetting.Post,
                AppSetting.VirtualHost,
                AppSetting.PrefetchCount,
                AppSetting.UserName,
                AppSetting.Password);

            RabbitMq
                .CreateBus()
                .DeclareExchange(AppSetting.Exchange);

            var thread = new Thread(CheckFraudCaseStatus);
            thread.Start();
        }

        [HttpPost]
        public void Beat(int fraudcaseid, bool status, string exception, int timeout, bool isActive, string message)
        {
            var fraudCaseName = Enum.GetName(typeof(FraudCaseId), fraudcaseid);

            var fcHealth = LstFcHealth.Where(f => f.FraudCaseId == fraudcaseid).FirstOrDefault();
            if (fcHealth == null)
            {
                var fraudcase = new FraudCaseHealth()
                {
                    FraudCaseId = fraudcaseid,
                    IsLive = true,
                    TimeOut = timeout,
                    IsActive = isActive,
                    LastTimeSignal = DateTime.Now,
                    LetstalkCurrentTimes = 1
                };

                LstFcHealth.Add(fraudcase);
            }
            else
            {
                fcHealth.IsActive = isActive;
                fcHealth.TimeOut = timeout;
                fcHealth.LastTimeSignal = DateTime.Now;
                fcHealth.LetstalkCurrentTimes = 1;
            }

            if (status) return;
            var bodyMessage = $"Service name: {fraudCaseName} - Status: {status} - Message: {message} - Exception: {exception}";
#if DEBUG
            System.Console.WriteLine(bodyMessage);
#else
                LogService.Error(bodyMessage);
#endif

        }

        [HttpGet]
        public string Testing()
        {
            return "I'm here. Everything is ok now. (Nhìn cái gì!! OK rồi =.=')";
        }

        private static void RetryFraudCaseService(FraudCase fraudCase)
        {
            var fraudCaseService = new Service.FraudCaseService();

            if (string.IsNullOrWhiteSpace(fraudCase.FraudCaseName)) return;
            RabbitMq
                .DeclareQueue(fraudCase.FraudCaseName)
                .DeclareRoutingKey(fraudCase.FraudCaseName)
                .Bind();

            if (string.IsNullOrWhiteSpace(RabbitMq.QueueName)) return;
            var message = new Message<FraudCaseMessage>(new FraudCaseMessage
            {
                Action = Model.Action.Retry,
                Id = fraudCase.FraudCaseId,
                Name = fraudCase.FraudCaseName,
                Message = "Message is sent at" + DateTime.Now
            }, new MessageProperties { DeliveryMode = 2, Priority = 9 });

            var log = $"Reload Fraud Case - Queue: {RabbitMq.QueueName}, RoutingKey: {RabbitMq.RoutingKey}, Exchange: {RabbitMq.ExchangeName}";

            RabbitMq.AdvanceBus.PublishAsync(RabbitMq.Exchange, RabbitMq.RoutingKey, false, message);
#if DEBUG
            Console.WriteLine(log);
#endif
        }

        private static async void CheckFraudCaseStatus()
        {
            do
            {
                var startTime = DateTime.Now;
                Parallel.ForEach(LstFcHealth, fcHealth =>
                {
                    var fraudCaseService = new Service.FraudCaseService();
                    var timeout = fcHealth.TimeOut;
                    var period = (DateTime.Now - fcHealth.LastTimeSignal).TotalSeconds;
                    var fraudCaseName = Enum.GetName(typeof(FraudCaseId), fcHealth.FraudCaseId);
                    FraudCase fraudCase = null;

                    if (AppSetting.Environment == 2)
                    {
                        fraudCase = new FraudCase()
                        {
                            FraudCaseId = fcHealth.FraudCaseId,
                            FraudCaseName = fraudCaseName,
                            IsActive = LstFcHealth.Where(f => f.FraudCaseId == fcHealth.FraudCaseId).FirstOrDefault().IsActive
                        };
                    }
                    else
                    {
                        fraudCase = fraudCaseService.GetByFraudCaseId(fcHealth.FraudCaseId, AppSetting.Environment);
                    }

                    if (!AppSetting.IgnoreCase.Contains(fraudCase.FraudCaseId) && fraudCase.IsActive && period > fcHealth.TimeOut)
                    {
                        try
                        {
                            RetryFraudCaseService(fraudCase);
                            var notifyMessage = $"Health check - Service name: {fraudCaseName} - Retry service";
                            LogService.Error(notifyMessage);
                        }
                        catch (Exception ex)
                        {
                            LogService.Error($"Health check - Error occurs: {ex.Message}");
                        }

                        var errorMessage = $"Health check - Service name: {fraudCaseName} - Do not receive heart beat in " + (int)period + " seconds";
                        LogService.Error(errorMessage);
                        SendMessageToLetstalk(fcHealth.LetstalkCurrentTimes,errorMessage);
                        fcHealth.LetstalkCurrentTimes += 1;
                    }
                });

                await Task.Delay(65000);
            } while (true);
        }

        private static void SendMessageToLetstalk(int currentTimes, string message)
        {
            if (currentTimes <= MaximumLetstalkConfig)
            {
                LetstalkService.Error(message);
            }
        }
    }
}
