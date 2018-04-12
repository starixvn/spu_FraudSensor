namespace Fanex.NAP.FS.Service.Utilities
{
    using Fanex.NAP.Core.Models;
    using Logging;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// AppSession class.
    /// </summary>
    public static class AppSession
    {
        private const string CurrentActionNameKey = "Fanex.NAP.FraudDetection.Contants.AppSession.CurrentActionName";
        private const string CurrentLogInUserKey = "Fanex.NAP.FraudDetection.Contants.AppSession.CurrentLogInUser";
        private const string CurrentUserMenuKey = "Fanex.NAP.FraudDetection.Contants.AppSession.CurrentUserMenu";
        private const string CurrentUserRoleAccessKey = "Fanex.NAP.FraudDetection.Contants.AppSession.CurrentUserRoleAccess";

        private const string IsAccountingLoginKey = "Fanex.NAP.FraudDetection.Contants.AppSession.IsAccountingLogin";
        private const string ViewIpVnKey = "ViewIPVN";
        private const string CurrentWorkingOnMenuKey = "Fanex.NAP.FraudDetection.Contants.AppSession.CurrentWorkingOnMenu";

        /// <summary>
        ///     Gets or sets current login user.
        /// </summary>
        /// <value>Current User.</value>
        public static User CurrentUser
        {
            get { return SessionManager.Get<User>(CurrentLogInUserKey); }

            set { SessionManager.Set(CurrentLogInUserKey, value); }
        }

        /// <summary>
        ///     Gets or sets menu of current login user.
        /// </summary>
        /// <value>Current User Menu.</value>
        public static IList<MenuItem> CurrentUserMenu
        {
            get { return SessionManager.Get<IList<MenuItem>>(CurrentUserMenuKey); }

            set { SessionManager.Set(CurrentUserMenuKey, value); }
        }

        public static Dictionary<string, int> RoleAccess
        {
            get
            {
                try
                {
                    return SessionManager.Get<Dictionary<string, int>>(CurrentUserRoleAccessKey);
                }
                catch (Exception exception)
                {
                    LogService.Error(exception.Message, exception);
                    throw;
                }
            }

            set
            {
                SessionManager.Set(CurrentUserRoleAccessKey, value);
            }
        }

        /// <summary>
        ///     Gets or sets current working menu url.
        /// </summary>
        /// <value>Current Working On Menu.</value>
        public static MenuItem CurrentWorkingOnMenu
        {
            get { return SessionManager.Get<MenuItem>(CurrentWorkingOnMenuKey); }

            set { SessionManager.Set(CurrentWorkingOnMenuKey, value); }
        }

        /// <summary>
        ///     Gets or sets IsTrackedMenu: determine current working on menu is.
        ///     tracked for page tracking or not.
        /// </summary>
        /// <value>Current Action Name.</value>
        public static string CurrentActionName
        {
            get { return SessionManager.Get<string>(CurrentActionNameKey); }

            set { SessionManager.Set(CurrentActionNameKey, value); }
        }

        public static bool IsAccountingLogin
        {
            get { return SessionManager.Get<bool>(IsAccountingLoginKey); }

            set { SessionManager.Set(IsAccountingLoginKey, value); }
        }

        public static bool ViewIPVN
        {
            get { return SessionManager.Get<bool>(ViewIpVnKey); }

            set { SessionManager.Set(ViewIpVnKey, value); }
        }
    }
}