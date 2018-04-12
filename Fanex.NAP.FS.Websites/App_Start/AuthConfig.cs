namespace Fanex.NAP.FS.Websites
{
    using Fanex.NAP.Core.Auth;
    using Fanex.NAP.Core.Web.Auth;
    using System.Web;
    using System.Web.Security;

    public static class AuthConfig
    {
        public static void RegisterAuth(HttpContext context)
        {
            var user = HttpContext.Current.User;

            if (user != null && user.Identity is FormsIdentity && user.Identity.IsAuthenticated)
            {
                var cookie = FormsAuthentication.FormsCookieName;
                var httpCookie = context.Request.Cookies[cookie];

                if (httpCookie == null)
                {
                    return;
                }

                var ticket = FormsAuthentication.Decrypt(httpCookie.Value);
                if (ticket == null || ticket.Expired)
                {
                    return;
                }

                var identity = new FormsIdentity(ticket);
                if (string.IsNullOrEmpty(ticket.UserData))
                {
                    return;
                }

                var udata = AuthInfo.CreateUserData(ticket.UserData);
                var principal = new NapPrincipal(identity, udata);
                context.User = principal;
            }
        }
    }
}