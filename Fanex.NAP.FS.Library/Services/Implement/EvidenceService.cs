namespace Fanex.NAP.FS.Library.Services
{
    using System.Linq;
    using System.Collections.Generic;
    using Fanex.NAP.FS.Library.ViewModels.Account.Evidence;
    using Fanex.NAP.FS.Library.ViewModels.Evidence;
    using Fanex.NAP.FS.Library.Models;
    using Fanex.NAP.FS.Library.Criteria.Account.Evidence;
    using Fanex.NAP.FS.Library.Core.BaseClasses;
    using Fanex.NAP.FS.Library.Models.Account.Evidence;
    using Fanex.NAP.FS.Library.ViewModels.Account.Evidence.Exception;

    public class EvidenceService : IEvidenceService
    {
        #region Chains To Evidence
        public ChainsToEvidence GetChainsToEvidence(ChainsToEvidenceFilter filter)
        {
            var accounts = GetAccountsEvidence(filter);

            return GetChainsToEvidence(accounts);
        }

        public DBResult AddExceptionEvidence(ExceptionEvidenceAdd exceptionEvidenceAdd)
        {
            var criteria = new InsertExceptionEvidenceCriteria
            {
                ip_MainAccountId = exceptionEvidenceAdd.AccountId,
                ip_RootAccountId = exceptionEvidenceAdd.ExceptionAccountId,
                ip_SubscriberId = exceptionEvidenceAdd.SubscriberId,
                ip_Remark = exceptionEvidenceAdd.Comment,
                ip_CreatedBy = exceptionEvidenceAdd.UserId
            };

            return DataService.UpdateData(criteria, "Insert exception evidence");
        }

        private static IEnumerable<AccountEvidence> GetAccountsEvidence(ChainsToEvidenceFilter filter)
        {
            var criteria = new GetAccountEvidenceCriteria
            {
                ip_TargetId = filter.AccountId,
                ip_TargetType = false,
                ip_UserId = filter.UserId,
                ip_IsMaster = filter.IsMasterSubscriberAssignment,
                ip_IsActiveEvidence = true
            };

            return DataService.GetData<AccountEvidence>(criteria);
        }

        private static ChainsToEvidence GetChainsToEvidence(IEnumerable<AccountEvidence> accounts)
        {
            var chainsToEvidence = new ChainsToEvidence();

            chainsToEvidence.ChainsDirected = GetChainsDirected(accounts);

            chainsToEvidence.ChainsAffected = GetChainsAffected(accounts);

            return chainsToEvidence;
        }

        private static IEnumerable<ChainNode> GetChainsDirected(IEnumerable<AccountEvidence> accounts)
        {
            return accounts.AsEnumerable()
                .Where(c => c.MainAccount == c.RootAccount)
                .Select(c => new ChainNode
                {
                    DirectedAccountId = c.MainAccountID,
                    DirectedUserName = c.MainAccount,
                    EvidenceCode = c.EvidenceCode
                })
                .OrderBy(c=>c.EvidenceCode);
        }

        private static IEnumerable<ChainNode> GetChainsAffected(IEnumerable<AccountEvidence> accounts)
        {
            return accounts.AsEnumerable()
                .Where(c => c.MainAccount != c.RootAccount)
                .Select(c => new ChainNode
                {
                    AffectedAccountId = c.MainAccountID,
                    AffectedUserName = c.MainAccount,
                    DirectedAccountId = c.RootAccountID,
                    DirectedUserName = c.RootAccount,
                    EvidenceCode = c.EvidenceCode
                })
                .OrderBy(c => c.DirectedUserName)
                .ThenBy(c => c.EvidenceCode);
        }
        #endregion

        #region Retracted And Exception Evidence
        public RetractedAndExceptionEvidence GetRetractedAndExceptionEvidence(RetractedAndExceptionEvidenceFilter filter)
        {
            var accountCollected = new RetractedAndExceptionCollected();

            accountCollected.AccountsRetracted = GetAccountsRetractedEvidence(filter);

            accountCollected.AccountsException = GetAccountsExceptionEvidence(filter);

            return GetRetractedAndExceptionEvidence(accountCollected);
        }

        private static IEnumerable<AccountEvidence> GetAccountsRetractedEvidence(RetractedAndExceptionEvidenceFilter filter)
        {
            var criteria = new GetAccountEvidenceCriteria
            {
                ip_TargetId = filter.AccountId,
                ip_TargetType = false,
                ip_UserId = filter.UserId,
                ip_IsMaster = filter.IsMasterSubscriberAssignment,
                ip_IsActiveEvidence = false
            };

            return DataService.GetData<AccountEvidence>(criteria);
        }

        private static IEnumerable<AccountExceptionEvidence> GetAccountsExceptionEvidence(RetractedAndExceptionEvidenceFilter filter)
        {
            var criteria = new GetExceptionAccountEvidenceCriteria
            {
                ip_AccountId = filter.AccountId,
                ip_UserId = filter.UserId,
                ip_IsMaster = filter.IsMasterSubscriberAssignment
            };

            return DataService.GetData<AccountExceptionEvidence>(criteria);
        }

        private static RetractedAndExceptionEvidence GetRetractedAndExceptionEvidence(RetractedAndExceptionCollected accountCollected)
        {
            var accountEvidence = new RetractedAndExceptionEvidence();

            accountEvidence.ChainsDirectedRetractedEvidence = GetDirectedRetractedEvidence(accountCollected.AccountsRetracted);

            accountEvidence.ChainsAffectedRetractedEvidence = GetAffectedRetractedEvidence(accountCollected.AccountsRetracted);

            accountEvidence.ChainsExceptionEvidence = GetExceptionEvidence(accountCollected.AccountsException);

            return accountEvidence;
        }

        private static IEnumerable<RetractedAndExceptionNode> GetDirectedRetractedEvidence(IEnumerable<AccountEvidence> accounts)
        {
            return accounts.AsEnumerable()
                .Where(c => c.MainAccount == c.RootAccount)
                .Select(c => new RetractedAndExceptionNode
                {
                    DirectedAccountId = c.MainAccountID,
                    DirectedUserName = c.MainAccount,
                    EvidenceCode = c.EvidenceCode                    
                })
                .OrderBy(c => c.EvidenceCode);
        }

        private static IEnumerable<RetractedAndExceptionNode> GetAffectedRetractedEvidence(IEnumerable<AccountEvidence> accounts)
        {
            return accounts.AsEnumerable()
                 .Where(c => c.MainAccount != c.RootAccount)
                 .Select(c => new RetractedAndExceptionNode
                 {
                     AffectedAccountId = c.MainAccountID,
                     AffectedUserName = c.MainAccount,
                     DirectedAccountId = c.RootAccountID,
                     DirectedUserName = c.RootAccount,
                     EvidenceCode = c.EvidenceCode,                     
                 })
                 .OrderBy(c => c.DirectedUserName)
                 .ThenBy(c => c.EvidenceCode);
        }

        private static IEnumerable<RetractedAndExceptionNode> GetExceptionEvidence(IEnumerable<AccountExceptionEvidence> accounts)
        {
            return accounts.AsEnumerable()
                .Select(c => new RetractedAndExceptionNode
                {
                    AffectedAccountId = c.MainAccountId,
                    AffectedUserName = c.MainAccountUsername,
                    DirectedAccountId = c.RootAccountId,
                    DirectedUserName = c.RootAccountUsername,
                    EvidenceCode = c.EviCode,
                    IsActive = c.IsActive
                })
                .OrderBy(c => c.DirectedUserName)
                .ThenBy(c => c.EvidenceCode);            
        }
        #endregion

        #region Evidence Management
        public IEnumerable<Evidence> GetAllEvidences()
        {
            var evidences = new List<Evidence>();

            evidences.Add(new Evidence { EvidenceGroupID = 1, EvidenceGroupName = "Financial", EvidenceTypeID = 1 , EvidenceTypeCode = "1.1", EvidenceTypeName = "Credit Card Fraud", Description = "Description Credit Card Fraud" });
            evidences.Add(new Evidence { EvidenceGroupID = 1, EvidenceGroupName = "Financial", EvidenceTypeID = 2, EvidenceTypeCode = "1.2", EvidenceTypeName = "ACH/Debit Fraud", Description = "Description ACH/Debit Fraud" });
            evidences.Add(new Evidence { EvidenceGroupID = 1, EvidenceGroupName = "Financial", EvidenceTypeID = 3, EvidenceTypeCode = "1.3", EvidenceTypeName = "Friendly Chargeback", Description = "Description Friendly Chargeback" });
            evidences.Add(new Evidence { EvidenceGroupID = 2, EvidenceGroupName = "Cheating", EvidenceTypeID = 4, EvidenceTypeCode = "2.1", EvidenceTypeName = "Collusion", Description = "Description Collusion" });
            evidences.Add(new Evidence { EvidenceGroupID = 2, EvidenceGroupName = "Cheating", EvidenceTypeID = 5, EvidenceTypeCode = "2.2", EvidenceTypeName = "Chip Dumping", Description = "Description Chip Dumping" });
            evidences.Add(new Evidence { EvidenceGroupID = 3, EvidenceGroupName = "Misconduct", EvidenceTypeID = 6, EvidenceTypeCode = "3.1", EvidenceTypeName = "Chat Abuse", Description = "Description Chat Abuse" });

            return evidences;
        }

        public IEnumerable<Evidence> GetEvidencesByAccount(int accountId)
        {
            return new List<Evidence>();
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

        public DBResult PlaceEvidenceByAccount(EvidenceByAccount evidenceByAccount)
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