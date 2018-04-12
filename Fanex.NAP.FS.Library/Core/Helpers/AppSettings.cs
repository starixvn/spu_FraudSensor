using System.Configuration;

namespace Fanex.NAP.FS.Library.Core.Helpers
{
    public static class AppSettings
    {
        public static string Get(string key) => ConfigurationManager.AppSettings[key];
    }
}