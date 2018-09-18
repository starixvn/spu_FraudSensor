namespace Fanex.NAP.FS.Websites
{
    using Fanex.BetList.Core;
    using Fanex.BetList.Core.RemotingRef;
    using Fanex.Data;
    using Fanex.Logging;
    using Fanex.NAP.FS.Library.Core.Helpers.Xml;
    using Fanex.NAP.FS.Library.Core.Utilities;
    using Fanex.RemotingRef;
    using System;
    using System.Configuration;
    using System.Data.SqlClient;
    using System.Web;
    using System.Web.Http;
    using System.Web.Mvc;
    using System.Web.Optimization;
    using System.Web.Routing;

    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            DbSettingProviderManager
                .StartNewSession()
                .UseDefaultConnectionStringProvider()
                .UseDefaultDbSettingProvider(resourcePath: ConfigSettings.GetConfigStringByName("UAT") ?? "~/App_Data", enableWatching: true)
                .Run();

            RefDataTypeContainer
                .ConnectTo(new Uri(ConfigurationManager.AppSettings["Fanex.RemotingRef.Service.Url"]))
                .FromApplication("SPU - Fraud Sensor");

            BetListManager.RegisterRemotingRef(new CacheRef());

            AreaRegistration.RegisterAllAreas();
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            XmlHelper.RegisterXmlTypes();
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            AuthConfig.RegisterAuth(Context);
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError().GetBaseException();
            SqlException sqlEx = ex as SqlException;
            string exceptionFormat = "{0}\r\nType: {1}" +
                                     "\r\nSource: {2}" +
                                     "\r\nDomain: {3}" +
                                     "\r\nReferrer: {4}" +
                                     "\r\nIP: {5}" +
                                     "\r\nUserAgent: {6}" +
                                     "\r\nForm Variable: {7}" +
                                     "\r\nQueryString: {8}" +
                                     "\r\nTargetSite: {9}" +
                                     "\r\nStackTrace: {10}";

            if (!Request.IsLocal)
            {
                string message = string.Format(
                                            exceptionFormat,
                                            ex.Message + "\r\n" + ex.InnerException.Message,
                                            ex.GetType(),
                                            ex.Source,
                                            HttpContext.Current.Request.Headers["Host"],
                                            HttpContext.Current.Request.UrlReferrer,
                                            HttpContext.Current.Request.UserHostAddress,
                                            HttpContext.Current.Request.UserAgent,
                                            Request.Form,
                                            Request.QueryString,
                                            ex.TargetSite,
                                            ex.StackTrace);

                if (sqlEx != null)
                {
                    message += string.Format(
                                          "\r\nProcedure: {0}\r\nLine Number: {1}\r\nServer: {2}",
                                          sqlEx.Procedure,
                                          sqlEx.LineNumber,
                                          sqlEx.Server);
                }

                Logger.Log.Error(ex.Message, ex);
            }
        }
    }
}