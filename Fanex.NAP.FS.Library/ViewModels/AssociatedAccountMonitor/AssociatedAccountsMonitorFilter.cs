namespace Fanex.NAP.FS.Library.ViewModels.AssociatedAccountMonitor
{
    using System;
    using System.Collections.Generic;

    public class AssociatedAccountsMonitorFilter
    {
        public DateTime AssociatedDateFrom { get; set; }

        public DateTime AssociatedDateTo { get; set; }

        public string Sites { get; set; }

        public string CustLevels { get; set; }

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