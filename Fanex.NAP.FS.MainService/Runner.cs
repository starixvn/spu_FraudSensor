namespace Fanex.NAP.FS.MainService
{
    using EasyNetQ;
    using Fanex.Logging;
    using Fanex.NAP.FS.Core;
    using Fanex.NAP.FS.Library.Models;
    using Fanex.NAP.FS.Model;
    using Fanex.NAP.FS.Service;
    using Microsoft.Owin.Hosting;
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.ServiceProcess;

    public partial class Runner : ServiceBase
    {
        private IDisposable _server = null;

        private string projectName = "Fraud Sensor Service";

        private static Queue.EasyNetQ _rabbitMQ;
        private readonly LetstalkService letstalkService;

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

            letstalkService = new LetstalkService();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                GetFraudCase();

                var localIp = GetLocalIPAddress();

                var startOptions = new StartOptions();
                startOptions.Urls.Add(string.Format("http://{0}:{1}", localIp, AppSetting.HeartBeatPort));
                startOptions.Urls.Add(string.Format("http://{0}:{1}", "localhost", AppSetting.HeartBeatPort));
                startOptions.Urls.Add(string.Format("http://{0}:{1}", "127.0.0.1", AppSetting.HeartBeatPort));
       
                _server = WebApp.Start<Startup>(startOptions);

                LogService.Info($"{projectName} - Main service has been started.");

                letstalkService.Info(projectName, "Main service has been started");

            }
            catch (Exception ex)
            {
                LogService.Error($"{projectName} - Main Service - Load Fraud Case - Exception: {ex.Message}");
            }
        }

        protected override void OnStop()
        {
            _rabbitMQ.Dispose();
            _server.Dispose();

            LogService.Info($"{projectName} - Main service has been stopped.");
            letstalkService.Info(projectName, "Main service has been stopped");
        }

        protected override void OnShutdown()
        {
            _rabbitMQ.Dispose();
            _server.Dispose();

            LogService.Info($"{projectName} - Main service has been shut down.");
            letstalkService.Info(projectName, "Main service has been shut down");
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
                            var message = new Message<FraudCaseMessage>(new FraudCaseMessage
                            {
                                Action = Model.Action.Activating,
                                Id = fraudCase.FraudCaseId,
                                Name = fraudCase.FraudCaseName,
                                Message = "Message has been sent at" + DateTime.Now
                            }, new MessageProperties { DeliveryMode = 2, Priority = 9 });

                            var log = $"{projectName} - Load Fraud Case - Queue: {_rabbitMQ.QueueName}, RoutingKey: {_rabbitMQ.RoutingKey}, Exchange: {_rabbitMQ.ExchangeName}";

                            _rabbitMQ.AdvanceBus.PublishAsync(_rabbitMQ.Exchange, _rabbitMQ.RoutingKey, false, message);
#if DEBUG
                            Console.WriteLine(log);
#endif
                        }
                    }
                }
            }
        }

        private string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }

            return "localhost";
        }
    }
}
