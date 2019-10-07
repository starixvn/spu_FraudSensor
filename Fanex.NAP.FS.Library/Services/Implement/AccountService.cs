namespace Fanex.NAP.FS.Library.Services.Implement
{
    using Fanex.NAP.FS.Library.Core.Consts;
    using Fanex.NAP.FS.Library.Core.Helpers;
    using Fanex.NAP.FS.Library.Core.Helpers.Api;
    using Fanex.NAP.FS.Library.Models;
    using Fanex.NAP.FS.Library.Services.Interface;
    using Fanex.NAP.FS.Library.ViewModels.Account.AssociatedToMonitor;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System;
    using System.Net.Http;
    using System.Linq;
    using Fanex.NAP.Core.Service.Impl;

    public class AccountService : IAccountService
    {
        #region Associated Account To Monitor
        public IEnumerable<Evidence> GetAllEvidences()
        {
            return GetAllEvidencesFromAPI();
        }

        public IEnumerable<AssociatedAccountsToMonitor> GetAssociatedAccountsToMonitor(AssociatedAccountsToMonitorFilter filter)
        {
            var userService = new UserService();
           
            var accountsFromAPI = GetAssociatedAccountsFromAPI(filter);

            var accounts = accountsFromAPI.AsEnumerable()
                .Select(c => new AssociatedAccountsToMonitor
                {
                    AssAccId = c.AssAccId,
                    AssociatedAccountUsername = c.AssociatedAccountUsername,
                    AssociatedAccountSite = c.AssociatedAccountSite,
                    AssociatedAccountLevel = c.AssociatedAccountLevel,
                    AssociatedAccountEvidences = c.AssociatedAccountEvidences,
                    MainAccountUsername = c.MainAccountUsername,
                    MainAccountSite = c.MainAccountSite,
                    MainAccountLevel = c.MainAccountLevel,
                    MainAccountEvidences = c.MainAccountEvidences,
                    FirstAssociatedDate = c.FirstAssociatedDate,
                    IsVerified = c.IsVerified,
                    VerifiedBy = c.VerifiedBy,
                    VerifiedByUserName = c.VerifiedBy > 0 ? userService.Get(c.VerifiedBy).Username : string.Empty,
                    VerifiedDate = c.VerifiedDate,
                    Note = c.Note
                });

            return accounts;
        }

        public UpdateResult UpdateAssociatedAccountToMonitorOfVerify(AssociatedAccountToMonitorOfVerify accountOfVerify)
        {
            var isUpdated = UpdateAssociatedAccountToMonitorOfVerifyFromAPI(accountOfVerify);

            return new UpdateResult
            {
                IsSuccessStatusCode = isUpdated,
                Content = isUpdated ? GetUpdatedUserName(accountOfVerify.UserId) : "API update fail!"
            };
        }

        public UpdateResult UpdateAssociatedAccountToMonitorOfNote(AssociatedAccountToMonitorOfNote accountOfNote)
        {
            var isUpdated = UpdateAssociatedAccountToMonitorOfNoteFromAPI(accountOfNote);

            return new UpdateResult
            {
                IsSuccessStatusCode = isUpdated,
                Content = isUpdated ? string.Empty : "API update fail!"
            };
        }

        private static IEnumerable<Evidence> GetAllEvidencesFromAPI()
        {
            var url = AppSettings.Get(Consts.FPS_WEBAPI_GET_ALL_EVIDENCE);

            var evidences = ApiHelper.Post<IEnumerable<Evidence>>(url, null);

            Task.WaitAll(evidences);

            return evidences.Result;
        }

        private static IEnumerable<AssociatedAccountsToMonitor> GetAssociatedAccountsFromAPI(AssociatedAccountsToMonitorFilter filter)
        {
            var url = AppSettings.Get(Consts.FPS_WEBAPI_GET_ASSOCIATED_ACCOUNT_TO_MONITOR);

            var accounts = ApiHelper.Post<IEnumerable<AssociatedAccountsToMonitor>>(url, filter);

            Task.WaitAll(accounts);

            return accounts.Result;
        }

        private static bool UpdateAssociatedAccountToMonitorOfVerifyFromAPI(AssociatedAccountToMonitorOfVerify accountOfVerify)
        {
            var url = AppSettings.Get(Consts.FPS_WEBAPI_UPDATE_ASSOCIATED_ACCOUNT_TO_MONITOR_OF_VERIFY);

            try
            {
                var response =  ApiHelper.Post<HttpResponseMessage>(url, accountOfVerify);

                return response.Result.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private static bool UpdateAssociatedAccountToMonitorOfNoteFromAPI(AssociatedAccountToMonitorOfNote accountOfNote)
        {
            var url = AppSettings.Get(Consts.FPS_WEBAPI_UPDATE_ASSOCIATED_ACCOUNT_TO_MONITOR_OF_NOTE);

            try
            {
                var response = ApiHelper.Post<HttpResponseMessage>(url, accountOfNote);

                return response.Result.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private static string GetUpdatedUserName(int userId)
        {
            var userName = new UserService().Get(userId).Username;

            return userName + ", "+ DateTime.Now.ToString("MMM dd, yyyy");
        }
        #endregion
    }
}
