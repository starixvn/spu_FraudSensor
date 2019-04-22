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

        private const string ProjectName = "Fraud Sensor Service";

        private static Queue.EasyNetQ _rabbitMq;
        private readonly LetstalkService _letstalkService;

        public Runner()
        {
            InitializeComponent();

            _rabbitMq = new Queue.EasyNetQ(AppSetting.Host + ":" + AppSetting.Post,
                AppSetting.VirtualHost,
                AppSetting.PrefetchCount,
                AppSetting.UserName,
                AppSetting.Password);

            _rabbitMq
                .CreateBus()
                .DeclareExchange(AppSetting.Exchange);

            _letstalkService = new LetstalkService();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                GetFraudCase();

                var localIp = GetLocalIpAddress();

                var startOptions = new StartOptions();
                startOptions.Urls.Add($"http://{localIp}:{AppSetting.HeartBeatPort}");
                startOptions.Urls.Add($"http://localhost:{AppSetting.HeartBeatPort}");
                startOptions.Urls.Add($"http://127.0.0.1:{AppSetting.HeartBeatPort}");
       
                _server = WebApp.Start<Startup>(startOptions);

                LogService.Info($"{ProjectName} - Main service has been started.");

                _letstalkService.Info(ProjectName, "Main service has been started");

            }
            catch (Exception ex)
            {
                LogService.Error($"{ProjectName} - Main Service - Load Fraud Case - Exception: {ex.Message}");
            }
        }

        protected override void OnStop()
        {
            _rabbitMq.Dispose();
            _server.Dispose();

            LogService.Info($"{ProjectName} - Main service has been stopped.");
            _letstalkService.Info(ProjectName, "Main service has been stopped");
        }

        protected override void OnShutdown()
        {
            _rabbitMq.Dispose();
            _server.Dispose();

            LogService.Info($"{ProjectName} - Main service has been shut down.");
            _letstalkService.Info(ProjectName, "Main service has been shut down");
        }

        private void GetFraudCase()
        {
            var fraudCaseService = new Service.FraudCaseService();

            var fraudCases = fraudCaseService.Get(null, 1, Core.AppSetting.Environment);

            foreach (var fraudCase in fraudCases)
            {
                if (!fraudCase.IsActive) continue;
                var fraudCaseName = Enum.GetName(typeof(FraudCaseId), fraudCase.FraudCaseId);

                if (string.IsNullOrWhiteSpace(fraudCaseName)) continue;
                _rabbitMq
                    .DeclareQueue(fraudCaseName)
                    .DeclareRoutingKey(fraudCaseName)
                    .Bind();

                if (string.IsNullOrWhiteSpace(_rabbitMq.QueueName)) continue;
                var message = new Message<FraudCaseMessage>(new FraudCaseMessage
                {
                    Action = Model.Action.Activating,
                    Id = fraudCase.FraudCaseId,
                    Name = fraudCase.FraudCaseName,
                    Message = "Message has been sent at" + DateTime.Now
                }, new MessageProperties { DeliveryMode = 2, Priority = 9 });

                var log = $"{ProjectName} - Load Fraud Case - Queue: {_rabbitMq.QueueName}, RoutingKey: {_rabbitMq.RoutingKey}, Exchange: {_rabbitMq.ExchangeName}";

                _rabbitMq.AdvanceBus.PublishAsync(_rabbitMq.Exchange, _rabbitMq.RoutingKey, false, message);
#if DEBUG
                Console.WriteLine(log);
#endif
            }
        }

        private static string GetLocalIpAddress()
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
