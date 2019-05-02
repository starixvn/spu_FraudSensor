namespace Fanex.NAP.FS.Service
{
    using Fanex.LetstalkService.Client;
    using Fanex.LetstalkService.Core.Models;
    using Fanex.LetstalkService.Core.Services;
    using Fanex.NAP.FS.Core;
    using System.Collections.Generic;

    public class LetstalkService
    {
        private LetstalkClientService letstalkService;

        public LetstalkService()
        {
            ApplicationSettingServiceImp letstalkApi = new ApplicationSettingServiceImp();
            WebApiClientServiceImp webAPIClientService = new WebApiClientServiceImp(letstalkApi);
            letstalkService = new LetstalkClientServiceImp(webAPIClientService);
        }

        public void Info(string message)
        {
            var subject = "Info";
            SendMessage(subject, message);

        }

        public void Info(string serviceName, string message)
        {
            var subject = "Info - " + serviceName;
            SendMessage(subject, message);

        }

        public void Error(string message)
        {
            var subject = "Error";
            SendMessage(subject, message);

        }

        public void Error(string serviceName, string message)
        {
            var subject = "Error - " + serviceName;
            SendMessage(subject, message);

        }

        private void SendMessage(string subject, string content)
        {
            var env = string.Empty;
            switch (AppSetting.Environment)
            {
                case 3:
                    env = "PRO";
                    break;
                case 2:
                    env = "UAT";
                    break;
                default:
                    env = "LOCAL";
                    break;
            }
            LetstalkModel messsage = new LetstalkModel()
            {
                TargetName = AppSetting.LetstalkTarget,
                Subject = subject + " - " + env,
                Content = content,
                Group = AppSetting.LetstalkGroup,
                LetstalkAttachments = new List<LetstalkAttachment>()
            };

            letstalkService.SendMessage(messsage);
        }
    }
}
