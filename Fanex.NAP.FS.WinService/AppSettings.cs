  
  
 


namespace Fanex.NAP.FS.WinService
{
    using System.Configuration;

    public static class AppSettings
    {
                public static string RemoteRefURL => ConfigurationManager.AppSettings["RemoteRefURL"];
                public static string RemoteCacheUrl => ConfigurationManager.AppSettings["RemoteCacheUrl"];
                public static string queuePath => ConfigurationManager.AppSettings["queuePath"];
                public static string FSUrl => ConfigurationManager.AppSettings["FSUrl"];
                public static string ClientSettingsProvider_ServiceUri => ConfigurationManager.AppSettings["ClientSettingsProvider.ServiceUri"];
                public static string MailingListId => ConfigurationManager.AppSettings["MailingListId"];
                public static string ExpiredTime => ConfigurationManager.AppSettings["ExpiredTime"];
                public static string Environment => ConfigurationManager.AppSettings["Environment"];
                public static string FPS_WEBAPI => ConfigurationManager.AppSettings["FPS_WEBAPI"];
                public static string FPS_WEBAPI_GET_CUSTOMER_EVIDENCE => ConfigurationManager.AppSettings["FPS_WEBAPI_GET_CUSTOMER_EVIDENCE"];
                public static string MS_WEBAPI_GROUP_CUSTOMER => ConfigurationManager.AppSettings["MS_WEBAPI_GROUP_CUSTOMER"];
            }
}

