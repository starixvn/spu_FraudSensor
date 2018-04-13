namespace Fanex.NAP.FS.Library.ViewModels.MatchMonitor
{
    using System.Collections.Generic;
    using System.Linq;

    public class FraudCase
    {
        public FraudCase()
        {
        }

        public FraudCase(int id, string name, IEnumerable<Library.Models.MatchMonitor.FraudCase> data)
        {
            FraudCaseName = name;
            FraudCaseId = id;

            if (data.All(f => !f.SuspiciousTicketsCount.HasValue))
            {
                Violations = null;
            }
            else
            {
                Violations = data.Sum(f => f.SuspiciousTicketsCount.GetValueOrDefault());
            }
        }

        public int FraudCaseId { get; set; }

        public string FraudCaseName { get; set; }

        public int? Violations { get; set; }
    }
}