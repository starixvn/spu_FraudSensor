namespace Fanex.NAP.FS.Service.Utilities
{
    using Fanex.NAP.Core.Models.ValueObject;
    using Fanex.NAP.FS.Library.Core.Consts;
    using NAP.Core.Business;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class PermissionService
    {
        /// <summary>
        ///     Get Roll Access: get permission to access with each menu item of current user login.
        /// </summary>
        /// <param name="currentFunctions">Current Functions.</param>
        /// <returns>Return Dictionary.</returns>
        public static Dictionary<string, int> GetRoleAccess(IEnumerable<FunctionPolicy> currentFunctions)
        {
            return currentFunctions.Where(functionPolicy => functionPolicy.Policy >= (int)Constant.Policy.Read)
                                   .ToDictionary(functionPolicy => functionPolicy.Function.Key, functionPolicy => functionPolicy.Policy);
        }

        public static bool HasHostPermission(string functionName)
        {
            return AppSession.RoleAccess != null &&
                   AppSession.RoleAccess.ContainsKey(functionName) &&
                   AppSession.RoleAccess[functionName] == (int)Constant.Policy.Host;
        }

        public static bool HasPermission(string functionName)
        {
            return AppSession.RoleAccess != null &&
                   AppSession.RoleAccess.ContainsKey(functionName) &&
                   AppSession.RoleAccess[functionName] >= (int)Constant.Policy.Read;
        }

        /// <summary>
        ///     HasPermission: check that user has permission to access one menu link.
        /// </summary>
        /// <param name="functionName">Function Name.</param>
        /// <returns>Return result check permission.</returns>
        public static bool HasReadPermission(string functionName)
        {
            return AppSession.RoleAccess != null &&
                   AppSession.RoleAccess.ContainsKey(functionName) &&
                   AppSession.RoleAccess[functionName] == (int)Constant.Policy.Read;
        }

        public static bool IsMasterSubscriberAssignment()
        {
            var subscribers = AppSession.CurrentSubscriberAssignments;

            if (subscribers != null && subscribers.Any())
            {
                return subscribers[0].IsMaster == 1;
            }

            return false;
        }

        public static bool IsPermissionCN88()
        {
            var subscribers = AppSession.CurrentSubscriberAssignments;

            foreach (var sub in subscribers)
            {
                if (sub.IsMaster == 1)
                {
                    return true;
                }

                if (sub.isAssigned == 1 && sub.SubscriberName.ToUpper().Contains(Consts.SubscriberNameCN88))
                {
                    return true;
                }
            }

            return false;
        }
    }
}