namespace Fanex.NAP.FS.Service.Utilities
{
    using System;
    using System.CodeDom.Compiler;
    using System.Web;

    /// <summary>
    ///     TODO: Update summary.
    /// </summary>
    [GeneratedCodeAttribute("T4", "1.0.0.0")]
    public static class SessionManager
    {
        /// <summary>
        ///     Get Value from Session with specific Type.
        /// </summary>
        /// <typeparam name="T">T parameter.</typeparam>
        /// <param name="sessionKey">Session Key.</param>
        /// <returns>Return T result.</returns>
        public static T Get<T>(string sessionKey)
        {
            try
            {
                var value = (T)HttpContext.Current.Session[sessionKey];
                return value;
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        /// <summary>
        ///     Get value object from Session.
        /// </summary>
        /// <param name="sessionKey">Session Key.</param>
        /// <returns>Return object.</returns>
        public static object Get(string sessionKey)
        {
            return Get<object>(sessionKey);
        }

        public static void Set<T>(string sessionKey, T value)
        {
            HttpContext.Current.Session[sessionKey] = value;
        }

        public static void Set(string sessionKey, object value)
        {
            Set<object>(sessionKey, value);
        }
    }
}