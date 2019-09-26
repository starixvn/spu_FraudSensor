namespace Fanex.NAP.FS.Library.Services.Implement
{
    using Fanex.NAP.FS.Library.Models;
    using Fanex.NAP.FS.Library.Services.Interface;
    using Fanex.NAP.FS.Library.ViewModels.Account.Associated;
    using System;
    using System.Collections.Generic;

    public class AccountService : IAccountService
    {
        public IEnumerable<Evidence> GetAllEvidences()
        {
            return GetAllEvidences_Test();
        }

        public IEnumerable<AssociatedAccounts> GetAssociatedAccounts(AssociatedAccountsFilter filter)
        {
            return GetAssociatedAccounts_Test();
        }

        public DBResult UpdateAssociatedAccountOfVerify(AssociatedAccountOfVerify accountOfVerify)
        {
            return new DBResult { ErrorCode = 0, ErrorMsg = "" };
        }

        public DBResult UpdateAssociatedAccountOfNote(AssociatedAccountOfNote accountOfNote)
        {
            return new DBResult { ErrorCode = 0, ErrorMsg = "" };
        }

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

        private static IEnumerable<AssociatedAccounts> GetAssociatedAccounts_Test()
        {
            var accounts = new List<AssociatedAccounts>();

            accounts.Add(CreateNewAssociatedAccounts());

            return accounts;
        }

        private static AssociatedAccounts CreateNewAssociatedAccounts()
        {
            var note = "Now that we have imported the necessary package, we can start by defining a client which takes in various " +
                "options such as the address of the redis instance we want to connect to, " +
                "the password and the database we wish to use in that instance.";

            return new AssociatedAccounts
            {
                AssociatedAccountUsername = "abc",
                AssociatedAccountSite = "12bet",
                AssociatedAccountLevel = "Master",
                AssociatedAccountEvidences = "1.7 Shipping Fraud",
                MainAccountUsername = "def",
                MainAccountSite = "12bet",
                MainAccountLevel = "Master",
                MainAccountEvidences = "100 Seize Bet",
                FirstAssociated = DateTime.Now.ToString("MM/dd/yyyy"),
                Verified = false,
                VerifiedBy = "",
                Note = note
            };
        }
       
        #endregion

    }
}
