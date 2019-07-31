namespace Fanex.NAP.FS.Library.Services
{
    using Fanex.NAP.FS.Library.ViewModels.SubscriberAccount.Evidence;    
    using Models.MatchMonitor.FraudCaseDetails;
    using System;    
    using System.Collections.Generic;
    using Fanex.NAP.FS.Library.Models.Subscriber;

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
            var evidenceAccounts = new List<EvidenceAccount>();
            {
                for (int i = 0; i < 5; i++)
                {
                    var item = new EvidenceAccount();
                    {
                        item.Id = 100 + i;
                        item.AffectedDate = DateTime.Now;
                        item.CentralPoint = "CentralPoint";
                        item.Comment = "Comment";
                        item.EvidenceKey = new Evidence { EvidenceCode = "1.1", EvidenceName = "abc" };
                    }

                    evidenceAccounts.Add(item);
                }
            }

            var listChainLevel1 = new List<ChainLevel>();
            {
                var level = new ChainLevel();
                {
                    level.EvidenceKey = new Evidence { EvidenceCode = "1.1", EvidenceName = "abc" };
                    level.OL1 = string.Empty;
                    level.OL2 = "level1";
                }

                listChainLevel1.Add(level);
            }

            var listChainLevel2 = new List<ChainLevel>();
            {
                var level = new ChainLevel();
                {
                    level.EvidenceKey = new Evidence { EvidenceCode = "1.2", EvidenceName = "xyz" };
                    level.OL1 = "level1";
                    level.OL2 = "level2";
                }

                listChainLevel2.Add(level);
            }

            var data = new ChainsToEvidence();
            {
                data.CentralPoint = accountId.ToString();
                data.Subscriber = "Subscriber";
                data.ChainsEvidenceType = "Account";
                data.Evidences = evidenceAccounts;
                data.Level1 = listChainLevel1;
                data.Level2 = listChainLevel2;
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