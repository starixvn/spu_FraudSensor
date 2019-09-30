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

    public class AccountService : IAccountService
    {
        #region Associated Account To Monitor
        public IEnumerable<Evidence> GetAllEvidences()
        {
            return GetAllEvidences_Test();
        }

        public IEnumerable<AssociatedAccountsToMonitor> GetAssociatedAccountsToMonitor(AssociatedAccountsToMonitorFilter filter)
        {
            return GetAssociatedAccountsFromAPI(filter);
        }

        public DBResult UpdateAssociatedAccountToMonitorOfVerify(AssociatedAccountToMonitorOfVerify accountOfVerify)
        {
            return UpdateAssociatedAccountToMonitorOfVerifyFromAPI(accountOfVerify);
        }

        public DBResult UpdateAssociatedAccountToMonitorOfNote(AssociatedAccountToMonitorOfNote accountOfNote)
        {
            return UpdateAssociatedAccountToMonitorOfNoteFromAPI(accountOfNote);
        }

        private static IEnumerable<AssociatedAccountsToMonitor> GetAssociatedAccountsFromAPI(AssociatedAccountsToMonitorFilter filter)
        {
            var url = AppSettings.Get(Consts.FPS_WEBAPI_GET_ASSOCIATED_ACCOUNT_TO_MONITOR);

            var accounts = ApiHelper.Post<IEnumerable<AssociatedAccountsToMonitor>>(url, filter);

            Task.WaitAll(accounts);

            return accounts.Result;
        }

        private DBResult UpdateAssociatedAccountToMonitorOfVerifyFromAPI(AssociatedAccountToMonitorOfVerify accountOfVerify)
        {
            var url = AppSettings.Get(Consts.FPS_WEBAPI_UPDATE_ASSOCIATED_ACCOUNT_TO_MONITOR_OF_VERIFY);

            try
            {
                ApiHelper.Post(url, accountOfVerify).Wait();

                return new DBResult { ErrorCode = 0, ErrorMsg = "" };
            }
            catch (Exception ex)
            {
                return new DBResult { ErrorCode = 1, ErrorMsg = ex.Message };
            }
        }

        public DBResult UpdateAssociatedAccountToMonitorOfNoteFromAPI(AssociatedAccountToMonitorOfNote accountOfNote)
        {
            var url = AppSettings.Get(Consts.FPS_WEBAPI_UPDATE_ASSOCIATED_ACCOUNT_TO_MONITOR_OF_NOTE);

            try
            {
                ApiHelper.Post(url, accountOfNote).Wait();

                return new DBResult { ErrorCode = 0, ErrorMsg = "" };
            }
            catch (Exception ex)
            {
                return new DBResult { ErrorCode = 1, ErrorMsg = ex.Message };
            }
        }

        #endregion

        #region Test
        private static IEnumerable<Evidence> GetAllEvidences_Test()
        {
            var evidences = new List<Evidence>();
            evidences.Add(new Evidence { EvidenceId = 1, EvidenceCode = "1.1", EvidenceName = "Credit Card Fraud" });
            evidences.Add(new Evidence { EvidenceId = 2, EvidenceCode = "1.2", EvidenceName = "ACH/Debit Fraud" });
            evidences.Add(new Evidence { EvidenceId = 3, EvidenceCode = "1.3", EvidenceName = "Friendly Chargeback" });
            evidences.Add(new Evidence { EvidenceId = 4, EvidenceCode = "1.4", EvidenceName = "Insufficient Funds" });
            evidences.Add(new Evidence { EvidenceId = 5, EvidenceCode = "1.5", EvidenceName = "Fraud Other" });
            evidences.Add(new Evidence { EvidenceId = 6, EvidenceCode = "1.6", EvidenceName = "Potential Fraud" });
            evidences.Add(new Evidence { EvidenceId = 7, EvidenceCode = "1.7", EvidenceName = "Shipping Fraud" });
            evidences.Add(new Evidence { EvidenceId = 8, EvidenceCode = "1.8", EvidenceName = "Counterfeit Money Order/Cashiers Check" });
            evidences.Add(new Evidence { EvidenceId = 9, EvidenceCode = "1.9", EvidenceName = "Click Fraud" });
            evidences.Add(new Evidence { EvidenceId = 10, EvidenceCode = "1.10", EvidenceName = "Affiliate Fraud" });
            evidences.Add(new Evidence { EvidenceId = 11, EvidenceCode = "1.11", EvidenceName = "First Party Fraud" });
            evidences.Add(new Evidence { EvidenceId = 12, EvidenceCode = "1.12", EvidenceName = "Loan Default" });

            evidences.Add(new Evidence { EvidenceId = 13, EvidenceCode = "2.1", EvidenceName = "Collusion" });
            evidences.Add(new Evidence { EvidenceId = 14, EvidenceCode = "2.2", EvidenceName = "Chip Dumping" });
            evidences.Add(new Evidence { EvidenceId = 15, EvidenceCode = "2.3", EvidenceName = "All-In Abuse" });

            return evidences;
        }
       
        #endregion
    }
}
