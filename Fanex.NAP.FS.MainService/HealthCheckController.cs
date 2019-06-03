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
        private static List<FraudCaseHealth> lstFcHealth;
        private static List<Thread> lstThreadManager;
        private static readonly Queue.EasyNetQ RabbitMq;
        private static readonly Service.LetstalkService LetstalkService;
        private const int MaximumLetstalkConfig = 5;
        static HealthCheckController()
        {
            LetstalkService = new Service.LetstalkService();
            lstFcHealth = new List<FraudCaseHealth>();
            lstThreadManager = new List<Thread>();
            RabbitMq = new Queue.EasyNetQ(AppSetting.Host + ":" + AppSetting.Post,
                AppSetting.VirtualHost,
                AppSetting.PrefetchCount,
                AppSetting.UserName,
                AppSetting.Password);

            RabbitMq
                .CreateBus()
                .DeclareExchange(AppSetting.Exchange);
            
            var thread = new Thread(InitThreadsForCheckHealth);
            thread.Start();
        }

        [HttpPost]
        public void Beat(int fraudcaseid, bool status, string exception, int timeout, bool isActive, string message)
        {
            var fraudCaseName = Enum.GetName(typeof(FraudCaseId), fraudcaseid);

            var fcHealth = lstFcHealth.Where(f => f.FraudCaseId == fraudcaseid).FirstOrDefault();
            if (fcHealth == null)
            {
                var fraudcase = new FraudCaseHealth()
                {
                    FraudCaseId = fraudcaseid,
                    IsLive = true,
                    TimeOut = timeout,
                    IsActive = isActive,
                    LastTimeSignal = DateTime.Now,
                    LetstalkCurrentTimes = 1,
                    IsOkNow = true
                };

                lstFcHealth.Add(fraudcase);
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

            LogService.Error(bodyMessage);

        }

        [HttpGet]
        public string Testing()
        {
            return "I'm here. Everything is ok now. (Nhìn cái gì!! OK rồi =.=')";
        }

        [HttpGet]
        public string Restart(int fraudCaseId)
        {
            var fraudCaseService = new Service.FraudCaseService();
            var fraudCase = fraudCaseService.GetByFraudCaseId(fraudCaseId, AppSetting.Environment);

            RetryFraudCaseService(fraudCase);

            return "Restart service is successfully";
        }

        private static void RetryFraudCaseService(FraudCase fraudCase)
        {
            var fraudCaseService = new Service.FraudCaseService();

            if (string.IsNullOrWhiteSpace(fraudCase.FraudCaseName)) return;

            var fraudCaseName = Enum.GetName(typeof(FraudCaseId), fraudCase.FraudCaseId);

            RabbitMq
                .DeclareQueue(fraudCaseName)
                .DeclareRoutingKey(fraudCaseName)
                .Bind();

            if (string.IsNullOrWhiteSpace(RabbitMq.QueueName)) return;
            var message = new Message<FraudCaseMessage>(new FraudCaseMessage
            {
                Action = Model.Action.Retry,
                Id = fraudCase.FraudCaseId,
                Name = fraudCase.FraudCaseName,
                Message = "Message is sent at" + DateTime.Now
            }, new MessageProperties { DeliveryMode = 2, Priority = 9 });

            RabbitMq.AdvanceBus.PublishAsync(RabbitMq.Exchange, RabbitMq.RoutingKey, false, message);
        }

        private static async void InitThreadsForCheckHealth()
        {
            do
            {
                foreach (var fcHealth in lstFcHealth)
                {
                    var thread = lstThreadManager.Where(t => t.Name == fcHealth.FraudCaseId.ToString()).FirstOrDefault();
                    if (thread == null)
                    {
                        Thread newThread = new Thread(() => CheckFC(fcHealth));
                        newThread.Name = fcHealth.FraudCaseId.ToString();
                        lstThreadManager.Add(newThread);
                        newThread.Start();
                    }
                }

                await Task.Delay(30000);
            } while (true);
        }

        private static void CheckFC(FraudCaseHealth fcHealth)
        {
            do
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
                        IsActive = lstFcHealth.FirstOrDefault(f => f.FraudCaseId == fcHealth.FraudCaseId).IsActive
                    };
                }
                else
                {
                    fraudCase = fraudCaseService.GetByFraudCaseId(fcHealth.FraudCaseId, AppSetting.Environment);
                }

                if (!AppSetting.IgnoreCase.Contains(fraudCase.FraudCaseId) && fraudCase.IsActive)
                {
                    if ((period - fcHealth.TimeOut) > 300) // 15 min
                    {
                        var errorMessage = $"Service name: {fraudCaseName} - Do not receive THE SIGNAL in " + (int)period + " seconds. ";

                        switch (fcHealth.LetstalkCurrentTimes)
                        {
                            case 1:
                            case 2:
                                {
                                    errorMessage += $"The service is retrying...";
                                    break;
                                }

                            case 3:
                            case 4:
                                {
                                    errorMessage += $"Please contact with Admin. The service is retrying({fcHealth.LetstalkCurrentTimes} times.)";
                                    break;
                                }
                            case 5:
                                {
                                    errorMessage += $"Please contact with Admin. THE SERVICE IS DEAD :(.";
                                    break;
                                }

                        }

                        SendMessageToLetstalk(fcHealth.LetstalkCurrentTimes, errorMessage);

                        fcHealth.LetstalkCurrentTimes += 1;

                        RetryFraudCaseService(fraudCase);

                        fcHealth.IsOkNow = false;
                    }
                    else
                    {
                        if (!fcHealth.IsOkNow)
                        {
                            SendMessageToLetstalk(fcHealth.LetstalkCurrentTimes, $"Service name: {fraudCaseName} is OK now :).");

                            fcHealth.IsOkNow = true;
                        }
                    }
                }

                Thread.Sleep(Convert.ToInt32(fcHealth.TimeOut * 1000));

            } while (true);
        }

        private static void SendMessageToLetstalk(int currentTimes, string message)
        {
            if (currentTimes <= MaximumLetstalkConfig)
            {
                LetstalkService.Error(message);

                LogService.Error(message);
            }
        }
    }
}
