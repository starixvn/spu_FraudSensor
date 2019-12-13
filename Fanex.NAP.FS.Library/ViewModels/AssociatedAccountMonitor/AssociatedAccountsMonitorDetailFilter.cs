namespace Fanex.NAP.FS.Library.ViewModels.AssociatedAccountMonitor
{ 
    using System;
    using System.Collections.Generic;

    public class AssociatedAccountsMonitorDetailFilter
    {
        public int AccountCustId { get; set; }

        public string Username { get; set; }

        public int NoOfAssociations { get; set; }

        public DateTime DateFrom { get; set; }

        public DateTime DateTo { get; set; }

        public string Sites { get; set; }

        public string CustLevels { get; set; }

        public string EvidenceCodes { get; set; }

        public VerifyStatus VerifyStatus { get; set; }

        public string EvidencesCache { get; set; }

        public IEnumerable<Evidence> GetEvidencesCache()
        {
            var evidences = new List<Evidence>();

            if (this.EvidencesCache != null)
            {
                var evidenceItems = this.EvidencesCache.Split(';');

                foreach (var item in evidenceItems)
                {
                    var evidence = item.Split(':');

                    if (evidence.Length == 2)
                    {
                        evidences.Add(new Evidence
                        {
                            EvidenceCode = evidence[0],
                            EvidenceName = evidence[1]
                        });
                    }
                }
            }

            return evidences;
        }
    }
}
