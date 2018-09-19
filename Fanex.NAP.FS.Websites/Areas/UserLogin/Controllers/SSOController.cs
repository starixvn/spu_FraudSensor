namespace Fanex.NAP.FS.Areas.UserLogin.Controllers
{
    using Fanex.Common;
    using Fanex.Logging;
    using Fanex.NAP.Core.Configuration;
    using Fanex.NAP.Core.Models;
    using Fanex.NAP.Core.Models.ParameterObject;
    using Fanex.NAP.Core.Service.Impl;
    using Fanex.NAP.Core.Service.Interface;
    using Fanex.NAP.Core.Web.Auth;
    using Fanex.NAP.Core.Web.Session;
    using Fanex.NAP.FS.Service.Utilities;
    using Fanex.NAP.FS.Websites.Controllers;
    using System;
    using System.CodeDom.Compiler;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Security;

    /// <summary>
    /// SSO Controller.
    /// </summary>
    [GeneratedCodeAttribute("T4", "1.0.0.0")]
    public class SsoController : BaseController
    {
        #region members

        private readonly ICurrentSession _currentSession;
        private readonly int _timeExipration = 20;
        private readonly IUserService _userService;
        private readonly ISecurityService _securityService;

        #endregion members

        #region ctors

        public SsoController()
            : this(null, null)
        {
        }

        public SsoController(
            IUserService userService,
            ICurrentSession currentSession)
        {
            _userService = userService ?? new UserService();
            _currentSession = currentSession ?? new CurrentSession();
            _securityService = new SecurityService();
        }

        #endregion ctors

        /// <summary>
        /// SSO login.
        /// </summary>
        /// <param name="userId">User id value.</param>
        /// <param name="onuserId">On user id value.</param>
        /// <param name="time">Time value.</param>
        /// <param name="redirectUrl">The url redirection.</param>
        /// <returns>Redirect page.</returns>
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Index(string userId, string onuserId, string time, Uri redirectUrl = null)
        {
            try
            {
                userId = Cryptography.CfsDeCrypt(userId.Replace(" ", "+"));
                onuserId = Cryptography.CfsDeCrypt(onuserId.Replace(" ", "+"));
                time = Cryptography.CfsDeCrypt(time.Replace(" ", "+"));
                var currentTime = _securityService.GetTime();

                var expiration = Convert.ToDateTime(currentTime) - Convert.ToDateTime(time);

                if (expiration.TotalSeconds <= _timeExipration)
                {
                    var criteria = SetUserCriteria(Convert.ToInt32(userId), Convert.ToInt32(onuserId));
                    var onUserId = _securityService.SSOLogin(criteria);

                    if (onUserId != 0)
                    {
                        var user = _userService.Get(criteria.UserId);

                        _currentSession.UserId = user.Id;
                        _currentSession.Username = user.Username;
                        _currentSession.OnUserId = criteria.OnUserId;
                        _currentSession.SessionId = Session.SessionID;
                        _currentSession.Level = _userService.GetUserLevel(user.Id, SiteId);

                        var currentFunction = _userService.GetPermissions(user.Id, NapSettings.Settings.SiteId, null, NapSettings.Settings.Environment);
                        AppSession.CurrentUser = user;
                        AppSession.RoleAccess = PermissionService.GetRoleAccess(currentFunction);

                        if (SiteId != 0)
                        {
                            _currentSession.MenuItemList = _userService.GetMenuItem(user.Id, SiteId, Environment, true).ToList();
                            AppSession.CurrentUserMenu = _currentSession.MenuItemList;
                        }

                        CreateAuthInfo(user);

                        string forwardLink = redirectUrl == null ? FormsAuthentication.DefaultUrl : redirectUrl.ToString();

                        return Redirect(forwardLink);
                    }
                }

                return Redirect(FormsAuthentication.LoginUrl);
            }
            catch (Exception e)
            {
                LogService.Error(e.Message, e);
                throw;
            }
        }

        /// <summary>
        /// Accounting login.
        /// </summary>
        /// <param name="username">User name value.</param>
        /// <param name="onuserId">On user id.</param>
        /// <param name="redirectUrl">Redirect url.</param>
        /// <returns>Redirect page.</returns>
        [AllowAnonymous]
        [HttpGet]
        public ActionResult AccountingLogin(string username, string onuserId, string redirectUrl)
        {
            try
            {
                Session.Clear();
                username = Cryptography.CfsDeCrypt(username.Replace(" ", "+"));
                onuserId = Cryptography.CfsDeCrypt(onuserId.Replace(" ", "+"));

                var user = _userService.Get(username);

                if (user != null)
                {
                    var criteria = new LoginParameter
                    {
                        UserId = user.Id,
                        SiteId = SiteId,
                        NetSessionId = Session.SessionID,
                        Ip = CurrentContext.IP,
                        LoginDate = DateTime.Now,
                        BrowserName = CurrentContext.BrowserName,
                        CountryCode = CurrentContext.CountryCode,
                        Domain = CurrentContext.DomainName,
                        Platform = CurrentContext.Platform
                    };

                    _securityService.Login(criteria);

                    _currentSession.UserId = user.Id;
                    _currentSession.Username = user.Username;
                    _currentSession.SessionId = Session.SessionID;
                    _currentSession.Level = _userService.GetUserLevel(user.Id, SiteId);

                    var currentFunction = _userService.GetPermissions(user.Id, NapSettings.Settings.SiteId, NapSettings.Settings.Environment);
                    AppSession.CurrentUser = user;
                    AppSession.RoleAccess = PermissionService.GetRoleAccess(currentFunction);
                    AppSession.IsAccountingLogin = true;

                    CreateAuthInfo(user);
                    return Redirect(redirectUrl);
                }

                return Redirect(FormsAuthentication.LoginUrl);
            }
            catch (Exception exception)
            {
                LogService.Error(exception.Message, exception);
                return Redirect(FormsAuthentication.LoginUrl);
            }
        }

        #region utilities

        /// <summary>
        ///     Set new value to user criteria.
        /// </summary>
        /// <param name="userId">User id parameter.</param>
        /// <param name="onuserId">On User id.</param>
        /// <returns>Return UserCriteria.</returns>
        private SSOLoginParameter SetUserCriteria(int userId, int onuserId)
        {
            return new SSOLoginParameter
            {
                UserId = userId,
                SiteId = SiteId,
                CheckOnUserId = onuserId,
                OnUserId = 0,
                IP = CurrentContext.IP,
                NetSessionId = Session.SessionID,
                LoginDate = DateTime.Now,
                BrowserName = CurrentContext.BrowserName,
                CountryCode = CurrentContext.CountryCode,
                Domain = CurrentContext.DomainName,
                Platform = CurrentContext.Platform
            };
        }

        /// <summary>
        /// Save user info to cookies.
        /// </summary>
        /// <param name="user">User info.</param>
        private void CreateAuthInfo(User user)
        {
            var currentDomain = CurrentContext.CurrentDomain;

            var authInfo = new AuthInfo();

            authInfo.UserId = user.Id;
            authInfo.OnUserId = _currentSession.OnUserId;

            var authTicket = new FormsAuthenticationTicket(
                1,
                user.Username,
                DateTime.Now,
                DateTime.Now.AddMinutes(20000),
                false,
                authInfo.ToXml(),
                currentDomain.Path);

            var encrypted = FormsAuthentication.Encrypt(authTicket);
            var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted)
            {
                HttpOnly = true,
                Path = currentDomain.Path,
                Name = FormsAuthentication.FormsCookieName
            };

            Response.Cookies.Add(authCookie);
        }

        #endregion utilities
    }
}