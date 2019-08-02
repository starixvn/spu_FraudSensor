namespace Fanex.NAP.FS.Library.Services
{
    using Fanex.NAP.FS.Library.ViewModels.Account.Evidence;    
    using Models.MatchMonitor.FraudCaseDetails;
    using System;    
    using System.Collections.Generic;
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

            var evidenceAccounts = new List<AccountEvidence>();
            {
                var item = new AccountEvidence();
                {
                    item.Id = account.Id;
                    item.AffectedDate = DateTime.Now;
                    item.NodeNameRoot = account.UserName;
                    item.Comment = "Comment";
                    item.Evidence = new Evidence { EvidenceCode = "1.1", EvidenceName = "abc" };
                }

                evidenceAccounts.Add(item);
            }

            var chains = new List<ChainNode>();
            {
                for (int i = 1; i < 3; i++)
                {
                    var node = new ChainNode();
                    {
                        node.Evidence = new Evidence { EvidenceCode = "1." + i, EvidenceName = "xyz_" + i};
                        node.NodeIdCurrent = i;
                        node.NodeNameCurrent = "fpsw_pgmthai";
                        node.NodeIdParrent = account.Id;
                        node.NodeNameParent = account.UserName;
                        node.NodeType = "Retracted";
                    }

                    chains.Add(node);
                }

                for (int i = 1; i < 5; i++)
                {
                    var node = new ChainNode();
                    {
                        node.Evidence = new Evidence { EvidenceCode = "1." + i, EvidenceName = "xxx_" + i };
                        node.NodeIdCurrent = i;
                        node.NodeNameCurrent = "fpsw_pgmthai_" + i;
                        node.NodeIdParrent = account.Id;
                        node.NodeNameParent = account.UserName;
                        node.NodeType = "Exception";
                    }

                    chains.Add(node);
                }
            }

            var data = new ChainsToEvidence();
            {
                data.NodeNameRoot = "fpsa_pgmthai";
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
            data.ChainsToEvidence = GetChainsToEvidence_Test(accountId);

            return data;
        }

        #endregion
    }
}