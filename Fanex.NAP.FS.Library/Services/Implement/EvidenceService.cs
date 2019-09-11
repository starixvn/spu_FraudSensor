namespace Fanex.NAP.FS.Library.Services
{
    using System.Linq;
    using System.Collections.Generic;
    using Fanex.NAP.FS.Library.ViewModels.Account.Evidence;
    using Fanex.NAP.FS.Library.ViewModels.Evidence;
    using Fanex.NAP.FS.Library.Models;
    using Fanex.NAP.FS.Library.Criteria.Account.Evidence;
    using Fanex.NAP.FS.Library.Core.BaseClasses;

    public class EvidenceService : IEvidenceService
    {
        public ChainsToEvidence GetChainsToEvidence(ChainsToEvidenceFilter filter)
        {
            var accountAffectedEvidence = GetAccountAffectedEvidence(filter);

            return GetChainsToEvidence(accountAffectedEvidence);
        }

        public RetractedEvidenceAndException GetRetractedEvidenceAndException(int accountId)
        {
            return new RetractedEvidenceAndException();
        }

        public IEnumerable<Evidence> GetAllEvidences()
        {
            return new List<Evidence>();
        }

        public IEnumerable<Evidence> GetEvidencesByAccount(int accountId)
        {
            return new List<Evidence>();
        }

        #region Chains To Evidence
        private static IEnumerable<AccountAffectedEvidence> GetAccountAffectedEvidence(ChainsToEvidenceFilter filter)
        {
            var criteria = new GetAccountAffectedEvidence
            {
                targetId = filter.AccountId,
                targetType = false,
                userId = filter.UserId,
                isMaster = filter.IsMasterSubscriberAssignment
            };

            return DataService.GetData<AccountAffectedEvidence>(criteria);
        }

        private static ChainsToEvidence GetChainsToEvidence(IEnumerable<AccountAffectedEvidence> accountAffectedEvidence)
        {
            var chainsToEvidence = new ChainsToEvidence();

            chainsToEvidence.ChainsRoot = GetChainNodeRoots(accountAffectedEvidence);

            chainsToEvidence.Chains = GetChainNodes(accountAffectedEvidence);

            return chainsToEvidence;
        }

        private static IEnumerable<ChainNodeRoot> GetChainNodeRoots(IEnumerable<AccountAffectedEvidence> accountAffectedEvidence)
        {
            var chains = accountAffectedEvidence.AsEnumerable()
                .Where(c => c.MainAccount == c.RootAccount)
                .Select(c => new ChainNodeRoot
                {
                    NodeId = 1,
                    NodeName = "fpsa_pgmthai",
                    EvidenceCode = "1.1",
                    EvidenceName = "1.1"
                });

            return chains;
        }

        private static IEnumerable<ChainNode> GetChainNodes(IEnumerable<AccountAffectedEvidence> accountAffectedEvidence)
        {
            var chains = accountAffectedEvidence.AsEnumerable()
                .Where(c => c.MainAccount != c.RootAccount)
                .Select(c => new ChainNode
                {
                    NodeId = 1,
                    NodeName = "fpsa_pgmthai",
                    EvidenceCode = "1.1",
                    EvidenceName = "1.1"
                });

            return chains;
        }
        #endregion

        #region Evidence Management
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
        #endregion
        
    }
}