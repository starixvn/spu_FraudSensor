namespace Fanex.NAP.FS.Library.Core.Helpers
{
    using System.Configuration;

    public static class AppSettings
    {
        public static string Get(string key) => ConfigurationManager.AppSettings[key];
    }
}