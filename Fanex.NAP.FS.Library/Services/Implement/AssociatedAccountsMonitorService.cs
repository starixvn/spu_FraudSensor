namespace Fanex.NAP.FS.Library.Services.Implement
{
    using Fanex.NAP.FS.Library.Core.Consts;
    using Fanex.NAP.FS.Library.Core.Helpers;
    using Fanex.NAP.FS.Library.Core.Helpers.Api;   
    using Fanex.NAP.FS.Library.Services.Interface;
    using Fanex.NAP.FS.Library.ViewModels.AssociatedAccountMonitor;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System;
    using System.Net.Http;
    using System.Linq;
    using Fanex.NAP.Core.Service.Impl;
    using System.Text;    

    public class AssociatedAccountsMonitorService : IAssociatedAccountsMonitorService
    {
        public IEnumerable<Subscriber> GetAllSubscribers()
        {
            var url = AppSettings.Get(Consts.FPS_WEBAPI_CUSTOMER_SUBSCRIBER);

            var response = ApiHelper.Get<IEnumerable<string>>(url);

            Task.WhenAll(response);

            var responseSubscribers = response.Result.OrderBy(c => c);

            var subscribers = new List<Subscriber>();

            foreach (string item in responseSubscribers)
            {
                subscribers.Add(new Subscriber { SubscriberName = item });
            }

            return subscribers;
        }

        public IEnumerable<Evidence> GetAllEvidences()
        {
            return GetAllEvidencesFromAPI();
        }

        public IEnumerable<AssociatedAccountsMonitor> GetAssociatedAccountsToMonitor(AssociatedAccountsMonitorFilter filter)
        {
            var evidencesCache = filter.GetEvidencesCache();
            var accountsFromAPI = GetAssociatedAccountsFromAPI(filter);

            var accounts = accountsFromAPI.AsEnumerable()
                .Select(c => new AssociatedAccountsMonitor
                {
                    MainAccountCustId = c.MainAccountCustId,
                    MainAccountUsername = c.MainAccountUsername,
                    MainAccountSite = c.MainAccountSite,
                    MainAccountLevel = c.MainAccountLevel,
                    MainAccountEvidences = GetEvidenceForDisplay(evidencesCache, c.MainAccountEvidences),
                    NoOfAssociations = c.NoOfAssociations,
                    NoOfVerified = c.NoOfVerified
                }).OrderBy(c => c.MainAccountUsername);

            return accounts;
        }

        public IEnumerable<AssociatedAccountsDetailMonitor> GetAssociatedAccountsDetailToMonitor(AssociatedAccountsDetailMonitorFilter filter)
        {
            var userService = new UserService();
            var evidencesCache = filter.GetEvidencesCache();
            var accountsFromAPI = GetAssociatedAccountsDetailMonitorsFromAPI(filter);

            var accounts = accountsFromAPI.AsEnumerable()
                .Select(c => new AssociatedAccountsDetailMonitor
                {
                    AssAccId = c.AssAccId,
                    AssociatedCustUsername = c.AssociatedCustUsername,
                    AssociatedCustSite = c.AssociatedCustSite,
                    AssociatedCustLevel = c.AssociatedCustLevel,
                    AssociatedCustEvidences = GetEvidenceForDisplay(evidencesCache, c.AssociatedCustEvidences),
                    FirstAssociatedDate = c.FirstAssociatedDate,
                    IsVerified = c.IsVerified,
                    VerifiedBy = c.VerifiedBy,
                    VerifiedByUserName = c.VerifiedBy > 0 ? userService.Get(c.VerifiedBy).Username : null,
                    VerifiedDate = c.VerifiedDate,
                    Note = c.Note
                }).OrderBy(c => c.AssociatedCustUsername);

            return accounts;
        }

        public UpdateResult VerifyAssociatedAccountMonitor(AssociatedAccountMonitorVerify account)
        {
            var isUpdated = VerifyAssociatedAccountMonitorFromAPI(account);

            return new UpdateResult
            {
                IsSuccessStatusCode = isUpdated,
                Content = isUpdated ? GetUpdatedUserName(account.UserId) : "API update fail!"
            };
        }

        public UpdateResult UpdateNoteAssociatedAccountMonitor(AssociatedAccountMonitorNote account)
        {
            var isUpdated = UpdateNoteAssociatedAccountMonitorFromAPI(account);

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

        private static IEnumerable<AssociatedAccountsMonitor> GetAssociatedAccountsFromAPI(AssociatedAccountsMonitorFilter filter)
        {
            var url = AppSettings.Get(Consts.FPS_WEBAPI_GET_ASSOCIATED_ACCOUNT_MONITOR);

            var accounts = ApiHelper.Post<IEnumerable<AssociatedAccountsMonitor>>(url, filter);

            Task.WhenAll(accounts);

            return accounts.Result;
        }

        private static IEnumerable<AssociatedAccountsDetailMonitor> GetAssociatedAccountsDetailMonitorsFromAPI(AssociatedAccountsDetailMonitorFilter filter)
        {
            var url = AppSettings.Get(Consts.FPS_WEBAPI_GET_ASSOCIATED_ACCOUNT_DETAIL_MONITOR);

            var accounts = ApiHelper.Post<IEnumerable<AssociatedAccountsDetailMonitor>>(url, filter);

            Task.WhenAll(accounts);

            return accounts.Result;
        }

        private static bool VerifyAssociatedAccountMonitorFromAPI(AssociatedAccountMonitorVerify account)
        {
            var url = AppSettings.Get(Consts.FPS_WEBAPI_VERIFY_ASSOCIATED_ACCOUNT_MONITOR);

            var response = ApiHelper.Post<HttpResponseMessage>(url, account);

            return response.Result.IsSuccessStatusCode;
        }

        private static bool UpdateNoteAssociatedAccountMonitorFromAPI(AssociatedAccountMonitorNote account)
        {
            var url = AppSettings.Get(Consts.FPS_WEBAPI_UPDATE_NOTE_ASSOCIATED_ACCOUNT_MONITOR);

            var response = ApiHelper.Post<HttpResponseMessage>(url, account);

            return response.Result.IsSuccessStatusCode;
        }

        private static string GetUpdatedUserName(int userId)
        {
            var userName = new UserService().Get(userId).Username;

            return userName + "; " + DateTime.Now.ToString("MMM dd, yyyy");
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
    }
}