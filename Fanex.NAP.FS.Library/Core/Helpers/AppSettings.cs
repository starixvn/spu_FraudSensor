using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.FS.Library.Core.Helpers
{
    public static class AppSettings
    {
        public static string Get(string key) => ConfigurationManager.AppSettings[key];
    }
}
