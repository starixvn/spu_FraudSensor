namespace Fanex.NAP.FS.Service.Utilities
{
    using Fanex.NAP.Core.Models.ValueObject;
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
            var currentSubscriberAssignments = AppSession.CurrentSubscriberAssignments;

            if (currentSubscriberAssignments != null && currentSubscriberAssignments.Any())
            {
                return currentSubscriberAssignments[0].IsMaster == 1;
            }

            return false;
        }
    }
}