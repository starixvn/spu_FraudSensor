namespace Fanex.NAP.FS.Library.ViewModels.Account.AssociatedToMonitor
{
    using System;
    using System.Collections.Generic;

    public class AssociatedAccountsToMonitorFilter
    {
        public DateTime AssociatedDateFrom { get; set; }

        public DateTime AssociatedDateTo { get; set; }

        public VerifyStatus VerifyStatus { get; set; }

        public string EvidenceCodes { get; set; }

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