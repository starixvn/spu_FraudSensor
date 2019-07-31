namespace Fanex.NAP.FS.Library.Services
{
    using Fanex.NAP.FS.Library.ViewModels.SubscriberAccount.Evidence;    
    using Models.MatchMonitor.FraudCaseDetails;
    using System;    
    using System.Collections.Generic;
    using Fanex.NAP.FS.Library.Models.Subscriber;
    using Fanex.NAP.FS.Library.Models.Account;

    public class EvidenceService : IEvidenceService
    {
        public ChainsToEvidence GetChainsToEvidence(int accountId)
        {
            return GetChainsToEvidence_Test(accountId);
        }

        public RetractedEvidenceAndException GetRetractedEvidenceAndException(int accountId)
        {
            return GetRetractedEvidenceAndException_Test(accountId);
        }

        #region Test
        private ChainsToEvidence GetChainsToEvidence_Test(int accountId)
        {
            var account = new Account();
            account.Id = accountId;
            account.UserName = "fpsa_pgmthai";
            account.UserName2 = "fpsa_pgmthai";

            var evidenceAccounts = new List<EvidenceAccount>();
            {
                var item = new EvidenceAccount();
                {
                    item.Id = account.Id;
                    item.AffectedDate = DateTime.Now;
                    item.CentralPoint = account.UserName;
                    item.Comment = "Comment";
                    item.Evidence = new Evidence { EvidenceCode = "1.1", EvidenceName = "abc" };
                }

                evidenceAccounts.Add(item);
            }

            var chains = new List<ChainNode>();
            {
                for (int i = 1; i < 10; i++)
                {
                    var node = new ChainNode();
                    {
                        node.Evidence = new Evidence { EvidenceCode = "1." + i, EvidenceName = "xyz_" + i};
                        node.NodeName = "fpsw_pgmthai_" + i;
                        node.NodeParentName = account.UserName;
                    }

                    chains.Add(node);
                }
            }

            var data = new ChainsToEvidence();
            {
                data.CentralPoint = "fpsa_pgmthai";
                data.Subscriber = "Subscriber";
                data.ChainsEvidenceType = "Account";
                data.Evidences = evidenceAccounts;
                data.Chains = chains;
            }

            return data;
        }

        private RetractedEvidenceAndException GetRetractedEvidenceAndException_Test(int accountId)
        {
            var data = new RetractedEvidenceAndException();

            return data;
        }

        #endregion
    }
}