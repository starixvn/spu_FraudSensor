namespace Fanex.NAP.FS.Library.Services
{
    using Fanex.NAP.FS.Library.ViewModels.Account.Evidence;
    using Fanex.NAP.FS.Library.ViewModels.Evidence;
    using System;
    using FraudCaseEvidence = Models.MatchMonitor.FraudCaseDetails;
    using System.Collections.Generic;
    using Fanex.NAP.FS.Library.Models.Account;
    using System.Linq;
    using Fanex.NAP.FS.Library.Models;

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

        public IEnumerable<Evidence> GetAllEvidences()
        {
            return GetAllEvidences_Test().OrderBy(c => c.EvidenceGroupID).ThenBy(c => c.EvidenceTypeCode);
        }

        public IEnumerable<Evidence> GetEvidencesByAccount(int accountId)
        {
            return GetEvidencesByAccount_Test().OrderBy(c => c.EvidenceGroupID).ThenBy(c => c.EvidenceTypeCode);
        }

        public DBResult InsertEvidenceGroup(string groupName)
        {
            return new DBResult
            {
                ErrorCode = 0,
                ErrorMsg = string.Empty
            };
        }

        public DBResult InsertEvidence(int groupId, string evidenceName)
        {
            return new DBResult
            {
                ErrorCode = 0,
                ErrorMsg = string.Empty
            };
        }

        public DBResult DeleteEvidence(int evidenceId)
        {
            return new DBResult
            {
                ErrorCode = 0,
                ErrorMsg = string.Empty
            };
        }

        public DBResult UpdateEvidenceByAccount(EvidenceByAccount evidenceByAccount)
        {
            return new DBResult
            {
                ErrorCode = 0,
                ErrorMsg = string.Empty
            };
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
                    item.Evidence = new FraudCaseEvidence.Evidence { EvidenceCode = "1.1", EvidenceName = "abc" };
                }

                evidenceAccounts.Add(item);
            }

            var chains = new List<ChainNode>();
            {
                for (int i = 1; i < 3; i++)
                {
                    var node = new ChainNode();
                    {
                        node.Evidence = new FraudCaseEvidence.Evidence { EvidenceCode = "1." + i, EvidenceName = "xyz_" + i};
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
                        node.Evidence = new FraudCaseEvidence.Evidence { EvidenceCode = "1." + i, EvidenceName = "xxx_" + i };
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

        private IEnumerable<Evidence> GetAllEvidences_Test()
        {
            var evidences = new List<Evidence>();

            for (int i = 1; i < 6; i++)
            {
                evidences.Add(new Evidence
                {
                    EvidenceGroupID = 1,
                    EvidenceGroupName = "Monitoring",
                    EvidenceTypeID = i,
                    EvidenceTypeCode = "1." + i,
                    EvidenceTypeName = "VIP Smart Punter " + i
                });
            }

            for (int i = 1; i < 10; i++)
            {
                evidences.Add(new Evidence
                {
                    EvidenceGroupID = 2,
                    EvidenceGroupName = "Soccer Fraud",
                    EvidenceTypeID = i,
                    EvidenceTypeCode = "2." + i,
                    EvidenceTypeName = "Abnormal Betting " + i
                });
            }

            return evidences;
        }

        private IEnumerable<Evidence> GetEvidencesByAccount_Test()
        {
            var evidences = new List<Evidence>();

            evidences.Add(new Evidence
            {
                EvidenceGroupID = 1,
                EvidenceGroupName = "Monitoring",
                EvidenceTypeID = 6,
                EvidenceTypeCode = "1.6",
                EvidenceTypeName = "VIP Smart Punter 6"
            });

            return evidences;
        }

        #endregion
    }
}