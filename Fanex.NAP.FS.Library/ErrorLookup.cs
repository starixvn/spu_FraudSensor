using System.Resources;

namespace Fanex.NAP.FS.Library
{
    public static class ErrorLookup
    {
        private static readonly ResourceManager resource = new ResourceManager(typeof(Error));

        public static string GetValue(string key)
        {
            return resource.GetString(key);
        }
    }
}
