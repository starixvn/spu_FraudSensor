namespace Fanex.NAP.FS.Library.Services.Implement
{
    using Fanex.NAP.FS.Library.Core.Consts;
    using Fanex.NAP.FS.Library.Core.Helpers;
    using Fanex.NAP.FS.Library.Core.Helpers.Api;   
    using Fanex.NAP.FS.Library.Services.Interface;
    using Fanex.NAP.FS.Library.ViewModels.Account.AssociatedToMonitor;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System;
    using System.Net.Http;
    using System.Linq;
    using Fanex.NAP.Core.Service.Impl;
    using System.Text;

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
            var evidencesCache = filter.GetEvidencesCache();
            var accountsFromAPI = GetAssociatedAccountsFromAPI(filter);

            var accounts = accountsFromAPI.AsEnumerable()
                .Select(c => new AssociatedAccountsToMonitor
                {
                    AssAccId = c.AssAccId,
                    AssociatedAccountUsername = c.AssociatedAccountUsername,
                    AssociatedAccountSite = c.AssociatedAccountSite,
                    AssociatedAccountLevel = c.AssociatedAccountLevel,
                    AssociatedAccountEvidences = GetEvidenceForDisplay(evidencesCache, c.AssociatedAccountEvidences),
                    MainAccountUsername = c.MainAccountUsername,
                    MainAccountSite = c.MainAccountSite,
                    MainAccountLevel = c.MainAccountLevel,
                    MainAccountEvidences = GetEvidenceForDisplay(evidencesCache, c.MainAccountEvidences),
                    FirstAssociatedDate = c.FirstAssociatedDate,
                    IsVerified = c.IsVerified,
                    VerifiedBy = c.VerifiedBy,
                    VerifiedByUserName = c.VerifiedBy > 0 ? userService.Get(c.VerifiedBy).Username : null,
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

        private static string GetEvidenceForDisplay(IEnumerable<Evidence> evidences, string codes)
        {
            if (!string.IsNullOrEmpty(codes))
            {
                var evidencesForDisplay = new StringBuilder();
                var evidenceCodes = codes.Split(',');
                
                foreach (var evidenceCode in evidenceCodes)
                {
                    var query = evidences
                            .Where(e => e.EvidenceCode == evidenceCode)
                            .Select(e => e.EvidenceName);

                    if (query.Any())
                    {
                        evidencesForDisplay.Append(evidenceCode + "-" + query.ToList()[0] + "; ");
                    }
                }

                return evidencesForDisplay.ToString().Replace(Consts.FPS_EVIDENCE_CODE_SEIZE_BET_99_100, Consts.FPS_EVIDENCE_CODE_SEIZE_BET_100).TrimEnd(' ').TrimEnd(';');
            }
            
            return null;
        }
        #endregion
    }
}