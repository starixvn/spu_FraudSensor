namespace Fanex.NAP.FS.Areas.UserLogin.Controllers
{
    using Fanex.NAP.Core.Models;
    using Fanex.NAP.Core.Models.ParameterObject;
    using Fanex.NAP.Core.Models.ValueObject;
    using Fanex.NAP.FS.Service.Utilities;
    using Fanex.NAP.FS.Websites.Controllers;
    using Logging;
    using NAP.Core.Configuration;
    using NAP.Core.Service.Impl;
    using NAP.Core.Service.Interface;
    using NAP.Core.Web;
    using NAP.Core.Web.Auth;
    using NAP.Core.Web.Session;
    using ONEbook.UM.Services;
    using ONEbook.UM.Services.Impl;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Security;

    /// <summary>
    /// Login controller.
    /// </summary>
    public class LoginController : BaseController
    {
        #region members

        private readonly ICurrentSession _currentSession;
        private readonly IGroupService _groupService;
        private readonly IMaintenanceService _maintenanceService;
        private readonly ISiteService _siteService;
        private readonly IUserService _userService;
        private readonly ISecurityService _securityService;
        private readonly SHA256Managed _hashAlgorithm;
        private const string LoginFormToken = "_SecureLoginFormToken";

        #endregion members

        #region ctorssky

        public LoginController()
            : this(null, null, null, null, null)
        {
        }

        public LoginController(
            IUserService userService,
            IGroupService groupService,
            ISiteService siteService,
            ICurrentSession currentSession,
            IMaintenanceService maintenanceService)
        {
            _userService = userService ?? new UserService();
            _groupService = groupService ?? new GroupService();
            _siteService = siteService ?? new SiteService();
            _currentSession = currentSession ?? new CurrentSession();
            _maintenanceService = maintenanceService ?? new MaintenanceService();
            _securityService = new SecurityService();
            _hashAlgorithm = new SHA256Managed();
        }

        #endregion ctorssky

        #region properties

        public string FormToken
        {
            get
            {
                if (Session[LoginFormToken] == null)
                {
                    return string.Empty;
                }

                return Session[LoginFormToken].ToString();
            }

            set
            {
                Session[LoginFormToken] = value;
            }
        }

        #endregion properties

        #region sign in

        /// <summary>
        /// Login action.
        /// </summary>
        /// <returns>Return result.</returns>
        [HttpGet]
        public ActionResult Index()
        {
            if (!NapSettings.Settings.IsNapHub && !IsSiteLogin)
            {
                // if login-directly feature is disabled then redirect users to NAP Hub
                return Redirect(NapSettings.Settings.HubUrl);
            }

            if (_maintenanceService.GetUnderMaintenanceTime().IsUnderMaintenanceTime)
            {
                return RedirectToAction("Offline", "Common");
            }

            GenerateToken();

            return View();
        }

        /// <summary>
        /// Login function.
        /// </summary>
        /// <param name="username">User name value.</param>
        /// <param name="password">Password value.</param>
        /// <returns>Return result.</returns>
        [HttpPost]
        public ActionResult Index(string username, string password, string loginToken)
        {
            try
            {
                Response.BufferOutput = true;
                object msg = null;

                if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password) && CheckValidToken(loginToken))
                {
                    var user = _userService.Get(username.Trim());

                    try
                    {
                        if (user != null)
                        {
                            var hashPw = GetHash(user.Password + loginToken);

                            if (password == hashPw)
                            {
                                var group = user.Group == null ? null : _groupService.Get(user.Group.Id);

                                // gets all sites of logging-in user
                                var siteLevelList = _userService.GetSiteLevel(user.Id, Environment);

                                ClearToken();

                                if ((group == null || group.IsActive) && siteLevelList.Any(x => x.IsActive))
                                {
                                    // Check-in user
                                    var criteria = SetUserCheckInCriteria(user.Id);
                                    var onId = _securityService.Login(criteria);

                                    // Saves user session
                                    SaveUserSession(user, username, onId);

                                    var currentFunction = _userService.GetPermissions(user.Id, NapSettings.Settings.SiteId, null, NapSettings.Settings.Environment);
                                    AppSession.CurrentUser = user;
                                    AppSession.RoleAccess = PermissionService.GetRoleAccess(currentFunction);

                                    if (AppSession.CurrentUserMenu != null &&
                                        AppSession.CurrentUserMenu.Where(m => m.Parent == null).ToList().Count > 0)
                                    {
                                        // if the user login directly, not through NAP Hub
                                        if (IsSiteLogin)
                                        {
                                            if (UserIsActiveOnThisSite(siteLevelList))
                                            {
                                                var returnUrl = FormsAuthentication.GetRedirectUrl(username, true);
                                                returnUrl = string.IsNullOrEmpty(returnUrl) ? FormsAuthentication.DefaultUrl : returnUrl;
                                                return Redirect(returnUrl);
                                            }

                                            DoSignOut();
                                            ViewBag.Message = ViewResources.Your_account_was_suspended;
                                        }
                                        else
                                        {
                                            return RedirectToRouteResult(siteLevelList);
                                        }
                                    }
                                    else
                                    {
                                        // user has no menu permission
                                        DoSignOut();
                                        ViewBag.Message = ViewResources.You_have_no_authorization;
                                    }
                                }
                                else
                                {
                                    // user account was suspended on all sites
                                    ViewBag.Message = siteLevelList.Any() ? ViewResources.Your_account_was_suspended : ViewResources.You_have_no_authorization;
                                }
                            }
                            else
                            {
                                // wrong password
                                msg = ViewResources.Invalid_Username_or_Password;
                            }
                        }
                        else
                        {
                            // user doesn't exist
                            msg = ViewResources.Invalid_Username_or_Password;
                        }
                    }
                    catch (Exception exception)
                    {
                        LogService.Error(exception.Message, exception);
                        throw;
                    }
                }
                else
                {
                    msg = "Invalid input!";
                }

                GenerateToken();

                return View(msg);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.Message, ex);
                throw;
            }
        }

        #endregion sign in

        #region sign out

        public ActionResult LogOut()
        {
            DoSignOut();

            return Redirect(FormsAuthentication.LoginUrl);
        }

        #endregion sign out

        #region re check in

        /// <summary>
        /// Re-check in.
        /// </summary>
        /// <returns>Return JSON value.</returns>
        public ActionResult ReCheckLogin()
        {
            try
            {
                if (!ConfigurationManager.AppSettings["Environment"].Equals("1"))
                {
                    var maintenanceInfo = _maintenanceService.GetUnderMaintenanceTime();
                    
                    if (maintenanceInfo.IsUnderMaintenanceTime)
                    {
                        Logger.Log.Info("UM");

                        return Json(new
                        {
                            IsKickedOut = "3"
                        });
                    }

                    var siteStatus = _siteService.IsActivated(SiteId);

                    if (!siteStatus)
                    {
                        return Json(new
                        {
                            IsKickedOut = "2"
                        });
                    }

                    if (_currentSession == null || _currentSession.OnUserId == 0)
                    {
                        return Json(new
                        {
                            IsKickedOut = "1"
                        });
                    }

                    var isKickedOut = _securityService.ReCheckIn(_currentSession.OnUserId, SiteId);

                    return Json(new
                    {
                        OnUserId = Convert.ToInt32(_currentSession.OnUserId),
                        IsKickedOut = isKickedOut
                    });
                }

                return Json(new
                {
                    IsKickedOut = "0"
                });
            }
            catch (Exception exception)
            {
                LogService.Error(exception.Message, exception);
                throw;
            }
        }

        #endregion re check in

        #region helpers

        private void DoSignOut()
        {
            _securityService.Logout(_currentSession.OnUserId);
            FormsAuthentication.SignOut();
            _currentSession.ClearAll();
            DeleteCookiesAndClearSession();
        }

        private void CreateUserMenu(User user)
        {
            _currentSession.MenuItemList = (IList<MenuItem>)_userService.GetMenuItem(user.Id, SiteId, Environment, true);
            AppSession.CurrentUserMenu = _currentSession.MenuItemList;
        }

        /// <summary>
        /// Save cookie user info.
        /// </summary>
        /// <param name="user">User info.</param>
        private void CreateAuthInfo(User user)
        {
            var currentDomain = CurrentContext.CurrentDomain;
            var authInfo = new AuthInfo { UserId = user.Id, OnUserId = _currentSession.OnUserId };
            var authTicket = new FormsAuthenticationTicket(
                1,
                user.Username,
                DateTime.Now,
                DateTime.Now.AddMinutes(20000),
                false,
                authInfo.ToXml(),
                currentDomain.Path);

            var encryptedTicket = FormsAuthentication.Encrypt(authTicket);
            var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket)
            {
                HttpOnly = true,
                Path = currentDomain.Path,
                Name = FormsAuthentication.FormsCookieName
            };

            Response.Cookies.Add(authCookie);
        }

        /// <summary>
        ///     Set new value to user criteria.
        /// </summary>
        /// <param name="userId">User id parameter.</param>
        /// <returns>Return user criteria.</returns>
        private LoginParameter SetUserCheckInCriteria(int userId)
        {
            return new LoginParameter
            {
                UserId = userId,
                SiteId = SiteId,
                NetSessionId = Session.SessionID,
                Ip = CurrentContext.IP,
                LoginDate = DateTime.Now,
                BrowserName = CurrentContext.BrowserName,
                CountryCode = CurrentContext.CountryCode,
                Domain = CurrentContext.DomainName,
                Platform = CurrentContext.Platform
            };
        }

        /// <summary>
        ///     Save user session.
        /// </summary>
        /// <param name="user">User parameter.</param>
        /// <param name="userName">User name.</param>
        /// <param name="userId">On user id.</param>
        private void SaveUserSession(User user, string userName, int userId)
        {
            _currentSession.Level = _userService.GetUserLevel(userId, SiteId);
            _currentSession.UserId = user.Id;
            _currentSession.Username = userName;
            _currentSession.OnUserId = userId;
            _currentSession.SessionId = Session.SessionID;

            // current site is not NAP Hub
            if (SiteId != 0)
            {
                CreateUserMenu(user);
            }

            CreateAuthInfo(user);
        }

        /// <summary>
        ///     Checks if logging-in user is active on this site.
        /// </summary>
        /// <param name="siteLevelList">Site level.</param>
        /// <returns>Return boolean.</returns>
        private bool UserIsActiveOnThisSite(IEnumerable<SiteLevel> siteLevelList)
        {
            bool userIsActiveOnThisSite = false;

            foreach (var siteLevel in siteLevelList)
            {
                if (siteLevel.Site.Id == SiteId && siteLevel.IsActive)
                {
                    userIsActiveOnThisSite = true;
                }
            }

            return userIsActiveOnThisSite;
        }

        /// <summary>
        ///     Redirect To Route result.
        /// </summary>
        /// <param name="siteLevelList">Site level.</param>
        /// <returns>Return route result.</returns>
        private RedirectToRouteResult RedirectToRouteResult(IEnumerable<SiteLevel> siteLevelList)
        {
            // user has access to multiple sites
            if (siteLevelList.Count(s => s.IsActive) > 1)
            {
                return RedirectToAction("Site", "UserSite");
            }

            var site = siteLevelList.First(s => s.IsActive).Site;

            var listSiteUrl =
                _siteService.GetUrlById(site.Id, Environment).Where(u => u.IsActive).ToList();

            // user has acces to one site
            if (listSiteUrl.Count > 1)
            {
                return RedirectToAction("Site", "UserSite");
            }

            return RedirectToAction("Forward", "UserSite", new { id = listSiteUrl.First().SiteUrlId });
        }

        private void AddTokenToCookie(string loginToken)
        {
            // Create the non-persistent secure form token cookie
            var responseCookie = new HttpCookie(LoginFormToken)
            {
                HttpOnly = true,
                Value = loginToken
            };

            // If we are using SSL, the cookie should be set to secure to
            // prevent it from being sent over HTTP connections
            if (FormsAuthentication.RequireSSL && Request.IsSecureConnection)
            {
                responseCookie.Secure = true;
            }

            // Add the cookie to the response
            Response.Cookies.Set(responseCookie);
        }

        private void GenerateToken()
        {
            string loginToken = Guid.NewGuid().ToString("N");

            FormToken = loginToken;
            ViewBag.LoginToken = loginToken;
            AddTokenToCookie(loginToken);
        }

        private bool CheckValidToken(string loginToken)
        {
            return !string.IsNullOrWhiteSpace(loginToken)
                && FormToken == loginToken
                && Request.Cookies[LoginFormToken] != null
                && loginToken == Request.Cookies[LoginFormToken].Value;
        }

        private string GetHash(string input)
        {
            var encrypted = string.Empty;

            if (!string.IsNullOrWhiteSpace(input))
            {
                var hashData = _hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));

                encrypted = BitConverter.ToString(hashData).Replace("-", string.Empty).ToLowerInvariant();
            }

            return encrypted;
        }

        private void ClearToken()
        {
            Session.Remove(LoginFormToken);

            if (Request.Cookies[LoginFormToken] != null)
            {
                var cookie = new HttpCookie(LoginFormToken);

                cookie.Expires = DateTime.Now.AddDays(-1d);
                Response.Cookies.Add(cookie);
            }
        }

        private void DeleteCookiesAndClearSession()
        {
            Response.Cookies.Clear();

            HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            authCookie.Expires = DateTime.Now.AddDays(-1);
            Response.Cookies.Add(authCookie);

            CurrentSession.ClearAll();
            Session.Clear();
            Session.Abandon();
        }

        #endregion helpers
    }
}