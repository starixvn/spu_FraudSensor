namespace Fanex.NAP.FS.Library.Core.Utilities
{
    using System.Configuration;

    public static class ConfigSettings
    {
        public static string GetConnectionStringByName(string name)
        {
            string returnValue = null;

            var settings = ConfigurationManager.ConnectionStrings[name];

            if (settings != null)
            {
                returnValue = settings.ConnectionString;
            }

            return returnValue;
        }

        public static string GetConfigStringByName(string name)
        {
            return ConfigurationManager.AppSettings[name];
        }

        public static string RabbitMQ_Host()
        {
            return ConfigurationManager.AppSettings["RabbitMQ-Host"];
        }

        public static string RabbitMQ_VirtualHost()
        {
            return ConfigurationManager.AppSettings["RabbitMQ-VirtualHost"];
        }

        public static string RabbitMQ_PrefetchCount()
        {
            return ConfigurationManager.AppSettings["RabbitMQ-PrefetchCount"];
        }

        public static string RabbitMQ_Username()
        {
            return ConfigurationManager.AppSettings["RabbitMQ-Username"];
        }

        public static string RabbitMQ_Password()
        {
            return ConfigurationManager.AppSettings["RabbitMQ-Password"];
        }

        public static string RabbitMQ_Port()
        {
            return ConfigurationManager.AppSettings["RabbitMQ-Port"];
        }

        public static string RabbitMQ_Queue()
        {
            return ConfigurationManager.AppSettings["RabbitMQ-Queue"];
        }


        public static string RabbitMQ_Exchange()
        {
            return ConfigurationManager.AppSettings["RabbitMQ-Exchange"];
        }

        public static string RabbitMQ_RoutingKey()
        {
            return ConfigurationManager.AppSettings["RabbitMQ-RoutingKey"];
        }
    }
}