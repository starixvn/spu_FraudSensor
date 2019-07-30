namespace Fanex.NAP.FS.Core
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;

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

        public static string AppData => ConfigurationManager.AppSettings["AppData-Path"];

        public static string Host => ConfigurationManager.AppSettings["Queue-Host"];

        public static string Post => ConfigurationManager.AppSettings["Queue-Port"];

        public static string VirtualHost => ConfigurationManager.AppSettings["Queue-VirtualHost"];

        public static string UserName => ConfigurationManager.AppSettings["Queue-UserName"];

        public static string Password => ConfigurationManager.AppSettings["Queue-Password"];

        public static int PrefetchCount => Convert.ToInt16(ConfigurationManager.AppSettings["Queue-PrefetchCount"]);

        public static string Exchange => ConfigurationManager.AppSettings["Queue-Exchange"];

        public static string FanexLogCategory => ConfigurationManager.AppSettings["Fanex.Log.Category"];

        public static string FanexLogRabbitMQHost => ConfigurationManager.AppSettings["Fanex.Log.RabbitMQ.Host"];

        public static string HeartBeatUrl => ConfigurationManager.AppSettings["HeartBeat-Path"];

        public static string HeartBeatPort => ConfigurationManager.AppSettings["HeartBeat-Port"];

        public static string GroupCustomerPath => ConfigurationManager.AppSettings["MS-WebAPI-GroupCustomer"];

        public static string GroupCustomerOneLevelPath => ConfigurationManager.AppSettings["MS-WebAPI-GroupCustomerOneLevel"];

        public static string CustomerEvidenceApi => ConfigurationManager.AppSettings["FPS-WebAPI-Get-CustomerEvidence"];

        public static string SentryLogging => ConfigurationManager.AppSettings["Log-SentryLogging-Config"];

        public static int FraudCaseId => Convert.ToInt32(ConfigurationManager.AppSettings["FraudCase.FraudCaseId"].ToString());

        public static string FraudCaseName => ConfigurationManager.AppSettings["FraudCase.FraudCaseName"];

        public static int FraudCaseInterval => Convert.ToInt32(ConfigurationManager.AppSettings["FraudCase.FraudCaseInterval"].ToString());

        public static string FraudCaseDesc => ConfigurationManager.AppSettings["FraudCase.FraudCaseDesc"];

        public static bool IsActive => Convert.ToBoolean(ConfigurationManager.AppSettings["FraudCase.IsActive"].ToString());

        public static bool IsTest => Convert.ToBoolean(ConfigurationManager.AppSettings["FraudCase.IsTest"].ToString());

        public static List<int> IgnoreCase => ConfigurationManager.AppSettings["Ignore-fraudcase"].Split(',').Select(Int32.Parse).ToList();

        public static string LetstalkTarget => ConfigurationManager.AppSettings["Letstalk-Target"];
        public static string LetstalkGroup => ConfigurationManager.AppSettings["Letstalk-Group"];
    }
}
