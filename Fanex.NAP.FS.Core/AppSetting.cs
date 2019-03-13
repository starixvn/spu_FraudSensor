using System;
using System.Configuration;

namespace Fanex.NAP.FS.Core
{
    public static class AppSetting
    {
        public static int Environment
        {
            get
            {

                var environment = ConfigurationManager.AppSettings["Environment"];

                switch (environment)
                {
                    case "1":
                        return Convert.ToInt16(Model.Environment.Local);
                    case "2":
                        return Convert.ToInt16(Model.Environment.UAT);
                    default:
                        return Convert.ToInt16(Model.Environment.Production);
                }
            }
        }

        public static string Host
        {
            get
            {
                return ConfigurationManager.AppSettings["Queue-Host"];
            }
        }

        public static string Post
        {
            get
            {
                return ConfigurationManager.AppSettings["Queue-Post"];
            }
        }

        public static string VirtualHost
        {
            get
            {
                return ConfigurationManager.AppSettings["Queue-VirtualHost"];
            }
        }

        public static string UserName
        {
            get
            {
                return ConfigurationManager.AppSettings["Queue-UserName"];
            }
        }

        public static string Password
        {
            get
            {
                return ConfigurationManager.AppSettings["Queue-Password"];
            }
        }

        public static int PrefetchCount
        {
            get
            {
                return Convert.ToInt16(ConfigurationManager.AppSettings["Queue-PrefetchCount"]);
            }
        }

        public static string Exchange
        {
            get
            {
                return ConfigurationManager.AppSettings["Queue-Exchange"];
            }
        }

        public static string RoutingKey
        {
            get
            {
                return ConfigurationManager.AppSettings["Queue-RoutingKey"];
            }
        }

        public static string FanexLogCategory
        {
            get
            {
                return ConfigurationManager.AppSettings["Fanex.Log.Category"];
            }
        }

        public static string FanexLogRabbitMQHost
        {
            get
            {
                return ConfigurationManager.AppSettings["Fanex.Log.RabbitMQ.Host"];
            }
        }

        public static string HeartBeatUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["HeartBeat-Path"];
            }
        }

        public static string HeartBeatPort
        {
            get
            {
                return ConfigurationManager.AppSettings["HeartBeat-Port"];
            }
        }
    }
}
