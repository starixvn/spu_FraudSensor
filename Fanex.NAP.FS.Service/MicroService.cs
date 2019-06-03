using System.Runtime.Remoting.Channels;

namespace Fanex.NAP.FS.Service
{
    using Fanex.Logging;
    using Fanex.NAP.FS.Core;
    using Fanex.NAP.FS.Library.DTO;
    using Fanex.NAP.FS.Library.Models;
    using Fanex.NAP.FS.Model;
    using ONEbook.UM;
    using ONEbook.UM.AspNetMVC.Configuration;
    using ONEbook.UM.Helpers;
    using ONEbook.UM.Services;
    using RestSharp;
    using System;
    using System.Threading;
    using System.Timers;

    public class MicroService
    {
        private static Queue.EasyNetQ _rabbitMQ;
        private static System.Timers.Timer _timer;
        private static string _serviceName;
        private static int _fraudCaseId;
        private static string _queueName;
        private static DateTime _lastUpdate;


        protected FraudCase _fraudCase;
        protected FraudCaseService fraudCaseService;
        protected MaintenanceService maintenanceService;

        private readonly string projectName = "Fraud Sensor Service";

        public FraudCase FraudCase
        {
            get
            {
                return _fraudCase;
            }
        }

        public ElapsedEventHandler ExecutingActionHandler { get; set; }

        public MicroService(string serviceName, FraudCaseId fraudCaseId)
        {
            fraudCaseService = new FraudCaseService();
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

            var umConfig = new UMClientConfiguration();

            UMClientManager
                .UseConfig(umConfig)
                .UseRestClient(new ONEbook.UM.Transport.RestClient(umConfig))
                .UseHttpContext(new HttpContextHelper());

            maintenanceService = new MaintenanceService();

            _lastUpdate = DateTime.Now;
        }

        public void Start(ElapsedEventHandler onTimedEvent)
        {
#if DEBUG
            Console.WriteLine($"-> {_serviceName} is starting...");
#endif
            _timer = new System.Timers.Timer();
            _timer.Elapsed += (sender, e) => onTimedEvent(false, e);
            _timer.AutoReset = true;
            _timer.Interval = 60000;

            ExecutingActionHandler = (sender, e) => onTimedEvent(true, e);

            if (!string.IsNullOrWhiteSpace(_queueName))
            {
                try
                {
                    var fraudCaseMessage = GetMessage(MessageHandler);

                    if (fraudCaseMessage == null)
                    {
                        LogService.Info($"{projectName} - {_serviceName} - Start service - Fraud case id: {_fraudCaseId}");

                        MessageHandler(new FraudCaseMessage
                        {
                            Action = Model.Action.Activating,
                            Id = _fraudCaseId
                        });
                    }
                }
                catch (Exception ex)
                {
                    LogService.Error($"{projectName} - {_serviceName} - Start Exception: {ex.Message}");
                }
            }
            else
            {
                LogService.Error($"{projectName} - {_serviceName} - Start Exception: {_queueName} is empty.");
            }
        }

        private void ExecuteNow()
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback((_) =>
            {
                Thread.Sleep(1000 * 10);

                InvokeActionHandlerEvent(ExecutingActionHandler);
            }));
        }

        private void InvokeActionHandlerEvent(ElapsedEventHandler onTimedEvent)
        {
            if (_fraudCase == null)
            {
                _fraudCase = GetFraudCase(_fraudCaseId);
            }

            onTimedEvent.Invoke(null, null);
        }

        public void Hold()
        {
            _rabbitMQ.Dispose();
            _timer.Dispose();

            LogService.Error($"{_serviceName} is stopped. Please contact with Admin.");
        }

        public void MonitoringServiceLog()
        {
            var nowDate = DateTime.Now;

            if (nowDate.Subtract(_lastUpdate).TotalMinutes >= 60)
            {
#if DEBUG
                Console.WriteLine($"{_serviceName} is executed. Please be informed.");
#else
                LogService.Info($"{_serviceName} is executed. Please be informed.");
#endif
                _lastUpdate = nowDate;
            }
        }

        public void Report(ReportModel model)
        {
            var client = new RestClient(AppSetting.HeartBeatUrl);
            var request = new RestRequest("/api/HealthCheck/beat", Method.POST);

            request.AddQueryParameter("fraudcaseid", _fraudCaseId.ToString());
            request.AddQueryParameter("status", model.Status.ToString());
            request.AddQueryParameter("exception", model.Exception);
            request.AddQueryParameter("timeout", model.Timeout.ToString());
            request.AddQueryParameter("isActive", model.IsActive.ToString());
            request.AddQueryParameter("message", model.Message);

            try
            {
                var result = client.Execute(request);

                if (!result.IsSuccessful)
                {
                    LogService.Error(result.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                LogService.Error($"{projectName} - {_serviceName} - Report Exeption:{ex.Message}");
            }
        }

        public bool CheckUM()
        {
            return maintenanceService.IsUnderMaintenance().Result;
        }

        private FraudCaseMessage GetMessage(Action<FraudCaseMessage> handler)
        {
#if DEBUG
            Console.WriteLine($"{_serviceName} is listening....");
#endif
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
                LogService.Error($"{projectName} - {_serviceName} - MessageHandler Exception: {ex.Message}");
            }
        }

        private FraudCase GetFraudCase(int fraudCaseId)
        {
            var isUatTest = AppSetting.Environment == 2 ? true : false;
            var fraudCase = fraudCaseService.GetFraudCaseFromConfig();

            if (!isUatTest)
            {
                fraudCase = fraudCaseService.GetByFraudCaseId(fraudCaseId, AppSetting.Environment);
            }

            if (fraudCase != null)
            {
                //_timer.Interval = fraudCase.FraudCaseInterval * 1000;

                return fraudCase;
            }

            LogService.Error($"{projectName} - {_serviceName} - Fraud case is null. Please check again.");

            return new FraudCase
            {
                IsActive = false
            };
        }

        private void ActionHandler(FraudCaseMessage fraudCaseMessage)
        {
            switch (fraudCaseMessage.Action)
            {
                case Model.Action.Activating:
                    {
                        _fraudCase = GetFraudCase(fraudCaseMessage.Id);

                        if (_fraudCase.IsActive && !_timer.Enabled)
                        {
                            _timer.Start();

                            ExecuteNow();
                        }

                        LogService.Info($"Fraud case {fraudCaseMessage.ServiceName} is ACTIVED...");

                        break;
                    }
                case Model.Action.Deactivating:
                    {
                        _timer.Stop();

                        LogService.Info($"Fraud case {fraudCaseMessage.ServiceName} is DEACTIVED...");

                        break;
                    }
                case Model.Action.Retry:
                    {
                        _timer.Stop();

                        if (_fraudCase != null && _fraudCase.IsActive && !_timer.Enabled)
                        {
                            _timer.Start();

                            ExecuteNow();
                        }

                        LogService.Info($"Fraud case {fraudCaseMessage.ServiceName} is RETRIED...");

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
